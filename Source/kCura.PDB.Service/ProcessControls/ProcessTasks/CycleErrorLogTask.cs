namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class CycleErrorLogTask : BaseProcessControlTask, IProcessControlTask
	{
		public CycleErrorLogTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
			: base(logger, sqlRepo, agentId)
		{

		}

		public ProcessControlId ProcessControlID => ProcessControlId.CycleErrorLog;

		public bool Execute(ProcessControl processControl)
		{
			if (SqlRepo.AdminScriptsInstalled() == false)
			{
				LogWarning("Installation of Performance Dashboard is incomplete. Please install the latest scripts from PDB's custom pages.");
				return true;
			}

			var processCtrl = this.SqlRepo.ProcessControlRepository.ReadById(ProcessControlId.CycleErrorLog);
			var updateLastExecutedTime = false;
			if (processCtrl == null)
			{
				return true;
			}

			var execSucceeded = processCtrl.LastExecSucceeded;
			var utcDate = DateTime.UtcNow;
			try
			{
				this.Log("Called");

				var interval = processCtrl.Frequency.GetValueOrDefault(10080);

				if (interval > 0 && processCtrl.LastProcessExecDateTime.AddMinutes(interval) <= utcDate)
				{
					updateLastExecutedTime = true;

					// QoS_CycleErrorLog needs to be run on every active SQL server associated with Relativity
					var servers = this.SqlRepo.GetRegisteredSQLServers();

					foreach (var server in servers)
					{
						// Run the procedure
						SqlRepo.CycleSqlErrorLog(server.Name);
					}

					execSucceeded = true;
				}
				else
				{
					this.Log("Called - Skipping due to Interval");
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				this.LogError("Called - Failure. Details: {0}", message);
				execSucceeded = false;
				processCtrl.LastErrorMessage = ex.ToString();
			}
			finally
			{
				if (updateLastExecutedTime)
				{
					processCtrl.LastProcessExecDateTime = utcDate;
				}

				processCtrl.LastExecSucceeded = execSucceeded;
				this.SqlRepo.ProcessControlRepository.Update(processControl);
			}
			
			return true;
		}
	}
}
