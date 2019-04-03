namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class UserExperienceService : BestInServiceReportingService
	{
		private readonly IReportRepository reportRepository;

		public UserExperienceService(ISqlServerRepository sqlServerRepository)
			: base(sqlServerRepository)
		{
			this.reportRepository = sqlServerRepository.ReportRepository;
		}

		public virtual IQueryable<QoSServerInfo> ListDatabaseServers() => this.ListAllServers().Where(x => x.Name != "Web Servers");

		public virtual string ReadWorkspaceFriendlyName(int caseArtifactId)
		{
			return this.reportRepository.LookupWorkspaceName(caseArtifactId);
		}

		public virtual ServerViewGrid ServerWorkspaces(GridConditions gridConditions, ServerViewFilterConditions filterConditions, ServerViewFilterOperands filterOperands)
		{
			var grid = new ServerViewGrid();
			var dt = this.reportRepository.GetUserExperienceServerDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
										 select new UserExperienceServerWorkspaceInfo
										 {
											 Index = d.Field<int>("RowNumber"),
											 ServerId = d.Field<int>("ServerArtifactId"),
											 Server = d.Field<string>("Server"),
											 WorkspaceId = d.Field<int>("CaseArtifactId"),
											 Workspace = d.Field<string>("Workspace"),
											 SummaryDayHour = d.Field<DateTime>("SummaryDayHour"),
											 Score = d.Field<int?>("Score").GetValueOrDefault(0),
											 TotalUsers = d.Field<int?>("TotalUsers").GetValueOrDefault(0),
											 TotalLongRunning = d.Field<int?>("TotalLongRunning").GetValueOrDefault(0),
											 TotalSearchAudits = d.Field<int?>("TotalSearchAudits").GetValueOrDefault(0),
											 TotalNonSearchAudits = d.Field<int?>("TotalNonSearchAudits").GetValueOrDefault(0),
											 TotalAudits = d.Field<int?>("TotalAudits").GetValueOrDefault(0),
											 TotalExecutionTime = d.Field<Int64?>("TotalExecutionTime").GetValueOrDefault(0),
											 IsActiveWeeklySample = d.Field<bool>("IsActiveArrivalRateSample")
										 }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}

			return grid;
		}

		public virtual HoursViewGrid WorkspaceSearches(GridConditions gridConditions, HoursViewFilterConditions filterConditions, HoursViewFilterOperands filterOperands)
		{
			var grid = new HoursViewGrid();
			var dt = this.reportRepository.GetUserExperienceHourDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
										 select new UserExperienceWorkspaceHourInfo
										 {
											 Index = d.Field<Int64>("RowNumber"),
											 Workspace = d.Field<string>("DatabaseName"),
											 SearchId = d.Field<int>("SearchArtifactId"),
											 Search = d.Field<string>("SearchName"),
											 TotalRunTime = d.Field<int?>("TotalRunTime").GetValueOrDefault(0),
											 AverageRunTime = d.Field<int?>("AverageRunTime").GetValueOrDefault(0),
											 TotalRuns = d.Field<int?>("TotalRuns").GetValueOrDefault(0),
											 IsComplex = d.Field<bool?>("IsComplex").GetValueOrDefault(false),
											 SummaryDayHour = d.Field<DateTime>("SummaryDayHour"),
											 IsActiveWeeklySample = d.Field<bool>("IsActiveArrivalRateSample")
										 }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}

			return grid;
		}

		public virtual SearchViewGrid SearchUsers(GridConditions gridConditions, SearchViewFilterConditions filterConditions, SearchViewFilterOperands filterOperands)
		{
			var grid = new SearchViewGrid();
			var dt = this.reportRepository.GetUserExperienceSearchDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
										select new UserExperienceSearchUserInfo
										{
											Index = d.Field<int>("RowNumber"),
											CaseArtifactId = d.Field<int>("CaseArtifactID"),
											AuditId = d.Field<long?>("LastAuditID").GetValueOrDefault(0),
											SearchArtifactId = d.Field<int>("SearchArtifactId"),
											Search = d.Field<string>("Search"),
											UserArtifactId = d.Field<int>("UserArtifactID"),
											User = d.Field<string>("User"),
											TotalRunTime = d.Field<Int64?>("TotalRunTime").GetValueOrDefault(0),
											AverageRunTime = d.Field<int?>("AverageRunTime").GetValueOrDefault(0),
											TotalRuns = d.Field<int?>("TotalRuns").GetValueOrDefault(0),
											PercentLongRunning = d.Field<int?>("PercentLongRunning").GetValueOrDefault(0),
											IsComplex = d.Field<bool?>("IsComplex").GetValueOrDefault(false),
											SummaryDayHour = d.Field<DateTime>("SummaryDayHour"),
											QoSHourID = d.Field<Int64?>("QoSHourID").GetValueOrDefault(0),
											IsActiveWeeklySample = d.Field<bool>("IsActiveArrivalRateSample")
										}).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}

			return grid;
		}
	}
}
