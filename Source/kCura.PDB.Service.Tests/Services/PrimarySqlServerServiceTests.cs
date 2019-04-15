namespace kCura.PDB.Service.Tests.Services
{
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Services;
	//using kCura.PDD.Model.Repositories;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class PrimarySqlServerServiceTests
	{
		[Test, Category("Unit")]
		public void UpdatePrimarySqlServer_Success()
		{
			// TODO move this test to correct project or remove the test. repository tests should be integration.
			
			//var primaryServer = new ResourceServer();
			//var sqlServerRepositoryMock = new Mock<ISqlServerRepository>();
			//var servers = new ServerInfo[] {};
			//sqlServerRepositoryMock.Setup(s => s.GetRegisteredSQLServers()).Returns(servers);

			//var primarySqlServerRepositoryMock = new Mock<IPrimarySqlServerRepository>();
			//primarySqlServerRepositoryMock.Setup(p => p.GetPrimarySqlServer()).Returns(primaryServer);
			//foreach (var targetServer in servers)
			//	primarySqlServerRepositoryMock.Setup(p => p.UpdatePrimarySqlServer(targetServer.Name, primaryServer));
			//sqlServerRepositoryMock.Object.PrimarySqlServerRepository = primarySqlServerRepositoryMock.Object;
			//var connectionString = "";
			
			//// Act
			//var service = new PrimarySqlServerService(primarySqlServerRepositoryMock.Object,
			//	sqlServerRepositoryMock.Object);
			//service.UpdatePrimarySqlServer(connectionString);

			//// Assert
			//primarySqlServerRepositoryMock.VerifyAll();
		}
	}
}
