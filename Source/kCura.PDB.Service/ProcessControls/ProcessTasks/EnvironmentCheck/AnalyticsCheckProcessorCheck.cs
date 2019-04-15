namespace kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;

	[Description(AnalyticsCheckProcessorCheck.AnalyticsCheckProcessorCheckDescription)]
	public class AnalyticsCheckProcessorCheck : BaseProcessControlTask, IProcessControlTask
	{
		public AnalyticsCheckProcessorCheck(IWMIHelper wmiHelper, ISqlServerRepository sqlRepo, int agentId)
			: base(sqlRepo, agentId)
		{
			this.wmiHelper = wmiHelper;
		}

		private const string AnalyticsCheckProcessorCheckDescription = "Environment Check - Analytics Processor Check";
		private readonly IWMIHelper wmiHelper;

		public bool Execute(ProcessControl processControl)
		{
			var numberOfConfiguredConnectors = GetConfiguredConnectors();

			GetServers()
			.Where(s => s.ServerTypeId == (int)ServerType.Analytics)
			.Select(s => new { Server = s, Cores = GetServerCores(s) })
			.ForEach(s => SaveServerRecommendation(s.Server, s.Cores, numberOfConfiguredConnectors));
			return true;
		}

		public void SaveServerRecommendation(Server server, Int32 cores, Int32 numberOfConfiguredConnectors)
		{
			SqlRepo.AnalyticsRepository.SaveAnalyticsRecommendation(server,
				cores == numberOfConfiguredConnectors ? Guids.EnvironmentCheck.ContentAnalystMaxConnectorsPerIndexDefaultGood : Guids.EnvironmentCheck.ContentAnalystMaxConnectorsPerIndexDefaultWarning,
				cores.ToString());
		}

		public int GetServerCores(Server server)
		{
			var processors = this.wmiHelper.CreateDiagnostics(
				server,
				ManagementField.Name | ManagementField.NumberOfCores | ManagementField.NumberOfLogicalProcessors,
				"Win32_Processor",
				AnalyticsCheckProcessorCheck.AnalyticsCheckProcessorCheckDescription);

			var numOfCores = processors.Where(q => q.Key == ManagementField.NumberOfCores.ToString())
										.Select(q => (Int32)Convert.ToDecimal(q.Value.ToString()))
										.Sum();

			return numOfCores;
		}

		public int GetConfiguredConnectors()
		{
			var connectorsValue = SqlRepo.ConfigurationRepository.ReadConfigurationValue("Relativity.Core", "ContentAnalystMaxConnectorsPerIndexDefault");
			return Convert.ToInt32(connectorsValue);
		}

		public ProcessControlId ProcessControlID
		{
			get { return ProcessControlId.EnvironmentCheckServerInfo; }
		}
	}
}
