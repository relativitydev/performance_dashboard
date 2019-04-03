namespace kCura.PDB.Service.ProcessControls.HealthPerformance
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data;

	public class DiskHealthPerformanceTask : HealthPerformanceTask
	{
		internal override void SavePerformanceMetrics()
		{
			using (var dataContext = new PDDModelDataContext())
			{
				Logger.LogVerbose("SavePerformanceMetrics Called for Disk", this.GetType().Name);

				try
				{
					if (ServerList.Count > 0)
					{
						dataContext.ServerDiskDWs.InsertAllOnSubmit(ServerList.OfType<ServerDiskDW>().ToList());
						dataContext.SubmitChanges();
					}

					Logger.LogVerbose(string.Format("SavePerformanceMetrics Called for Disk - Success. Server Count: {0}", ServerList.Count),
											 this.GetType().Name);

				}
				catch (Exception ex)
				{
					Logger.LogError(string.Format("SavePerformanceMetrics Called for Disk - Failure. Details: {0}", ex.Message),
							 this.GetType().Name);
					throw ex;
				}
			}
		}

		internal override void ProcessServer(Server server)
		{
			try
			{
				Logger.LogVerbose("ProcessServer Called for Disk", this.GetType().Name);

				var name = CreateDiagnostics(server, ManagementField.Name, "Win32_PerfFormattedData_PerfDisk_LogicalDisk", "");
				var diskReadPerSec = CreateDiagnostics(server, ManagementField.DiskReadsPersec, "Win32_PerfFormattedData_PerfDisk_LogicalDisk", "");
				var diskWritePerSec = CreateDiagnostics(server, ManagementField.DiskWritesPersec, "Win32_PerfFormattedData_PerfDisk_LogicalDisk", "");
				var diskSpaceFree = CreateDiagnostics(server, ManagementField.FreeMegabytes, "Win32_PerfFormattedData_PerfDisk_LogicalDisk", "");
				var diskSecPerRead = CreateDiagnostics(server, ManagementField.AvgDiskSecPerRead, "Win32_PerfRawData_PerfDisk_LogicalDisk", "");
				var diskSecPerWrite = CreateDiagnostics(server, ManagementField.AvgDiskSecPerWrite, "Win32_PerfRawData_PerfDisk_LogicalDisk", "");
				var diskSecPerReadBase = CreateDiagnostics(server, ManagementField.AvgDiskSecPerRead_Base, "Win32_PerfRawData_PerfDisk_LogicalDisk", "");
				var diskSecPerWriteBase = CreateDiagnostics(server, ManagementField.AvgDiskSecPerWrite_Base, "Win32_PerfRawData_PerfDisk_LogicalDisk", "");
				var perfFrequency = CreateDiagnostics(server, ManagementField.Frequency_PerfTime, "Win32_PerfRawData_PerfDisk_LogicalDisk", "");
				//var driveLetter = CreateDiagnostics(server, ManagementField.Name, "Win32_LogicalDisk", "");

				var lists = new List<string>();
				var k = 0;
				for (var i = 0; i < name.Count; i++)
				{
					if (!name[i].Value.ToString().Contains("HarddiskVolume")
							&& !name[i].Value.ToString().Contains("_Total"))
					{
						var db = new ServerDiskDW();
						db.CreatedOn = DateTime.UtcNow;
						db.ServerID = server.ServerId;
						db.DiskNumber = k++;
						db.DriveLetter = name[i].Value.ToString();
						db.DiskAvgReadsPerSec = decimal.Parse((diskReadPerSec[i].Value ?? string.Empty).ToString());
						db.DiskAvgWritesPerSec = decimal.Parse((diskWritePerSec[i].Value ?? string.Empty).ToString());
						db.DiskFreeMegabytes = int.Parse((diskSpaceFree[i].Value ?? string.Empty).ToString());
						db.DiskSecPerRead = Int64.Parse((diskSecPerRead[i].Value ?? string.Empty).ToString());
						db.DiskSecPerReadBase = Int64.Parse((diskSecPerReadBase[i].Value ?? string.Empty).ToString());
						db.FrequencyPerfTime = Int64.Parse((perfFrequency[i].Value ?? string.Empty).ToString());
						db.DiskSecPerWrite = Int64.Parse((diskSecPerWrite[i].Value ?? string.Empty).ToString());
						db.DiskSecPerWriteBase = Int64.Parse((diskSecPerWriteBase[i].Value ?? string.Empty).ToString());
						ServerList.Add(db);
					}
				}

				Logger.LogVerbose("ProcessServer Called for Disk - Success", this.GetType().Name);
			}
			catch (Exception ex)
			{
				Logger.LogError(string.Format("ProcessServer Called for Disk - Failure. Server: {0}. Details: {1}", server.ServerName, ex.Message),
						this.GetType().Name);
				throw ex;
			}
		}
	}
}
