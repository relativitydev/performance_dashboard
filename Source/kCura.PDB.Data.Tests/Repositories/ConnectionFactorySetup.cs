namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.IO;

	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Tests.Properties;
	using kCura.PDB.Tests.Common;

	public static class ConnectionFactorySetup
	{
		private static object lockObj = new object();
		private static readonly string[] dataSources = new[] { "MSSQLLocalDB", "v11.0", "v12.0", "v14.0" };
		private static readonly string databaseSuffix = Guid.NewGuid().ToString("N");
		private static IConnectionFactory _ConnectionFactory = null;
		private static readonly string[] sprocsToDeploy =
		{
			Resources.eddsdbo_RecoverabilityIntegritySummaryReport,
			Resources.eddsdbo_RecoverabilityObjectivesReport,
			Resources.eddsdbo_QoS_RecoverabilityIntegrityReport,
			Resources.eddsdbo_QoS_BackupDBCCReport,
			Resources.eddsdbo_QoS_RecoveryObjectivesReport,
			Resources.ReadDatabaseDbcc,
			Resources.eddsdbo_QoS_UptimeReport,
			Resources.QoS_ScoreHistory,
			Resources.eddsdbo_QoS_SystemLoadServerReport,
			Resources.eddsdbo_QoS_SystemLoadWaitsReport,
			Resources.eddsdbo_QoS_UserExperienceSearchReport,
			Resources.eddsdbo_QoS_UserExperienceServerReport,
			Resources.eddsdbo_QoS_UserExperienceWorkspaceReport,
			Resources.EVERYTIME_RecreateViews
		};

		private static readonly string[] testingTables =
		{
			Resources._0001_Create_MockHours,
			Resources._0002_Create_MockBackupSet,
			Resources._0003_Create_MockDbccServerResults,
			Resources._0004_Create_MockDatabasesChecked,
			Resources._0005_Create_MockServer,
			Resources._0006_Recreate_MockServer,
			Resources._0007_Recreate_MockHours
		};

		public static IConnectionFactory ConnectionFactory
		{
			get
			{
				SetupConnectionFactory();
				return _ConnectionFactory;
			}
		}

		private static void SetupConnectionFactory()
		{
			lock (lockObj)
			{
				if (_ConnectionFactory == null)
				{
					if (Config.UseLocalDbForPlatformTests)
					{
						CreateInMemoryConnectionFactory();
					}
					else
					{
						_ConnectionFactory = TestUtilities.GetIntegrationConnectionFactory();
					}
				}
			}
		}

		public static void CreateInMemoryConnectionFactory()
		{
			var mdfPath = $"{Path.GetTempFileName()}.mdf";
			File.WriteAllBytes(mdfPath, Resources.EDDSPerformance);

			foreach (var dataSource in dataSources)
			{
				var connFactory = new LocalDbConnectionFactory(dataSource, mdfPath, databaseSuffix);
				if (connFactory.TestDataSource())
				{
					_ConnectionFactory = connFactory;
					break;
				}
				_ConnectionFactory = null;
			}
			if (_ConnectionFactory == null)
			{
				throw new Exception("Could not create in memory database");
			}

			RunDeployScript(Resources.eddsperf);
			RunDeployScript(Resources.EddsPerformanceExtra);
			RunDeployScript(Resources.EddsDependentModifiedSprocs);
			sprocsToDeploy.ForEach(RunDeployScript);
			testingTables.ForEach(RunDeployScript);
		}

		public static void RunDeployScript(string text)
		{
			text = text
					.Replace(@"[{{resourcedbname}}]", $"[{Names.Database.EddsPerformance}{databaseSuffix}]")
					.Replace(@"[EDDSQoS]", $"[{Names.Database.EddsPerformance}{databaseSuffix}]")
					.Replace(@"[EDDS]", $"[{Names.Database.EddsPerformance}{databaseSuffix}]")
					.Replace(@"WITH EXEC AS SELF", string.Empty);

			using (var conn = _ConnectionFactory.GetEddsPerformanceConnection())
			{
				foreach (var statement in text.Split(new[] { "\nGO" }, StringSplitOptions.RemoveEmptyEntries))
				{
					try
					{
						conn.Execute(statement);
					}
					catch
					{
						Console.WriteLine(statement);
						throw;
					}
				}
			}
		}

		public static void DeleteExisting()
		{
			try
			{
				using (var conn = _ConnectionFactory?.GetMasterConnection())
				{
					if (conn == null)
					{
						return;
					}

					conn.Open();
					var exists =
						conn.QueryFirstOrDefault<int?>($"select 1 from sys.databases where name = 'eddsperformance{databaseSuffix}'");
					if (exists.HasValue && exists >= 1)
					{
						conn.Execute($"alter database [EDDSPerformance{databaseSuffix}] set single_user with rollback immediate");
						conn.Execute($"ALTER DATABASE [EDDSPerformance{databaseSuffix}] SET OFFLINE");
						conn.Execute($"drop database [EDDSPerformance{databaseSuffix}]");
					}

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Couldn't delete database: {ex.ToString()}");
			}
		}

	}
}
