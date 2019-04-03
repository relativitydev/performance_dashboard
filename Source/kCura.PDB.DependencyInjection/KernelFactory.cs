namespace kCura.PDB.DependencyInjection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using global::Ninject;
	using global::Ninject.Extensions.ChildKernel;
	using global::Ninject.Modules;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service;
	using kCura.PDB.Service.DataGrid;
	using Service.Audits;

	public class KernelFactory : IKernelFactory
	{
		private static readonly Lazy<IReadOnlyDictionary<Type, Type>> AutoBindings = new Lazy<IReadOnlyDictionary<Type, Type>>(GetAutoBindableTypes);
		private readonly INinjectModule[] modules;

		public KernelFactory(params INinjectModule[] modules)
		{
			this.modules = modules;
		}

		public static IKernel GetKernel(INinjectModule baseBindings, IList<Type> excludedTypes = null)
		{
			return GetKernel(new List<INinjectModule> { baseBindings }, excludedTypes);
		}

		public static IKernel GetKernel(IList<INinjectModule> baseBindings, IList<Type> excludedTypes = null)
		{
			INinjectModule dataGridBindings = null;
			try
			{
				dataGridBindings = new DataGridBindings();
			}
			catch
			{
				// Swallow exception and use alternate bindings
				dataGridBindings = new NoDataGridBindings();
			}

			var bindings = new List<INinjectModule> { new ServiceBindings(), dataGridBindings };
			bindings.AddRange(baseBindings);
			
			var allBindings = bindings.Where(e => e != null).ToArray();
			return new KernelFactory(allBindings).GetKernel(excludedTypes);
		}

		public virtual IKernel GetKernel(IList<Type> excludedTypes = null)
		{
			var kernel = new StandardKernel();
			ApplyAutoBindings(kernel, excludedTypes);
			if (this.modules.Any())
			{
				kernel.Load(this.modules);
			}

			kernel.Rebind<IKernelFactory>().ToConstant(this);
			return kernel;
		}

		public virtual IChildKernel GetChildKernel(IKernel parentKernel, IList<Type> excludedTypes = null)
		{
			var kernel = new ChildKernel(parentKernel);
			ApplyAutoBindings(kernel, excludedTypes);
			kernel.Rebind<IKernelFactory>().ToConstant(this);
			return kernel;
		}

		private static void ApplyAutoBindings(IKernel kernel, IList<Type> excludedTypes = null)
		{
			AutoBindings
				.Value
				.Where(abt => !(excludedTypes?.Any(et => et == abt.Key || et == abt.Value) ?? false))
				.Where(abt => typeof(IDisposable).IsAssignableFrom(abt.Value) == false) // filter out any IDisposable types since we're binding with Transient Scope
				.ForEach(l => kernel.Bind(l.Key).To(l.Value).InTransientScope());

			AutoBindings
				.Value
				.Where(abt => !(excludedTypes?.Any(et => et == abt.Key || et == abt.Value) ?? false))
				.Where(abt => typeof(IDisposable).IsAssignableFrom(abt.Value)) // find all IDisposable types and bind with Singleton Scope so it's disposed of when done
				.ForEach(l => kernel.Bind(l.Key).To(l.Value).InSingletonScope());
		}

		/// <summary>
		/// Matches interfaces with implementation for interfaces with single implementation
		/// </summary>
		/// <param name="assemblyNames">Names of assemblies to scan for interface implementations</param>
		/// <returns>Dictionary with interface implementation matches.</returns>
		internal static IReadOnlyDictionary<Type, Type> GetAutoBindableTypes()
		{
			EnsureAppDomainAssembliesLoaded();

			// Get all assemblies of the current executing AppDomain
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			// Get core assembly
			var interfaceAssemblies = assemblies.Where(asm =>
				asm.FullName.ToLower().Contains(Names.Application.CoreAssembly.ToLower()));

			// Get all of the interface types from the core assembly
			var interfaces = interfaceAssemblies.SelectMany(asm =>
				asm.GetTypes().Where(t => t.IsInterface && t.IsPublic && t.Namespace.ToLower().Contains("interfaces"))).ToList();

			// Matching assembly names with assemblies loaded in AppDomain
			var asms = assemblies.Where(IsAutoBindablePdbAssembly).ToList();

			// Matches interfaces with single implementation
			return asms.SelectMany(asm => asm.GetTypes()
					.Where(t => t.IsPublic && t.IsClass) // Get all the public classes
					.SelectMany(t => t.GetInterfaces().Select(i => new { type = t, intrfc = i })) // Get a list of interfaces for each class
					.Where(t => interfaces.Any(i => i == t.intrfc))) // Filter out classes that don't have an interface in our core interfaces list
				.ToLookup(t => t.intrfc) // group all the classes and interfaces into a grouping of 1 class to many interfaces
				.Where(l => l.Count() == 1) // filter the groupings where the class has only 1 interface
				.ToDictionary(l => l.Key, l => l.First().type); // create a dictionary
		}

		internal static void EnsureAppDomainAssembliesLoaded()
		{
			var currentAssemblies = new Dictionary<string, Assembly>();

			Assembly nextAssembly;
			while ((nextAssembly = GetNextAssembly(currentAssemblies)) != null)
			{
				currentAssemblies.Add(nextAssembly.GetName().Name, nextAssembly);
				var refernecedAssemblies = nextAssembly.GetReferencedAssemblies();
				refernecedAssemblies
					.Where(asm => IsAutoBindablePdbAssembly(asm) && currentAssemblies.Keys.All(k => k != asm.Name))
					.ForEach(asm => EnsureAppDomainAssemblyLoaded(asm));
			}
		}

		internal static Assembly EnsureAppDomainAssemblyLoaded(AssemblyName name) =>
			AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.GetName().Name == name.Name) ??
			AppDomain.CurrentDomain.Load(name);

		internal static Assembly GetNextAssembly(IDictionary<string, Assembly> currentAssemblies) =>
			AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => IsAutoBindablePdbAssembly(asm) && !currentAssemblies.Keys.Contains(asm.GetName().Name));

		internal static bool IsAutoBindablePdbAssembly(AssemblyName asm) =>
			(asm.Name.ToLower().StartsWith("kcura.pdb")
			|| asm.Name.ToLower().StartsWith("kcura.pdd"))
			&& AssembliesToExclude.All(n => !asm.Name.ToLower().Contains(n.ToLower())); // exclude assemblies

		internal static bool IsAutoBindablePdbAssembly(Assembly asm) => IsAutoBindablePdbAssembly(asm.GetName());

		internal static IList<string> AssembliesToExclude = new[] { Names.Application.DataGridServiceAssembly, Names.Application.AgentAssembly };
	}
}
