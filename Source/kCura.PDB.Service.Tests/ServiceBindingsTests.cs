namespace kCura.PDB.Service.Tests
{
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ServiceBindingsTests
	{
		[Test]
		public void ServiceModule_Load()
		{
			//Arrange
			var kernel = new Mock<IKernel>();
			kernel.SetupGet(k => k.Settings).Returns(new Mock<INinjectSettings>().Object);

			//Act
			var bindings = new ServiceBindings();
			bindings.OnLoad(kernel.Object);

			//Asset
			Assert.Pass();
		}
	}
}
