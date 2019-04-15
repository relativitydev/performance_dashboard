namespace kCura.PDB.Service.Tests.Logic.Hours
{
	using System;
	using System.Collections.Generic;
	using System.Data.Common;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Hours;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class HourCleanupLogicTests
	{
		private HourCleanupLogic hourCleanupLogic;
		private Mock<IHourRepository> hourRepositoryMock;
		private Mock<IServerRepository> serverRepositoryMock;
		private Mock<ICleanupTablesRepository> cleanupTablesRepositoryMock;
		private Mock<IDataIntegrityRepository> dataIntegrityRepositoryMock;
		private Mock<IServerCleanupRepository> serverCleanupRepositoryMock;
		private Mock<ILogger> loggerMock;

		[SetUp]
		public void SetUp()
		{
			this.hourRepositoryMock = new Mock<IHourRepository>();
			this.serverRepositoryMock = new Mock<IServerRepository>();
			this.cleanupTablesRepositoryMock = new Mock<ICleanupTablesRepository>();
			this.dataIntegrityRepositoryMock = new Mock<IDataIntegrityRepository>();
			this.serverCleanupRepositoryMock = new Mock<IServerCleanupRepository>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.hourCleanupLogic = new HourCleanupLogic(
				this.hourRepositoryMock.Object,
				this.serverRepositoryMock.Object,
				this.cleanupTablesRepositoryMock.Object,
				this.dataIntegrityRepositoryMock.Object,
				this.serverCleanupRepositoryMock.Object,
				this.loggerMock.Object);
		}

		[Test]
		public async Task CleanupForHour()
		{
			// Arrange
			var hourId = 3;
			var hour = new Hour { Id = hourId, HourTimeStamp = DateTime.UtcNow };
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);

			var primaryServer = new Server
			{
				ServerId = 1,
				ArtifactId = 123,
				ServerName = "SomeServerName",
				ServerType = ServerType.Database
			};
			var otherServer = new Server
			{
				ServerId = 2,
				ArtifactId = 1234,
				ServerName = "OtherServer",
				ServerType = ServerType.Database
			};
			var sqlServers = new List<Server> { primaryServer, otherServer };
			this.serverRepositoryMock.Setup(m => m.ReadAllActiveAsync()).ReturnsAsync(sqlServers);

			// Performance tables cleanup
			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupQoSGlassRunLog(
					It.Is<DateTime>(dt => dt == hour.HourTimeStamp.AddDays(DatabaseConstants.GlassRunLogDeleteThresholdDays)),
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupPerformanceTable(
					$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputCumulativeTable}",
					Names.Database.SummaryDayHourColumn,
					It.Is<DateTime>(dt => dt == hour.HourTimeStamp.AddDays(DatabaseConstants.PastQuarterThreshold)),
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupPerformanceTable(
					$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputDetailCumulativeTable}",
					Names.Database.SummaryDayHourColumn,
					It.Is<DateTime>(dt => dt == hour.HourTimeStamp.AddDays(DatabaseConstants.PastQuarterThreshold)),
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupDecommissionedServers(
					hour.HourTimeStamp,
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.dataIntegrityRepositoryMock.Setup(m => m.DropAllTriggersInCurrentDatabase()).Returns(Task.Delay(5));

			// Queuing QoS tasks
			var serverCleanupPrimary = new ServerCleanup
			{
				HourId = hourId,
				ServerId = primaryServer.ServerId,
				Success = false
			};
			var serverCleanupPrimaryResult = new ServerCleanup
			{
				Id = 1,
				HourId = serverCleanupPrimary.HourId,
				ServerId = serverCleanupPrimary.ServerId,
				Success = serverCleanupPrimary.Success
			};
			this.serverCleanupRepositoryMock.Setup(m => m.CreateAsync(It.Is<ServerCleanup>(s =>
					s.ServerId == serverCleanupPrimary.ServerId &&
					s.HourId == serverCleanupPrimary.HourId &&
					s.Success == serverCleanupPrimary.Success
				))).ReturnsAsync(serverCleanupPrimaryResult);

			var serverCleanupOther = new ServerCleanup
			{
				HourId = hourId,
				ServerId = otherServer.ServerId,
				Success = false
			};
			var serverCleanupOtherResult = new ServerCleanup
			{
				Id = 2,
				HourId = serverCleanupOther.HourId,
				ServerId = serverCleanupOther.ServerId,
				Success = serverCleanupOther.Success
			};
			this.serverCleanupRepositoryMock.Setup(m => m.CreateAsync(It.Is<ServerCleanup>(s =>
					s.ServerId == serverCleanupOther.ServerId &&
					s.HourId == serverCleanupOther.HourId &&
					s.Success == serverCleanupOther.Success
				))).ReturnsAsync(serverCleanupOtherResult);

			// Act
			await this.hourCleanupLogic.CleanupForHour(hourId);

			// Assert
			this.cleanupTablesRepositoryMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.hourRepositoryMock.VerifyAll();
			this.dataIntegrityRepositoryMock.VerifyAll();
			this.serverCleanupRepositoryMock.VerifyAll();
		}

		[Test]
		public async Task CleanupPerformanceTables()
		{
			// Arrange
			var hourId = 2;
			var hour = new Hour
			{
				Id = hourId,
				HourTimeStamp = DateTime.UtcNow
			};
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);

			// Performance tables cleanup
			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupQoSGlassRunLog(
					It.Is<DateTime>(dt => dt == hour.HourTimeStamp.AddDays(DatabaseConstants.GlassRunLogDeleteThresholdDays)),
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupPerformanceTable(
					$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputCumulativeTable}",
					Names.Database.SummaryDayHourColumn,
					It.Is<DateTime>(dt => dt == hour.HourTimeStamp.AddDays(DatabaseConstants.PastQuarterThreshold)),
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupPerformanceTable(
					$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputDetailCumulativeTable}",
					Names.Database.SummaryDayHourColumn,
					It.Is<DateTime>(dt => dt == hour.HourTimeStamp.AddDays(DatabaseConstants.PastQuarterThreshold)),
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupDecommissionedServers(
					hour.HourTimeStamp,
					Defaults.Database.DeleteBatchSize,
					true
				)).Returns(Task.Delay(5));

			this.dataIntegrityRepositoryMock.Setup(m => m.DropAllTriggersInCurrentDatabase()).Returns(Task.Delay(5));

			// Act
			await this.hourCleanupLogic.CleanupPerformanceTables(hourId);

			// Assert
			this.hourRepositoryMock.VerifyAll();
			this.cleanupTablesRepositoryMock.VerifyAll();
			this.dataIntegrityRepositoryMock.VerifyAll();
		}

		[Test]
		public async Task CleanupQosTables()
		{
			// Arrange
			var serverCleanupId = 3;
			var hourId = 3;
			var serverId = 2;
			var serverCleanup = new ServerCleanup
			{
				Id = serverCleanupId,
				HourId = hourId,
				ServerId = serverId
			};
			this.serverCleanupRepositoryMock.Setup(m => m.ReadAsync(serverCleanupId)).ReturnsAsync(serverCleanup);
			var hour = new Hour
			{
				Id = hourId,
				HourTimeStamp = DateTime.UtcNow
			};
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);
			var serverName = "TestServer";
			var server = new Server
			{
				ServerId = serverId,
				ServerName = serverName
			};
			this.serverRepositoryMock.Setup(m => m.ReadAsync(serverId)).ReturnsAsync(server);


			this.dataIntegrityRepositoryMock.Setup(m => m.DropAllTriggersInCurrentDatabase(server)).Returns(Task.Delay(5));
			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupQosTable(
				server.ServerName,
				$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputTable}",
				Names.Database.SummaryDayHourColumn,
				It.Is<DateTime>(dt => dt == hour.HourTimeStamp.AddDays(DatabaseConstants.PastWeekThreshold)),
				Defaults.Database.DeleteBatchSize,
				true
			)).Returns(Task.Delay(5));

			this.cleanupTablesRepositoryMock.Setup(m => m.CleanupQosTable(
				server.ServerName,
				$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputDetailTable}",
				Names.Database.TimestampColumn,
				hour.HourTimeStamp.AddDays(DatabaseConstants.PastWeekThreshold),
				Defaults.Database.DeleteBatchSize,
				true
			)).Returns(Task.Delay(5));

			this.serverCleanupRepositoryMock.Setup(m => m.UpdateAsync(
				It.Is<ServerCleanup>(sc => sc.Id == serverCleanup.Id &&
				                           sc.ServerId == serverCleanup.ServerId &&
				                           sc.HourId == serverCleanup.HourId
				                           && sc.Success == true))).Returns(Task.Delay(5));

			// Act
			await this.hourCleanupLogic.CleanupQosTables(serverCleanupId);

			// Assert
			this.hourRepositoryMock.VerifyAll();
			this.serverCleanupRepositoryMock.VerifyAll();
			this.serverRepositoryMock.VerifyAll();
			this.dataIntegrityRepositoryMock.VerifyAll();
			this.cleanupTablesRepositoryMock.VerifyAll();
		}
	}
}
