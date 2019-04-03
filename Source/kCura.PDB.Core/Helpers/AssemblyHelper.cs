namespace kCura.PDB.Core.Helpers
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using kCura.PDB.Core.Extensions;

	public static class AssemblyHelper
	{
		public static Assembly CoreAssemblyReference => typeof(AssemblyHelper).Assembly;

		public static Version CoreAssemblyVersion => CoreAssemblyReference.GetName().Version;

		public static Version GetLatestVersion(Assembly executingAssembly)
		{
			// Start with the core version
			var version = CoreAssemblyVersion;

			// Get all the referenced assemblies from where we entered if we can
			var assemblies = executingAssembly.GetReferencedAssemblies();

			// Get the latest version from PDB's assemblies
			var latestVersion = assemblies.Where(a => a.Name.StartsWith($"{nameof(kCura)}.{nameof(PDB)}")).Max(a => a.Version);
			return latestVersion > version ? latestVersion : version;
		}

		public static void InitResolves()
		{
			// This is here to avoid conflicts with different Newtonsoft.Json dlls in varying versions of Relativity.
			Resolve("Newtonsoft.Json", "Newtonsoft.Json.dll");
			new[]
			{
				"kCura.Data.RowDataGateway",
				"kCura.Notification"
			}
			.ForEach(asm => ResolveAssembly(asm, "kCura"));
		}

		// Use our specific file
		public static void Resolve(string assemblyName, string dllName)
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				if (args.Name.Contains(assemblyName))
				{
					var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty);
					return Assembly.LoadFile(Path.Combine(currentDir, dllName));
				}

				return null;
			};
		}

		// Find this instead and use it
		public static void ResolveAssembly(string oldAssembly, string newAssembly)
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				if (args.Name.Contains(oldAssembly))
				{
					return Assembly.Load(newAssembly);
				}

				return null;
			};
		}
	}
}