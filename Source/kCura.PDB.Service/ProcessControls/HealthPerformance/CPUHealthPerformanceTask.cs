namespace kCura.PDB.Service.ProcessControls.HealthPerformance
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data;

	public class CPUHealthPerformanceTask : HealthPerformanceTask
	{
		internal override void ProcessServer(Server server)
		{
			try
			{
				Logger.LogVerbose("ProcessServer Called for CPU", this.GetType().Name);

				var procTimes = CreateDiagnostics(
					server,
					ManagementField.PercentProcessorTime,
					"Win32_PerfFormattedData_PerfOS_Processor",
					"WHERE NAME = '_TOTAL'");
				var procName = CreateDiagnostics(
					server,
					ManagementField.Name,
					"Win32_Processor",
					string.Empty);

				for (var i = 0; i < procTimes.Count; i++)
				{
					var sdw = new ServerProcessorDW
					{
						CreatedOn = DateTime.UtcNow,
						CoreNumber = -1,
						CPUProcessorTimePct = decimal.Parse(procTimes[i].Value),
						ServerID = server.ServerId,
						CPUName = procName[i].Value.Truncate(200)
					};
					ServerList.Add(sdw);
				}

				Logger.LogVerbose("ProcessServer Called for CPU - Success", this.GetType().Name);
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				Logger.LogError($"ProcessServer Called for CPU - Failure. Server: {server.ServerName}. Details: {message}",
						GetType().Name);
				throw;
			}
		}

		internal override void SavePerformanceMetrics()
		{
			using (var dataContext = new PDDModelDataContext())
			{
				Logger.LogVerbose("SavePerformanceMetrics Called for CPU", this.GetType().Name);
				try
				{
					if (ServerList.Count > 0)
					{
						dataContext.ServerProcessorDWs.InsertAllOnSubmit(ServerList.OfType<ServerProcessorDW>().ToList());
						dataContext.SubmitChanges();
					}

					Logger.LogVerbose($"SavePerformanceMetrics Called for CPU - Success. Server Count: {ServerList.Count}",
						this.GetType().Name);

				}
				catch (Exception ex)
				{
					var message = ex.GetExceptionDetails();
					Logger.LogError($"SavePerformanceMetrics Called for CPU - Failure. Details: {message}",
							 GetType().Name);
					throw ex;
				}
			}
		}
	}
}
