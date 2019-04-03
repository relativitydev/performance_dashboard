namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class SystemLoadService : BestInServiceReportingService
	{
		private readonly IReportRepository reportRepository;

		public SystemLoadService(ISqlServerRepository sqlServerRepository)
			: base(sqlServerRepository)
		{
			this.reportRepository = sqlServerRepository.ReportRepository;
		}

		public virtual IQueryable<SystemInformation> ReadSystemInformation()
		{
			var sysInfo = this.reportRepository.GetSystemInformation();
			var model = (from DataRow server in sysInfo.Rows
						 select new SystemInformation
						 {
							 ServerArtifactId = server.Field<int?>("ArtifactID").GetValueOrDefault(0),
							 ServerName = server.Field<string>("ServerName"),
							 TotalMemoryGB = (double?)server.Field<decimal?>("MemoryGB"),
							 FreeDiskSpaceGB = (double?)server.Field<decimal?>("DiskFreeGB"),
							 Processor = server.Field<string>("CPUName")
						 });
			return model.AsQueryable();
		}

		public virtual LoadViewGrid ServerHours(GridConditions gridConditions, LoadViewFilterConditions filterConditions, LoadViewFilterOperands filterOperands)
		{
			var grid = new LoadViewGrid();
			var dt = this.reportRepository.GetSystemLoadServerDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
							 select new SystemLoadServerHourInfo
							 {
								 Index = d.Field<int>("RowNumber"),
								 ServerId = d.Field<int>("ServerArtifactId"),
								 Server = d.Field<string>("Server"),
								 ServerType = d.Field<string>("ServerType"),
								 SummaryDayHour = d.Field<DateTime>("SummaryDayHour"),
								 OverallScore = d.Field<int?>("Score").GetValueOrDefault(100),
								 CPUScore = d.Field<int?>("CPUScore").GetValueOrDefault(100),
								 RAMScore = d.Field<int?>("RAMScore").GetValueOrDefault(100),
								 MemorySignalStateScore = d.Field<int?>("MemorySignalStateScore").GetValueOrDefault(100),
								 MemorySignalStateRatio = d.Field<int?>("MemorySignalStateRatio").GetValueOrDefault(0),
								 Pageouts = d.Field<int?>("Pageouts").GetValueOrDefault(0),
								 WaitsScore = d.Field<int?>("WaitsScore").GetValueOrDefault(100),
								 VirtualLogFilesScore = d.Field<int?>("VirtualLogFilesScore").GetValueOrDefault(100),
								 MaxVirtualLogFiles = d.Field<int?>("MaxVirtualLogFiles").GetValueOrDefault(0),
								 LatencyScore = d.Field<int?>("LatencyScore").GetValueOrDefault(100),
								 HighestLatencyDatabase = d.Field<string>("HighestLatencyDatabase"),
								 ReadLatencyMs = d.Field<int?>("ReadLatencyMs").GetValueOrDefault(0),
								 WriteLatencyMs = d.Field<int?>("WriteLatencyMs").GetValueOrDefault(0),
								 IsDataFile = d.Field<bool?>("IsDataFile").GetValueOrDefault(false),
								 IsActiveWeeklySample = d.Field<bool>("IsActiveArrivalRateSample")
							 }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}
			return grid;
		}

		public virtual WaitsViewGrid Waits(GridConditions gridConditions, WaitsViewFilterConditions filterConditions, WaitsViewFilterOperands filterOperands)
		{
			var grid = new WaitsViewGrid();
			var dt = this.reportRepository.GetSystemLoadWaitsDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
							 select new SystemLoadWaitsInfo
							 {
								 Index = d.Field<int>("RowNumber"),
								 ServerId = d.Field<int>("ServerArtifactId"),
								 Server = d.Field<string>("Server"),
								 WaitType = d.Field<string>("WaitType"),
								 SummaryDayHour = d.Field<DateTime>("SummaryDayHour"),
								 OverallScore = d.Field<int?>("WaitsScore").GetValueOrDefault(100),
								 SignalWaitsRatio = d.Field<int?>("SignalWaitsRatio").GetValueOrDefault(0),
								 SignalWaitTime = d.Field<long?>("SignalWaitTime").GetValueOrDefault(0),
								 TotalWaitTime = d.Field<long?>("TotalWaitTime").GetValueOrDefault(0),
								 IsPoisonWait = d.Field<bool>("IsPoisonWait"),
								 IsActiveWeeklySample = d.Field<bool>("IsActiveArrivalRateSample"),
								 PercentOfCPUThreshold = d.Field<decimal?>("PercentOfCPUThreshold").GetValueOrDefault(0),
								 DifferentialWaitingTasksCount = d.Field<Int64?>("DifferentialWaitingTasksCount").GetValueOrDefault(0),
							 }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}
			return grid;
		}
	}
}
