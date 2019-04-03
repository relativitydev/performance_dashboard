namespace kCura.PDB.EventHandler.Test
{
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class InstallRequirementsPreInstall_TestSuite
	{
		private Mock<IPreInstallService> preInstallServiceMock;
		private InstallRequirementsPreInstallEvent eventHandler;

		[SetUp]
		public void Setup()
		{
			this.preInstallServiceMock = new Mock<IPreInstallService>();
			this.eventHandler = new InstallRequirementsPreInstallEvent(this.preInstallServiceMock.Object);
		}

		[Test]
		public void InstallRequirementsPreInstall_Execute()
		{
			// Arrange
			var expectedResponse = new ApplicationInstallResponse { Success = true, Message = string.Empty };
			this.preInstallServiceMock.Setup(m => m.RunEveryTimeAsync()).ReturnsAsync(expectedResponse);

			// Act
			var response = this.eventHandler.Execute();

			// Assert
			Assert.That(response.Success, Is.EqualTo(expectedResponse.Success));
			Assert.That(response.Message, Is.EqualTo(expectedResponse.Message));
			this.preInstallServiceMock.VerifyAll();
		}

		[Test]
		public void InstallRequirementsPreInstall_Execute_Fails()
		{
			// Arrange
			var expectedResponse = new ApplicationInstallResponse { Success = false, Message = "The thing failed" };
			this.preInstallServiceMock.Setup(m => m.RunEveryTimeAsync()).ReturnsAsync(expectedResponse);

			// Act
			var response = this.eventHandler.Execute();

			// Assert
			Assert.That(response.Success, Is.EqualTo(expectedResponse.Success));
			Assert.That(response.Message, Is.EqualTo(expectedResponse.Message));
		}
	}
}
