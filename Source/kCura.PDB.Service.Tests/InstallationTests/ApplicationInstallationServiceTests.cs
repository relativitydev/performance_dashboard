namespace kCura.PDB.Service.Tests.InstallationTests
{
    using System.Threading.Tasks;

    using kCura.Data.RowDataGateway;
    using kCura.PDB.Core.Interfaces.DatabaseDeployment;
    using kCura.PDB.Core.Interfaces.Repositories;
    using kCura.PDB.Core.Interfaces.Services;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Service.Installation;
    using kCura.PDB.Service.Logging;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    [Category("Unit")]
    public class ApplicationInstallationServiceTestsUnit
    {
        private ApplicationInstallationService applicationInstallationService;
        private TextLogger textLogger;

        private Mock<ISqlServerRepository> sqlServerRepositoryMock;
        private Mock<IConnectionFactory> connectionFactoryMock;
        private Mock<IHashConversionService> hashConverterMock;
        private Mock<ITabService> tabServiceMock;
        private Mock<IDatabaseDeployer> databaseDeployerMock;

        [SetUp]
        public void SetUp()
        {
            this.connectionFactoryMock = new Mock<IConnectionFactory>();
            this.sqlServerRepositoryMock = new Mock<ISqlServerRepository>();
            this.hashConverterMock = new Mock<IHashConversionService>();
            this.tabServiceMock = new Mock<ITabService>();
            this.databaseDeployerMock = new Mock<IDatabaseDeployer>();
            this.textLogger = new TextLogger();

            this.applicationInstallationService = new ApplicationInstallationService(
                this.connectionFactoryMock.Object,
                this.sqlServerRepositoryMock.Object,
                this.hashConverterMock.Object,
                this.tabServiceMock.Object,
                this.databaseDeployerMock.Object,
                this.textLogger);
        }

        [Test]
        public async Task EventHandler_FailsWhenUpgradingToRoundhouseFails()
        {
            //Arrange
            this.sqlServerRepositoryMock.Setup(x => x.UpgradeIfMissingRoundhouse()).Throws(new ExecuteSQLStatementFailedException());

            //Act
            var response = await this.applicationInstallationService.InstallApplication();

            //Assert
            Assert.IsFalse(response.Success);
        }

        [Test]
        public async Task EventHandler_UpgradesLegacyHashes()
        {
            //Arrange
            this.sqlServerRepositoryMock.Setup(x => x.PerformanceExists()).Returns(true);

            //Act
            var response = await this.applicationInstallationService.InstallApplication();

            //Assert
            this.hashConverterMock.Verify(
                x => x.ConvertHashes(It.IsAny<ISqlServerRepository>(), It.IsAny<ServerInfo[]>()), Times.Once);
        }

        [Test]
        public async Task EventHandler_SkipsLegacyHashUpgradeWhenPerformanceNotExists()
        {
            //Arrange
            this.sqlServerRepositoryMock.Setup(x => x.PerformanceExists()).Returns(false);

            //Act
            var response = await this.applicationInstallationService.InstallApplication();

            //Assert
            this.hashConverterMock.Verify(
                x => x.ConvertHashes(It.IsAny<ISqlServerRepository>(), It.IsAny<ServerInfo[]>()), Times.Never);
        }
    }
}
