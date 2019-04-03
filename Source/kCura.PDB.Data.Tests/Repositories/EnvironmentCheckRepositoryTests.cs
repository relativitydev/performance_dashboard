namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Tests.Common;
	using kCura.PDD.Web.Enum;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class EnvironmentCheckRepositoryTests
	{
		private EnvironmentCheckRepository ecRepo;

		[SetUp]
		public void SetUp()
		{
			this.ecRepo = new EnvironmentCheckRepository(TestUtilities.GetIntegrationConnectionFactory());
		}
		
		[Test]
		public void ExecuteTuningForkRelativity()
		{
			//Arrange
			ecRepo.ExecuteTuningForkRelativity();

			//Assert

		}

		[Test]
		public void ExecuteDatabaseDetails_Success()
		{
			//Arrange

			//Act
			ecRepo.ExecuteCollectDatabaseDetails(Config.Server);

			//Assert

		}

		[Test]
		public void ExecuteTuningForkSystem_Success()
		{
			//Arrange
			var targetQoSServer = Config.Server;

			//Act
			ecRepo.ExecuteTuningForkSystem(targetQoSServer);

			//Assert

		}

		[Test]
		public void SaveServerDetails()
		{
			//Arrange
			var serverDetails = new EnvironmentCheckServerDetails
			{
				ServerName = Environment.MachineName,
				OSVersion = "asdf",
				OSName = "windows",
				LogicalProcessors = 4,
				Hyperthreaded = true,
			};

			//Act
			ecRepo.SaveServerDetails(serverDetails);

			//Assert

		}

		[Test]
		public void ReadCheckIFISettings_Success()
		{
			//Arrange
			var sqlRepo = new SqlServerRepository(ConnectionFactorySetup.ConnectionFactory);
			var dirs = sqlRepo.DeploymentRepository.ReadMdfLdfDirectories();

			//Act
			var result = sqlRepo.EnvironmentCheckRepository.ReadCheckIFISettings(dirs);

			//Assert
			Assert.That(result, Is.True);
		}
	}


	[TestFixture]
	[Category("Integration")]
	[Category("UnitPlatform")]
	public class EnvironmentCheckRepositoryUnitPlatformTests
	{
		private EnvironmentCheckRepository ecRepo;

		[SetUp]
		public void SetUp()
		{
			this.ecRepo = new EnvironmentCheckRepository(ConnectionFactorySetup.ConnectionFactory);
		}
		
		[Theory]
		public void GetRecomendations(EnvironmentCheckRecommendationColumns sortColumn)
		{
			//Arrange
			var gridCond = new GridConditions { SortColumn = sortColumn.ToString(), SortDirection = "asc", };
			var filterConds = new EnvironmentCheckRecommendationFilterConditions { };

			//Act
			var result = ecRepo.GetRecomendations(gridCond, filterConds);

			//Assert
			Assert.That(result, Is.Not.Null);
		}


		[Theory]
		public void GetServerDetails(EnvironmentCheckServerColumns sortColumn)
		{
			//Arrange
			var gridCond = new GridConditions { SortColumn = sortColumn.ToString(), SortDirection = "asc", };
			var filterConds = new EnvironmentCheckServerFilterConditions { };
			var filterOps = new EnvironmentCheckServerFilterOperands { };

			//Act
			var result = ecRepo.GetServerDetails(gridCond, filterConds, filterOps);

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Theory]
		public void GetDatabaseDetails(EnvironmentCheckDatabaseColumns sortColumn)
		{
			//Arrange
			var gridCond = new GridConditions { SortColumn = sortColumn.ToString(), SortDirection = "asc", };
			var filterConds = new EnvironmentCheckDatabaseFilterConditions { };
			var filterOps = new EnvironmentCheckDatabaseFilterOperands { };

			//Act
			var result = ecRepo.GetDatabaseDetails(gridCond, filterConds, filterOps);

			//Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
