namespace kCura.PDB.Core.Constants
{
	public static class Messages
	{
		public static class Notification
		{
			public const string NoAgentsEnabled = "There must be at least 1 {0} Agent(s) enabled.";
			public const string NoSqlAgentsReporting = "Please make sure your Sql Server Agent Service is enabled and running jobs.";
			public const string ProcessControlFailed = "{0} PDB Process Control(s) Failed when executing. {1}";
			public const string DeploymentFailure = "One or more EDDSQoS databases may have failed to be deployed correctly. The application may not run until the databases are deployed. You can retry deployment in the Backfill Console.";
		}

		public static class Exception
		{
			public const string PreInstallEventHandlerFailure = "Failed to disable agents due to exception: {0}";
			public const string PostInstallFailure = "Failed to execute PostInstall event handler due to exception: {0}";
			public const string PostInstallServerRefreshFailure = "Server Refresh Failed due to exception: {0}";
			public const string PostInstallUpdateServerQoSFailure = "Update servers pending QoS Deployment Failed due to exception: {0}";
			public const string PostInstallAgentEnableFailure = "Failed to enable agents automatically due to exception: {0}";
			public const string PostInstallAgentCreateFailure = "Failed to create agents automatically due to exception: {0}";
			public const string PostInstallReadServerId = "Failed to read a serverArtifactId for given machineName: {0}";
			public const string PostInstallCreateAgentFailure = "Failed to create agent of type {0} on serverArtifactId {1}";
		}

		public static class Warning
		{
			public const string PostInstallAgentCreateWarning = "Warning: Configuration to create agents post-install exists but is empty.  Skipping process.";
		}
	}
}
