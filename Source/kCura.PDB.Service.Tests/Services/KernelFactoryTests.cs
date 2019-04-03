namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.DependencyInjection;
	using kCura.PDB.Service.Services;
	using Ninject;
	using Ninject.Modules;
	using NUnit.Framework;

	[TestFixture]
	public class KernelFactoryTests
	{
		[Test]
		public void KernelFactory_Getkernel()
		{
			// Act
			var factory = new KernelFactory(new ServiceBindings(), new TestBindings());
			var result = factory.GetKernel(new[] { typeof(ILogContext) });

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Get<ITestType>(), Is.Not.Null);
			Assert.Throws<ActivationException>(() => result.Get<ILogContext>(), "ILogContext Should not be bound");
		}

		[Test]
		public void KernelFactory_GetAutoBindableTypes()
		{
			// Arrange
			var stopwatch = new Stopwatch();

			// Act
			stopwatch.Start();
			var results = KernelFactory.GetAutoBindableTypes();
			stopwatch.Stop();

			// Assert
			Assert.That(results, Is.Not.Empty);
			Assert.That(results.Values.Any(t => t.Assembly.GetName().Name.ToLower().StartsWith("kcura.pdb.data")), Is.True);
			Assert.That(results.Values.Any(t => t.Assembly.GetName().Name.ToLower().StartsWith("kcura.pdb.service")), Is.True);

			// Extra
			Console.WriteLine($"Bindings found in: {stopwatch.Elapsed.TotalMilliseconds}ms");
			//results.ForEach(kvp => Console.WriteLine($"{kvp.Key} => {kvp.Value}"));
		}

		[Test]
		public void KernelFactory_EnsureAppDomainAssembliesLoaded()
		{
			// Act
			KernelFactory.EnsureAppDomainAssembliesLoaded();

			// Assert
			Assert.Pass("no results returned");
		}

		internal class TestBindings : NinjectModule
		{
			public override void Load()
			{
				this.Bind<ITestType>().To<TestType2>();
			}
		}

		private interface ITestType
		{

		}

		private class TestType1 : ITestType
		{

		}

		private class TestType2 : ITestType
		{

		}
	}
}
