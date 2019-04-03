namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class UptimeService : BestInServiceReportingService
	{
		private readonly IReportRepository reportRepository;

		public UptimeService(ISqlServerRepository sqlServerRepository)
			: base(sqlServerRepository)
		{
			this.reportRepository = sqlServerRepository.ReportRepository;
		}

		public virtual UptimeViewGrid UptimeHours(GridConditions gridConditions, UptimeViewFilterConditions filterConditions, UptimeViewFilterOperands filterOperands)
		{
			var grid = new UptimeViewGrid();
			var dt = this.reportRepository.GetUptimeHours(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
							 select new UptimeReportHourInfo
							 {
								 Index = d.Field<int>("RowNumber"),
								 SummaryDayHour = d.Field<DateTime?>("SummaryDayHour").GetValueOrDefault(DateTime.UtcNow),
								 Score = d.Field<int?>("Score").GetValueOrDefault(0),
								 Status = d.Field<string>("Status"),
								 Uptime = (double)d.Field<decimal?>("Uptime").GetValueOrDefault(100),
                                 AffectedByMaintenanceWindow = d.Field<bool>("AffectedByMaintenanceWindow")
                             }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}

			return grid;
		}
	}
}
