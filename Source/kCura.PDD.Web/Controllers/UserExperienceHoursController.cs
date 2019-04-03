using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using kCura.PDD.Web.Constants;
using kCura.PDD.Web.Enum;
using kCura.PDD.Web.Extensions;
using kCura.PDD.Web.Models;
using kCura.PDD.Web.Models.BISSummary;
using System.IO;
using System.Net.Http.Headers;


namespace kCura.PDD.Web.Controllers
{
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Extensions;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
    using kCura.PDB.Service.BISSummary;
    using kCura.PDD.Web.Services;

    public class UserExperienceHoursController : UserExperienceController
    {
        private readonly IRequestService requestService;

        public UserExperienceHoursController()
        {
            // TODO - Get rid of inheritence
            this.requestService = new RequestService();
        }

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Hours(int id)
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new UserExperienceService(sqlRepo);
			var model = PopulateHourModelSettings(id);

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.UserExperienceHoursState] = model;

			//Get the data
			var dtResponse = FetchHoursGridData(svc, model);

			//Serialize response
			var json = dtResponse.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		[System.Web.Http.HttpPost]
		public HttpResponseMessage GenerateCSV(int id)
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new UserExperienceService(sqlRepo);
			var model = PopulateHourModelSettings(id);
			model.GridConditions.StartRow = 1;
            model.GridConditions.EndRow = int.MaxValue;
			//Get the data
			var fetchedData = FetchFileData(svc, model);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fetchedData);
			message.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("UserExperienceHours-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		private string FetchFileData(UserExperienceService svc, UserExperienceHoursViewModel viewModel)
		{
			var filterResult = FilterData(svc, viewModel, true);
			using (var sw = new StringWriter())
			{
				var heaaderArr = new string[] { "Hour", "Database", "Search", "Complex / Simple", "Total Run Time", "Average Run Time", "Total Search Audits", "Weekly Sample" };
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		/// <summary>
		/// Translates an Hours view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchHoursGridData(UserExperienceService svc, UserExperienceHoursViewModel viewModel)
		{
			var data = FilterData(svc, viewModel, false);
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

		private FilterResult FilterData(UserExperienceService svc, UserExperienceHoursViewModel viewModel, bool IsDownload)
		{
			var grid = svc.WorkspaceSearches(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var aaData = grid.Data.Select(d => new string[] {
				//Hour
				String.Format("{0} {1}", d.SummaryDayHour.ToShortDateString(), d.SummaryDayHour.ToString("t")),
				//Workspace
				IsDownload ?
					d.Workspace :
					String.Format(SearchLinkFormat, d.WorkspaceId, String.Empty, d.Workspace),
				//Search
				IsDownload
					? $"{d.Search} ({d.SearchId})"
					: string.Format(SearchLinkFormat, d.WorkspaceId, $"&Search={d.SearchId}", $"{d.Search} ({d.SearchId})"),
				d.IsComplex ? "Complex" : "Simple",
				d.TotalRunTime.ToString(),
				d.AverageRunTime.ToString(),
				d.TotalRuns.ToString(),
				d.IsActiveWeeklySample ? "Yes" : "No"
			}).ToArray();
			return new FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Hours View
		/// </summary>
		/// <returns></returns>
		private UserExperienceHoursViewModel PopulateHourModelSettings(int server)
		{
			var model = new UserExperienceHoursViewModel();
			var queryParams = this.requestService.GetQueryParamsDecoded(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (HoursViewColumns?)System.Enum.Parse(typeof(HoursViewColumns), iSortCol.Value);
			var sortBy = sortColumn.HasValue
				? sortColumn.Value.ToString()
				: null;
			model.GridConditions.SortIndex = iSortCol.Value;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDir.Value;

			//Filter conditions
			var hourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var workspaceFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var searchFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var complexFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var totalRunTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var avgRunTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var numberOfRunsFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");
			var weeklyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_7");

			int avgRunTime, numberOfRuns;
			Int64 totalRunTime;
			DateTime hour;
			model.FilterConditions = new HoursViewFilterConditions()
			{
				SummaryDayHour = DateTime.TryParse(HttpUtility.UrlDecode(hourFilter.Value), out hour)
					? hour
					: (DateTime?)null,
				Server = server,
				Workspace = workspaceFilter.Value,
				Search = searchFilter.Value,
				TotalRunTime = Int64.TryParse(totalRunTimeFilter.Value, out totalRunTime)
					? totalRunTime
					: (Int64?)null,
				AverageRunTime = int.TryParse(avgRunTimeFilter.Value, out avgRunTime)
					? avgRunTime
					: (int?)null,
				NumberOfRuns = int.TryParse(numberOfRunsFilter.Value, out numberOfRuns)
					? numberOfRuns
					: (int?)null,
				IsComplex = string.IsNullOrEmpty(complexFilter.Value)
					? null
					: (bool?)complexFilter.Value.Equals("complex", StringComparison.CurrentCultureIgnoreCase),
				IsActiveWeeklySample = string.IsNullOrEmpty(weeklyFilter.Value)
					? null
					: (bool?)weeklyFilter.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
			};

			//Filter operands
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_4").Value, out model.FilterOperands.TotalRunTime);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_5").Value, out model.FilterOperands.AverageRunTime);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_6").Value, out model.FilterOperands.TotalRuns);

			return model;
		}
	}
}