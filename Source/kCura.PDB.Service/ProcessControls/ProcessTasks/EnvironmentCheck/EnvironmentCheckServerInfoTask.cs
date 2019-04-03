namespace kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck
{
	using System;
	using System.ComponentModel;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Runtime.InteropServices;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using kCura.PDB.Service.Services;

	[Description(EnvironmentCheckServerInfoTask.ServerInfoTaskDescription)]
	public class EnvironmentCheckServerInfoTask : BaseProcessControlTask, IProcessControlTask
	{
		public EnvironmentCheckServerInfoTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
			: base(logger, sqlRepo, agentId)
		{
			_wmiHelper = new WMIHelper(logger);
			initialDatabase = Names.Database.EddsQoS;
		}

		private const String ServerInfoTaskDescription = "Environment Check Server Info";
		private readonly IWMIHelper _wmiHelper;

		public ProcessControlId ProcessControlID { get { return ProcessControlId.EnvironmentCheckServerInfo; } }

		public bool Execute(ProcessControl processControl)
		{
			if (SqlRepo.AdminScriptsInstalled() == false)
			{
				LogWarning("Installation of Performance Dashboard is incomplete. Please install the latest scripts from PDB's custom pages.");
				return true;
			}
			
		    bool bSuccess = true;
			base.ExecuteForServers(server =>
			{
			    try
			    {
			        if (server.ServerTypeId == (int)ServerType.Database)
			        {
			            SqlRepo.EnvironmentCheckRepository.ExecuteCollectDatabaseDetails(server.ServerName);
			        }

			        var serverDetails = GetServerDetails(server);
			        SqlRepo.EnvironmentCheckRepository.SaveServerDetails(serverDetails);
			    }
			    catch (COMException exception)
			    {
                    // Catch RPC exception and swallow for now, but log error to investigate and continue attempting to get details from other servers.
			        bSuccess = false;
                    LogError($"Caught exception ({exception.Message}) for server {server.ServerName}.  Skipping and continuing environment check for other servers.");
			    }
			});
			return bSuccess;
		}


		private EnvironmentCheckServerDetails GetServerDetails(Server srv)
		{
			var version = _wmiHelper.CreateDiagnostics(srv, ManagementField.Version, "Win32_OperatingSystem", EnvironmentCheckServerInfoTask.ServerInfoTaskDescription);
			var osNameCaption = _wmiHelper.CreateDiagnostics(srv, ManagementField.Caption, "Win32_OperatingSystem", EnvironmentCheckServerInfoTask.ServerInfoTaskDescription);
			var spversion = _wmiHelper.CreateDiagnostics(srv, ManagementField.ServicePackMajorVersion, "Win32_OperatingSystem", EnvironmentCheckServerInfoTask.ServerInfoTaskDescription);
			var servicePack = (spversion.Any() && Convert.ToDouble(spversion.First().Value) > 0) ? String.Format(" SP {0}", spversion.First().Value) : String.Empty;
			var osName = osNameCaption.First().Value + servicePack;

			var processors = _wmiHelper.CreateDiagnostics(srv, ManagementField.Name | ManagementField.NumberOfCores | ManagementField.NumberOfLogicalProcessors, "Win32_Processor", EnvironmentCheckServerInfoTask.ServerInfoTaskDescription);

			Int32 numOfCores = processors.Where(q => q.Key == ManagementField.NumberOfCores.ToString())
										.Select(q => (Int32)Convert.ToDecimal(q.Value.ToString()))
										.Sum();

			Int32 numOfLogicalProcessors = processors.Where(q => q.Key == ManagementField.NumberOfLogicalProcessors.ToString())
										.Select(q => (Int32)Convert.ToDecimal(q.Value.ToString()))
										.Sum();

			var isHyperthreaded = (numOfCores < numOfLogicalProcessors);

			var serverDetails = new EnvironmentCheckServerDetails
			{
				ServerName = srv.ServerName,
				OSVersion = version.First().Value,
				OSName = osName,
				LogicalProcessors = Convert.ToInt32(numOfLogicalProcessors),
				Hyperthreaded = isHyperthreaded,
				ServerIPAddress = srv.ServerIpAddress
			};

			return serverDetails;
		}


	}
}
