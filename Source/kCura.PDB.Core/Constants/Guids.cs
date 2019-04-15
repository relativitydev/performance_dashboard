namespace kCura.PDB.Core.Constants
{
	using System;
	using System.Collections.Generic;

	public static class Guids
	{
		public static class Agent
		{
			public const string MetricManagerAgentGuidString = "043E1D11-3161-4B44-B272-7D78D8F959E1";
			public const string TrustWorkerAgentGuidString = "4E7DBF83-C74A-462F-BF9A-4B4360ADBB42";
			public const string QoSManagerAgentGuidString = "79F33C93-4FC3-4E92-8C75-9D6F4073F334";
			public const string QosWorkerAgentGuidString = "D943F8E1-CB2F-40FD-BA66-3970D3AB17C0";
			public const string WmiWorkerAgentGuidString = "AA805282-5F1A-487E-AAC9-17E3E9A5B4BA";

			public static readonly Guid MetricManagerAgentGuid = Guid.Parse(MetricManagerAgentGuidString);
			public static readonly Guid TrustWorkerAgentGuid = Guid.Parse(TrustWorkerAgentGuidString);
			public static readonly Guid QoSManagerAgentGuid = Guid.Parse(QoSManagerAgentGuidString);
			public static readonly Guid QosWorkerAgentGuid = Guid.Parse(QosWorkerAgentGuidString);
			public static readonly Guid WmiWorkerAgentGuid = Guid.Parse(WmiWorkerAgentGuidString);

			public static readonly Guid OldTrustWorkerAgentGuid1 = Guid.Parse(OldTrustWorkerAgentGuid1String); // First old Trust GUID
			public static readonly Guid OldTrustWorkerAgentGuid2 = Guid.Parse(OldTrustWorkerAgentGuid2String); // Second old Trust GUID
			public static readonly Guid OldQoSMangerAgentGuid = Guid.Parse(OldQoSMangerAgentGuidString); // Old QoSManager GUID
			public static readonly Guid OldQoSWorkerAgentGuid = Guid.Parse(OldQoSWorkerAgentGuidString); // Old QoSWorker/Varscat GUID
			public static readonly Guid OldWmiWorkerAgentGuid = Guid.Parse(OldWmiWorkerAgentGuidString);    // Old WMIWorker GUID

			public static readonly IList<Guid> AgentGuidsToRemove = new[]
			{
				OldTrustWorkerAgentGuid1, // First old Trust GUID
				OldTrustWorkerAgentGuid2, // Second old Trust GUID
				OldQoSMangerAgentGuid, // Old QoSManager GUID
				OldQoSWorkerAgentGuid, // Old QoSWorker/Varscat GUID
				OldWmiWorkerAgentGuid,	// Old WMIWorker GUID
				// TrustWorkerAgentGuid, // Trust has been deprecated, but we don't want to risk manually removing the agent currently.
			};

			public static readonly IList<Guid> CurrentAgentGuids = new[]
			{
				MetricManagerAgentGuid,
				QoSManagerAgentGuid,
				QosWorkerAgentGuid,
				WmiWorkerAgentGuid
			};

			public static readonly IList<Guid> AllAgentGuids = new[]
			{
				OldTrustWorkerAgentGuid1,
				OldTrustWorkerAgentGuid2,
				OldQoSMangerAgentGuid,
				OldQoSWorkerAgentGuid,
				OldWmiWorkerAgentGuid,
				MetricManagerAgentGuid,
				TrustWorkerAgentGuid,
				QoSManagerAgentGuid,
				QosWorkerAgentGuid,
				WmiWorkerAgentGuid
			};

			private const string OldTrustWorkerAgentGuid1String = "514D019C-6BDC-40FF-A2E8-938B7C5E43B1";
			private const string OldTrustWorkerAgentGuid2String = "FDC70DC6-9DFB-4F8E-8D8C-3C7CFF515A47";
			private const string OldQoSMangerAgentGuidString = "5177338F-9B85-4AC5-A18A-BFAA98AF1804";
			private const string OldQoSWorkerAgentGuidString = "5C475C47-90F0-407B-828F-83A37E29F2F8";
			private const string OldWmiWorkerAgentGuidString = "93DCA58D-2E8E-4EF2-BB31-CA7F4354E91C";
		}

		public static class Credential
		{
			public static readonly Guid CredentialGuid = new Guid("B231AE06-6647-4A9E-815C-430D3DC0E53A"); // -- PDB Guid // new Guid("3A54C203-8C53-42C9-90DD-DD78EE246CA0"); -- AFS
		}

		public static class EnvironmentCheck
		{
			public static readonly Guid ContentAnalystMaxConnectorsPerIndexDefaultWarning = Guid.Parse("ab10edae-67f9-4636-a4c6-94d7ef20d705");
			public static readonly Guid ContentAnalystMaxConnectorsPerIndexDefaultGood = Guid.Parse("d0b7cf77-9ec7-4510-9fd0-be594522930e");

			public static readonly Guid CaatMemoryPerSearchableDocumentsDefaultWarning = Guid.Parse("46BC6D63-7054-4ECB-8DA2-7011DA7AD86C");
			public static readonly Guid CaatMemoryPerSearchableDocumentsDefaultGood = Guid.Parse("86B83F5F-0A95-4BAF-ADCE-0F5988A8CC0F");

			public static readonly Guid CaatMemoryPerTrainingDocumentsDefaultWarning = Guid.Parse("1E373E4C-980C-457C-AC64-99C1F7390809");
			public static readonly Guid CaatMemoryPerTrainingDocumentsDefaultGood = Guid.Parse("9643992C-90A2-4E36-93DF-5484ABFF7940");
		}

		public static class Application
		{
			public const string PerformanceDashboardString = "60a1d0a3-2797-4fb3-a260-614cbfd3fa0d";
			public const string DataGridString = "6A8C2341-6888-44DA-B1A4-5BDCE0D1A383";
			public const string DataGridForAuditString = "293c50ad-7b6d-45d0-9121-7f3826e1cca5";

			public static readonly Guid PerformanceDashboard = Guid.Parse(PerformanceDashboardString);
			public static readonly Guid DataGrid = Guid.Parse(DataGridString);
			public static readonly Guid DataGridForAudit = Guid.Parse(DataGridForAuditString);
		}
	}
}
