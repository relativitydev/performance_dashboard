namespace kCura.PDB.Service.Tests.InstallationTests
{
    using System;

    using kCura.PDB.Core.Interfaces.Services;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Service.Installation;
    using kCura.PDB.Service.Services;
    using kCura.PDB.Tests.Common;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    [Category("Unit")]
    public class EnsureSingleDeploymentInstallationServiceTests
    {
        private EnsureSingleDeploymentInstallationService deploymentInstallationService;

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
            this.deploymentInstallationService = new EnsureSingleDeploymentInstallationService(
                this.applicationInstallationServiceMock.Object,
                this.versionCheckServiceMock.Object,
                this.timeService,
                this.loggerMock.Object);
        }

        [Test]
        public void InstallApplication_AdminCase_Success()
        {
            // Arrange
            var response = new ApplicationInstallResponse { Success = true };
            this.applicationInstallationServiceMock.Setup(m => m.InstallApplication()).Returns(response).Verifiable();
            this.versionCheckServiceMock.Setup(m => m.UpdateLatestVersion(It.IsAny<Version>())).Verifiable();

            // Act
            var result = this.deploymentInstallationService.InstallApplication(-1);

            // Assert
            Assert.That(result.Success, Is.True);
            this.applicationInstallationServiceMock.VerifyAll();
            this.versionCheckServiceMock.VerifyAll();
        }

        [Test]
        public void InstallApplication_AdminCase_InstallFailure()
        {
            // Arrange
            var response = new ApplicationInstallResponse { Success = false };
            this.applicationInstallationServiceMock.Setup(m => m.InstallApplication()).Returns(response).Verifiable();

            // Act
            var result = this.deploymentInstallationService.InstallApplication(-1);

            // Assert
            Assert.That(result.Success, Is.False);
            this.applicationInstallationServiceMock.VerifyAll();
            this.versionCheckServiceMock.VerifyAll();
        }

        [Test]
        public void InstallApplication_AdminCase_UpdateVersionException()
        {
            // Arrange
            var response = new ApplicationInstallResponse { Success = true };
            this.applicationInstallationServiceMock.Setup(m => m.InstallApplication()).Returns(response).Verifiable();
            var exception = new Exception("Failed to update to latest version");
            this.versionCheckServiceMock.Setup(m => m.UpdateLatestVersion(It.IsAny<Version>())).Throws(exception);

            // Act
            var result = this.deploymentInstallationService.InstallApplication(-1);

            // Assert
            Assert.That(result.Success, Is.True);
            this.applicationInstallationServiceMock.VerifyAll();
            this.versionCheckServiceMock.VerifyAll();
            this.loggerMock.Verify(m => m.LogError(It.IsAny<string>(), exception, (string)null), Times.Once);
        }


        //[Test]
        public void InstallApplication_Workspace()
        {

            this.deploymentInstallationService.InstallApplication(51773);
        }
    }
}
