namespace kCura.PDB.Tests.Common
{
	using System;
	using System.Configuration;

	public static class Config
	{
		static Config()
		{
			WorkSpaceId = int.Parse(ConfigurationManager.AppSettings["WorkSpaceId"]);
			WorkSpaceName = ConfigurationManager.AppSettings["WorkSpaceName"];
			EddsdboUsername = ConfigurationManager.AppSettings["EddsdboUsername"];
			EddsdboPassword = ConfigurationManager.AppSettings["EddsdboPassword"];
			Server = ConfigurationManager.AppSettings["Server"];

			RSAPIServer = ConfigurationManager.AppSettings["RSAPIServer"];
			RSAPIUsername = ConfigurationManager.AppSettings["RSAPIUsername"];
			RSAPIPassword = ConfigurationManager.AppSettings["RSAPIPassword"];

			SAUserName = ConfigurationManager.AppSettings["SAUserName"];
			SAPassword = ConfigurationManager.AppSettings["SAPassword"];

			RelativityServiceUrl = ConfigurationManager.AppSettings["RelativityService"];
			RelativityRestUrl = ConfigurationManager.AppSettings["RelativityRest"];
			RelativityUserName = ConfigurationManager.AppSettings["RelativityUserName"];
			RelativityPassword = ConfigurationManager.AppSettings["RelativityPassword"];

			PrimaryConnectionString = ConfigurationManager.ConnectionStrings["relativity"].ConnectionString;
			WorkspaceConnectionString = ConfigurationManager.ConnectionStrings["relativityWorkspace"]?.ConnectionString ?? PrimaryConnectionString;


			HourTimestamp = ConfigurationManager.AppSettings["HourTimestamp"];

			UITestScreenshotLocation = ConfigurationManager.AppSettings["UITestScreenshotLocation"];

			DownloadPath = ConfigurationManager.AppSettings["DownloadPath"];

			var defaultUseLocalDbForPlatformTests = true;
			if (bool.TryParse(ConfigurationManager.AppSettings["UseLocalDbForPlatformTests"], out defaultUseLocalDbForPlatformTests))
			{
				UseLocalDbForPlatformTests = defaultUseLocalDbForPlatformTests;
			}
			else
			{
				UseLocalDbForPlatformTests = true;
			}

			bool managerAgentInRelativity;
			if (bool.TryParse(ConfigurationManager.AppSettings["MetricManagerAgentInRelativity"], out managerAgentInRelativity))
			{
				ManagerAgentInRelativity = managerAgentInRelativity;
			}
			else
			{
				ManagerAgentInRelativity = true;
			}

			bool workerAgentInRelativity;
			if (bool.TryParse(ConfigurationManager.AppSettings["WorkerAgentInRelativity"], out workerAgentInRelativity))
			{
				WorkerAgentInRelativity = workerAgentInRelativity;
			}
			else
			{
				WorkerAgentInRelativity = true;
			}
		}

		public static int WorkSpaceId { get; private set; }
		public static string WorkSpaceName { get; private set; }
		public static string EddsdboUsername { get; private set; }
		public static string EddsdboPassword { get; private set; }
		public static string Server { get; private set; }

		public static string RSAPIServer { get; private set; }
		public static string RSAPIUsername { get; private set; }
		public static string RSAPIPassword { get; private set; }

		public static string SAUserName { get; private set; }
		public static string SAPassword { get; private set; }

		public static string RelativityServiceUrl { get; private set; }
		public static string RelativityRestUrl { get; private set; }
		public static string RelativityUserName { get; private set; }
		public static string RelativityPassword { get; private set; }

		public static string PrimaryConnectionString { get; private set; }
		public static string WorkspaceConnectionString { get; private set; }

		public static string HourTimestamp { get; private set; }

		public static string UITestScreenshotLocation { get; private set; }

		public static bool UseLocalDbForPlatformTests { get; private set; }
		public static bool ManagerAgentInRelativity { get; private set; }
		public static bool WorkerAgentInRelativity { get; private set; }

		public static string DownloadPath { get; private set; }
	}
}