namespace kCura.PDB.Service.ProcessControls.HealthPerformance
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data;

	public class RamHealthPerformanceTask : HealthPerformanceTask
	{
		internal override void ProcessServer(Server server)
		{
			try
			{
				Logger.LogVerbose("ProcessServer Called for RAM", GetType().Name);

				var name = CreateDiagnostics(server, ManagementField.PagesPerSec | ManagementField.PageFaultsPerSec | ManagementField.AvailableKBytes, "Win32_PerfFormattedData_PerfOS_Memory", "");
				var totalMemory = CreateDiagnostics(server, ManagementField.TotalVisibleMemorySize, "Win32_OperatingSystem", "");

				var sdw = new ServerDW
				{
					ServerID = server.ServerId,
					CreatedOn = DateTime.UtcNow,
				};

				if (totalMemory.Count > 0)
					sdw.TotalPhysicalMemory = decimal.Parse(totalMemory[0].Value);

				decimal available = 0;
				foreach (var item in name)
				{
					if (item.Key.Equals(ManagementField.PageFaultsPerSec.ToString(), StringComparison.CurrentCultureIgnoreCase))
					{
						sdw.RAMPageFaultsPerSec = decimal.Parse(item.Value);
					}
					else if (item.Key.Equals(ManagementField.PagesPerSec.ToString(), StringComparison.CurrentCultureIgnoreCase))
					{
						sdw.RAMPagesPerSec = decimal.Parse(item.Value);
					}
					else if (item.Key.Equals(ManagementField.AvailableKBytes.ToString(), StringComparison.CurrentCultureIgnoreCase))
					{
						available = decimal.Parse(item.Value);
						sdw.AvailableMemory = available;
					}
				}

				//Get total physical memory and RAM %
				if (totalMemory.Count > 0)
				{
					var total = decimal.Parse(totalMemory[0].Value);

					if (total > 0)
					{
						sdw.TotalPhysicalMemory = total;
						sdw.RAMPct = decimal.Round((total - available) / total * 100, 2, MidpointRounding.AwayFromZero);
					}
				}

				ServerList.Add(sdw);

				Logger.LogVerbose("ProcessServer Called for RAM - Success", GetType().Name);
			}
			catch (Exception ex)
			{
				Logger.LogError(string.Format("ProcessServer Called for RAM - Failure. Server: {0}. Details: {1}", server.ServerName, ex.GetExceptionDetails()),
					GetType().Name);
				throw ex;
			}
		}

		internal override void SavePerformanceMetrics()
		{
			using (var dataContext = new PDDModelDataContext())
			{
				Logger.LogVerbose("SavePerformanceMetrics Called for RAM", GetType().Name);

				try
				{
					if (ServerList.Count > 0)
					{
						dataContext.ServerDWs.InsertAllOnSubmit(ServerList.OfType<ServerDW>().ToList());
						dataContext.SubmitChanges();
					}

					Logger.LogVerbose(string.Format("SavePerformanceMetrics Called for RAM - Success. Server Count: {0}", ServerList.Count),
						GetType().Name);
				}
				catch (Exception ex)
				{
					Logger.LogError(string.Format("SavePerformanceMetrics Called for RAM - Failure. Details: {0}", ex.Message),
						GetType().Name);
					throw ex;
				}
			}
		}
	}
}
