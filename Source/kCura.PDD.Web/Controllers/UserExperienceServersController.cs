namespace kCura.PDD.Web.Controllers
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Web;
	using System.Web.SessionState;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Enum;
	using kCura.PDD.Web.Extensions;
	using kCura.PDD.Web.Models;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDD.Web.Services;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.BISSummary;

	public class UserExperienceServersController : UserExperienceController
	{
		private readonly IRequestService requestService;

		public UserExperienceServersController()
		{
			// TODO - Get rid of inheritence
			this.requestService = new RequestService();
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Servers()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new UserExperienceService(sqlRepo);
			var model = PopulateServerModelSettings();
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.UserExperienceServerState] = model;

			//Get the data
			var dtResponse = FetchServerGridData(svc, model, qualityIndicatorService);

			//Serialize response
			var json = dtResponse.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		[System.Web.Http.HttpPost]
		public HttpResponseMessage GenerateCSV()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new UserExperienceService(sqlRepo);
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

			var model = PopulateServerModelSettings();
			model.GridConditions.StartRow = 1;
			model.GridConditions.EndRow = int.MaxValue;
			//Get the data
			var fetchedData = FetchFileData(svc, model, qualityIndicatorService);

			//Serialize response
			var message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(fetchedData) };
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
			{
				FileName = $"UserExperienceServers-{DateTime.Now:yyyyMMdd-HHmmss}.csv"
			};
			return message;
		}

		private string FetchFileData(UserExperienceService svc, UserExperienceServerViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
		{
			var filterResult = FilterData(svc, viewModel, true, qualityIndicatorService);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Hour", "Server", "Score", "Workspace", "Long-Running Queries", "Total Users", "Total Search Audits",
					"Total Non-Search Audits", "Total Audits", "Total Execution Time", "Weekly Sample"
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Server View
		/// </summary>
		/// <returns></returns>
		protected UserExperienceServerViewModel PopulateServerModelSettings()
		{
			var model = new UserExperienceServerViewModel();
			var queryParams = this.requestService.GetQueryParamsDecoded(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (ServerViewColumns?)System.Enum.Parse(typeof(ServerViewColumns), iSortCol.Value);
			var sortBy = sortColumn?.ToString();
			model.GridConditions.SortIndex = iSortCol.Value;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDir.Value;

			//Filter conditions
			var hourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var serverFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var scoreFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var workspaceFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var totalLRQFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var totalUsersFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var totalSearchFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");
			var totalNonSearchFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_7");
			var totalAuditFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_8");
			var totalTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_9");
			var weeklyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_10");

			int totalLRQ, totalUsers, totalSearch, totalNonSearch, totalAudit, score;
			Int64 totalTime;
			DateTime hour;
			model.FilterConditions = new ServerViewFilterConditions()
			{
				SummaryDayHour = DateTime.TryParse(HttpUtility.UrlDecode(hourFilter.Value), out hour)
					? hour
					: (DateTime?)null,
				Server = serverFilter.Value,
				Score = int.TryParse(scoreFilter.Value, out score)
					? score
					: (int?)null,
				TotalLongRunning = int.TryParse(totalLRQFilter.Value, out totalLRQ)
					? totalLRQ
					: (int?)null,
				Workspace = workspaceFilter.Value,
				TotalUsers = int.TryParse(totalUsersFilter.Value, out totalUsers)
					? totalUsers
					: (int?)null,
				TotalSearchAudits = int.TryParse(totalSearchFilter.Value, out totalSearch)
					? totalSearch
					: (int?)null,
				TotalNonSearchAudits = int.TryParse(totalNonSearchFilter.Value, out totalNonSearch)
					? totalNonSearch
					: (int?)null,
				TotalAudits = int.TryParse(totalAuditFilter.Value, out totalAudit)
					? totalAudit
					: (int?)null,
				TotalExecutionTime = Int64.TryParse(totalTimeFilter.Value, out totalTime)
					? totalTime
					: (Int64?)null,
				IsActiveWeeklySample = string.IsNullOrEmpty(weeklyFilter.Value)
					? null
					: (bool?)weeklyFilter.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
			};

			//Filter operands
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_2").Value, out model.FilterOperands.Score);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_4").Value, out model.FilterOperands.TotalLongRunning);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_5").Value, out model.FilterOperands.TotalUsers);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_6").Value, out model.FilterOperands.TotalSearchAudits);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_7").Value, out model.FilterOperands.TotalNonSearchAudits);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_8").Value, out model.FilterOperands.TotalAudits);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_9").Value, out model.FilterOperands.TotalExecutionTime);

			return model;
		}

		/// <summary>
		/// Translates a Server view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchServerGridData(UserExperienceService svc, UserExperienceServerViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
		{
			var data = FilterData(svc, viewModel, false, qualityIndicatorService);
			var dtResponse = new DataTableResponse()
			{
				sEcho = string.IsNullOrEmpty(viewModel.GridConditions.sEcho)
					? "1"
					: viewModel.GridConditions.sEcho,
				aaData = data.Data,
				recordsTotal = data.Data.Count(),
				recordsFiltered = data.TotalRecordCount
			};

			return dtResponse;
		}

		private FilterResult FilterData(UserExperienceService svc, UserExperienceServerViewModel viewModel, bool isDownload, IQualityIndicatorService qualityIndicatorService)
		{
			var grid = svc.ServerWorkspaces(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var aaData = grid.Data.Select(d => new string[] {
				$"{d.SummaryDayHour.ToShortDateString()} {d.SummaryDayHour:t}",
				isDownload ?
					$"{d.Server} ({d.ServerId})"
					: string.Format(HoursLinkFormat, d.ServerId, string.Empty, $"{d.Server} ({d.ServerId})"),
				isDownload
					? d.Score.ToString()
					:$"<span class='{qualityIndicatorService.GetCssClassForScore(d.Score, false)}'>{d.Score}</span>",
				isDownload ?
					$"{d.Workspace} ({d.WorkspaceId})"
					: string.Format(HoursLinkFormat, d.ServerId, $"&Workspace={d.WorkspaceId}", $"{d.Workspace} ({d.WorkspaceId})"),
				d.TotalLongRunning.ToString(),
				d.TotalUsers.ToString(),
				d.TotalSearchAudits.ToString(),
				d.TotalNonSearchAudits.ToString(),
				d.TotalAudits.ToString(),
				d.TotalExecutionTime.ToString(),
				d.IsActiveWeeklySample ? "Yes" : "No"
			}).ToArray();
			return new FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}
	}
}