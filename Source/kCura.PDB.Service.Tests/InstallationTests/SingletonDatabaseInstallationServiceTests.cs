namespace kCura.PDB.Service.Tests.InstallationTests
{
    using System;
    using System.Threading.Tasks;

    using kCura.PDB.Core.Interfaces.Services;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Service.Installation;
    using kCura.PDB.Service.Services;
    using kCura.PDB.Tests.Common;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    [Category("Unit")]
    public class SingletonDatabaseInstallationServiceTests
    {
        private SingletonDatabaseInstallationService deploymentInstallationService;

        private Mock<IApplicationInstallationService> applicationInstallationServiceMock;
        private Mock<IVersionCheckService> versionCheckServiceMock;
        private ITimeService timeService;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void Setup()
        {
            this.applicationInstallationServiceMock = new Mock<IApplicationInstallationService>();
            this.versionCheckServiceMock = new Mock<IVersionCheckService>();
            this.timeService = TestUtilities.GetTimeService(1);
            this.loggerMock = TestUtilities.GetMockLogger();
            this.deploymentInstallationService = new SingletonDatabaseInstallationService(
                this.applicationInstallationServiceMock.Object,
                this.versionCheckServiceMock.Object,
                this.timeService,
                this.loggerMock.Object);
        }

        [Test]
        public async Task InstallApplication_AdminCase_Success()
        {
            // Arrange
            var response = new ApplicationInstallResponse { Success = true };
            this.applicationInstallationServiceMock.Setup(m => m.InstallApplication()).ReturnsAsync(response).Verifiable();
            this.versionCheckServiceMock.Setup(m => m.UpdateLatestVersion(It.IsAny<Version>())).Verifiable();

            // Act
            var result = await this.deploymentInstallationService.InstallApplication(-1);

            // Assert
            Assert.That(result.Success, Is.True);
            this.applicationInstallationServiceMock.VerifyAll();
            this.versionCheckServiceMock.VerifyAll();
        }

        [Test]
        public async Task InstallApplication_AdminCase_InstallFailure()
        {
            // Arrange
            var response = new ApplicationInstallResponse { Success = false };
            this.applicationInstallationServiceMock.Setup(m => m.InstallApplication()).ReturnsAsync(response).Verifiable();

            // Act
            var result = await this.deploymentInstallationService.InstallApplication(-1);

            // Assert
            Assert.That(result.Success, Is.False);
            this.applicationInstallationServiceMock.VerifyAll();
            this.versionCheckServiceMock.VerifyAll();
        }

        [Test]
        public async Task InstallApplication_AdminCase_UpdateVersionException()
        {
            // Arrange
            var response = new ApplicationInstallResponse { Success = true };
            this.applicationInstallationServiceMock.Setup(m => m.InstallApplication()).ReturnsAsync(response).Verifiable();
            var exception = new Exception("Failed to update to latest version");
            this.versionCheckServiceMock.Setup(m => m.UpdateLatestVersion(It.IsAny<Version>())).Throws(exception);

            // Act
            var result = await this.deploymentInstallationService.InstallApplication(-1);

            // Assert
            Assert.That(result.Success, Is.True);
            this.applicationInstallationServiceMock.VerifyAll();
            this.versionCheckServiceMock.VerifyAll();
            this.loggerMock.Verify(m => m.LogError(It.IsAny<string>(), exception, (string)null), Times.Once);
        }


        [Test]
        public async Task InstallApplication_Workspace_Success()
        {
            // Arrange
            this.versionCheckServiceMock.Setup(m => m.CurrentVersionIsInstalled(It.IsAny<Version>()))
                .ReturnsAsync(true);

            var result = await this.deploymentInstallationService.InstallApplication(51773);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        [Ignore("Takes too long for TeamCity")]
        public async Task InstallApplication_Workspace_SuccessDelayed()
        {
            // Arrange
            this.versionCheckServiceMock.SetupSequence(m => m.CurrentVersionIsInstalled(It.IsAny<Version>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            var result = await this.deploymentInstallationService.InstallApplication(51773);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        [Category("ignore")]
        [Ignore("Takes too long")]
        public async Task InstallApplication_Workspace_Failure()
        {
            // Arrange
            this.versionCheckServiceMock.Setup(m => m.CurrentVersionIsInstalled(It.IsAny<Version>()))
                .ReturnsAsync(false);

            var result = await this.deploymentInstallationService.InstallApplication(51773);

            Assert.That(result.Success, Is.False);
        }
    }
}
