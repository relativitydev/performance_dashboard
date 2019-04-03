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
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Enum;
	using kCura.PDD.Web.Extensions;
	using kCura.PDD.Web.Models;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDD.Web.Services;


	public class UserExperienceSearchController : UserExperienceController
	{
		private readonly IRequestService requestService;

		public UserExperienceSearchController()
		{
			// TODO - Get rid of inheritence
			this.requestService = new RequestService();
		}
		[System.Web.Http.HttpGet]
		public HttpResponseMessage Search(int id)
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new UserExperienceService(sqlRepo);
			var model = PopulateSearchModelSettings(id);

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.UserExperienceSearchState] = model;

			//Get the data
			var dtResponse = FetchSearchGridData(svc, model);

			//Serialize response
			var json = dtResponse.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		[System.Web.Http.HttpPost]
		public HttpResponseMessage GenerateCSV(int id)
		{
			//Initialize service and model 
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new UserExperienceService(sqlRepo);

			var model = PopulateSearchModelSettings(id);
			model.GridConditions.StartRow = 1;
			model.GridConditions.EndRow = int.MaxValue;
			//Get the data
			var fetchedData = FetchFileData(svc, model);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fetchedData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("UserExperienceSearch-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		private string FetchFileData(UserExperienceService svc, UserExperienceSearchViewModel viewModel)
		{
			var filterResult = FilterData(svc, viewModel, true);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[] { "Hour", "Search", "User", "Percent Long-Running", "Complex / Simple", "Total Run Time", "Average Run Time", "Total Runs", "QoS Hour ID", "Weekly Sample" };
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		/// <summary>
		/// Translates a Search view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchSearchGridData(UserExperienceService svc, UserExperienceSearchViewModel viewModel)
		{
			var filterData = FilterData(svc, viewModel, false);
			var dtResponse = new DataTableResponse()
			{
				sEcho = string.IsNullOrEmpty(viewModel.GridConditions.sEcho)
					? "1"
					: viewModel.GridConditions.sEcho,
				aaData = filterData.Data,
				recordsTotal = filterData.Data.Count(),
				recordsFiltered = filterData.TotalRecordCount
			};

			return dtResponse;
		}

		private FilterResult FilterData(UserExperienceService svc, UserExperienceSearchViewModel viewModel, bool isDownload)
		{
			var grid = svc.SearchUsers(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var aaData = grid.Data.Select(d => new string[] {
				$"{d.SummaryDayHour.ToShortDateString()} {d.SummaryDayHour.ToString("t")}",
				isDownload ?
					String.Format("{0} ({1})", d.Search, d.SearchArtifactId) :
					String.Format(@"<a onclick=""javascript:window.open('/Relativity/Case/History/QueryTextPopup.aspx?AppID={0}&amp;AuditID={1}&amp;AuditEmement=28','HistoryPopup','height=600,width=800,location=no,scrollbars=no,menubar=no,toolbar=no,status=no,resizable=yes');"" href=""#"">{2}</a>",
					d.CaseArtifactId,
					d.AuditId,
					String.Format("{0} ({1})", d.Search, d.SearchArtifactId)),
				$"{d.User} ({d.UserArtifactId})",
				$"{d.PercentLongRunning}%",
				d.IsComplex ? "Complex" : "Simple",
				d.TotalRunTime.ToString(),
				d.AverageRunTime.ToString(),
				d.TotalRuns.ToString(),
				d.QoSHourID.ToString(),
				d.IsActiveWeeklySample ? "Yes" : "No"
			}).ToArray();
			return new FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Search View
		/// </summary>
		/// <returns></returns>
		private UserExperienceSearchViewModel PopulateSearchModelSettings(int workspaceId)
		{
			var model = new UserExperienceSearchViewModel();
			var queryParams = this.requestService.GetQueryParamsDecoded(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (SearchViewColumns?)System.Enum.Parse(typeof(SearchViewColumns), iSortCol.Value);
			var sortBy = sortColumn.HasValue
				? sortColumn.Value.ToString()
				: null;
			model.GridConditions.SortIndex = iSortCol.Value;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDir.Value;

			//Filter conditions
			var hourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var searchFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var userFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var pctLRQFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var complexFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var totalRunTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var avgRunTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");
			var numberOfRunsFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_7");
			var qosHourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_8");
			var weeklyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_9");

			int avgRunTime, numberOfRuns, percentLongRunning;
			Int64 qosHour, totalRunTime;
			DateTime hour;
			model.FilterConditions = new SearchViewFilterConditions()
			{
				SummaryDayHour = DateTime.TryParse(HttpUtility.UrlDecode(hourFilter.Value), out hour)
					? hour
					: (DateTime?)null,
				CaseArtifactId = workspaceId,
				Search = searchFilter.Value,
				User = userFilter.Value,
				TotalRunTime = Int64.TryParse(totalRunTimeFilter.Value, out totalRunTime)
					? totalRunTime
					: (Int64?)null,
				AverageRunTime = int.TryParse(avgRunTimeFilter.Value, out avgRunTime)
					? avgRunTime
					: (int?)null,
				NumberOfRuns = int.TryParse(numberOfRunsFilter.Value, out numberOfRuns)
					? numberOfRuns
					: (int?)null,
				PercentLongRunning = int.TryParse(pctLRQFilter.Value.Replace("%", string.Empty), out percentLongRunning)
					? percentLongRunning
					: (int?)null,
				IsComplex = string.IsNullOrEmpty(complexFilter.Value)
					? null
					: (bool?)complexFilter.Value.Equals("complex", StringComparison.CurrentCultureIgnoreCase),
				QoSHourID = Int64.TryParse(qosHourFilter.Value, out qosHour)
					? qosHour
					: (Int64?)null,
				IsActiveWeeklySample = string.IsNullOrEmpty(weeklyFilter.Value)
					? null
					: (bool?)weeklyFilter.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
			};

			//Filter operands
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_3").Value, out model.FilterOperands.PercentLongRunning);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_5").Value, out model.FilterOperands.TotalRunTime);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_6").Value, out model.FilterOperands.AverageRunTime);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_7").Value, out model.FilterOperands.TotalRuns);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_8").Value, out model.FilterOperands.QoSHourID);

			return model;
		}
	}
}