namespace kCura.PDB.EventHandler.Test
{
	using kCura.PDB.Core.Interfaces;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class RunEveryTimePostInstallEventHandler_TestSuite
	{
		private Mock<IPostInstallService> postInstallServiceMock;
		private RunEveryTimePostInstallEventHandler eventHandler;
		
		[SetUp]
		public void Setup()
		{
			this.postInstallServiceMock = new Mock<IPostInstallService>();
			this.eventHandler = new RunEveryTimePostInstallEventHandler(this.postInstallServiceMock.Object);
		}

		[Test]
		public void Integration_ExecuteEventHandler()
		{
			//Arrange
			var expectedResponse = new ApplicationInstallResponse { Success = true };
			this.postInstallServiceMock.Setup(m => m.RunEveryTimeAsync()).ReturnsAsync(expectedResponse);
			
			//Act
			var response = this.eventHandler.Execute();

			//Assert
			Assert.IsTrue(response.Success, response.Message);
			this.postInstallServiceMock.VerifyAll();
		}
	}
}
