namespace kCura.PDB.Service.Tests.Queuing
{
	using kCura.PDB.Core.Models;
	using kCura.PDB.DependencyInjection;
	using Ninject;
	using NUnit.Framework;

	[TestFixture]
	public class EventChildKernelFactoryTests
	{
		[Test]
		public void CreateChildKernel()
		{
			// Arrange
			using (var kernel = new StandardKernel())
			{
				using (var factory = new EventChildKernelFactory(kernel))
				{
					// Act
					using (var childKernel = factory.CreateChildKernel(new Event()))
					{
						childKernel.Kernel.Bind<IThing>().To<Thing>();

						// Assert
						Assert.That(childKernel, Is.Not.Null, "child kernel wrapper is not null");
						Assert.That(childKernel.Kernel, Is.Not.Null, "child kernel is not null");
						Assert.That(childKernel.Kernel.Get<IThing>(), Is.Not.Null, "IThing is not null");
					}
				}
			}
		}

		public interface IThing
		{
		}

		public class Thing : IThing { }
	}
}
