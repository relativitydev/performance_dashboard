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
	using kCura.PDB.Core.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Enum;
	using kCura.PDD.Web.Extensions;
	using kCura.PDD.Web.Models;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDD.Web.Services;

	public class SystemLoadWaitsController : SystemLoadController
    {
        private readonly IRequestService requestService;

        public SystemLoadWaitsController()
        {
            // TODO - Get rid of inheritence
            this.requestService = new RequestService();
        }

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Waits()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new SystemLoadService(sqlRepo);
			var model = PopulateServerModelSettings();
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.SystemLoadWaitsState] = model;

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
			var svc = new SystemLoadService(sqlRepo);
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

			var model = PopulateServerModelSettings();
			model.GridConditions.StartRow = 1;
			model.GridConditions.EndRow = int.MaxValue;
			//Get the data
			var fetchedData = FetchFileData(svc, model, qualityIndicatorService);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fetchedData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("SystemLoadWaits-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		private string FetchFileData(SystemLoadService svc, SystemLoadWaitsViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
		{
			var filterResult = FilterData(svc, viewModel, true, qualityIndicatorService);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Hour", "Server", "Waits Score", "Signal Waits Ratio", "Processor Time Utilization", "Wait Type",
					"Signal Wait Time (ms)", "Total Wait Time (ms)","Waiting Task Count", "Poison Wait", "Weekly Sample"
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		private FilterResult FilterData(SystemLoadService svc, SystemLoadWaitsViewModel viewModel, bool isDownload, IQualityIndicatorService qualityIndicatorService)
		{
			var grid = svc.Waits(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var sysInfo = svc.ReadSystemInformation().ToList();
			var aaData = grid.Data.Select(d => new string[] {
				$"{d.SummaryDayHour.ToShortDateString()} {d.SummaryDayHour.ToString("t")}",
				$"{d.Server} ({d.ServerId})",
   				isDownload
                    ? d.OverallScore.ToString()
					:$"<span class='{qualityIndicatorService.GetCssClassForScore(d.OverallScore, false)}'>{d.OverallScore}</span>",
                $"{d.SignalWaitsRatio}%",
                d.PercentOfCPUThreshold.ToString("f2"),
				isDownload 
                    ? d.WaitType
                    : $"<div style='display:inline-block;' title='{d.WaitDescription}'>{d.WaitType}</div>",
				d.SignalWaitTime.ToString(),
                d.TotalWaitTime.ToString(),
                d.DifferentialWaitingTasksCount.ToString(),
                d.IsPoisonWait ? "Yes" : "No",
				d.IsActiveWeeklySample ? "Yes" : "No"
			}).ToArray();
			return new kCura.PDD.Web.Controllers.FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Server View
		/// </summary>
		/// <returns></returns>
		private SystemLoadWaitsViewModel PopulateServerModelSettings()
		{
			var model = new SystemLoadWaitsViewModel();

			var queryParams = this.requestService.GetQueryParamsDecoded(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (WaitsViewColumns?)System.Enum.Parse(typeof(WaitsViewColumns), iSortCol.Value);
			var sortBy = sortColumn?.ToString();
			model.GridConditions.SortIndex = iSortCol.Value;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDir.Value;

			//Filter conditions
			var hourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var serverFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var scoreFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var signalWaitsRatioFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var percentOfCPUThresholdFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var typeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var signalWaitTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");
			var waitTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_7");
			var differentialWaitingTasksCountFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_8");
			var poisonFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_9");
			var weeklyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_10");


			int score, ratio;
			long signalTime, waitTime, differentialWaitingTasksCount;
			DateTime hour;
			decimal percentOfCPUThreshold;
			model.FilterConditions = new WaitsViewFilterConditions()
			{
				SummaryDayHour = DateTime.TryParse(HttpUtility.UrlDecode(hourFilter.Value), out hour)
					? hour
					: (DateTime?)null,
				Server = serverFilter.Value,
				OverallScore = int.TryParse(scoreFilter.Value, out score)
					? score
					: (int?)null,
				SignalWaitsRatio = int.TryParse(signalWaitsRatioFilter.Value.Replace("%", string.Empty), out ratio)
					? ratio
					: (int?)null,
				WaitType = typeFilter.Value,
				SignalWaitTime = long.TryParse(signalWaitTimeFilter.Value, out signalTime)
					? signalTime
					: (long?)null,
				TotalWaitTime = long.TryParse(waitTimeFilter.Value, out waitTime)
					? waitTime
					: (long?)null,
				IsPoisonWait = string.IsNullOrEmpty(poisonFilter.Value)
					? null
					: (bool?)poisonFilter.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase),
				IsActiveWeeklySample = string.IsNullOrEmpty(weeklyFilter.Value)
					? null
					: (bool?)weeklyFilter.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase),
				PercentOfCPUThreshold = (string.IsNullOrEmpty(percentOfCPUThresholdFilter.Value) || false == decimal.TryParse(percentOfCPUThresholdFilter.Value, out percentOfCPUThreshold))
					? null : (decimal?)percentOfCPUThreshold,
				DifferentialWaitingTasksCount = (string.IsNullOrEmpty(differentialWaitingTasksCountFilter.Value) || false == long.TryParse(differentialWaitingTasksCountFilter.Value, out differentialWaitingTasksCount))
					? null : (decimal?)differentialWaitingTasksCount,
			};

			//Filter operands
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_2").Value, out model.FilterOperands.OverallScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_3").Value, out model.FilterOperands.SignalWaitsRatio);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_6").Value, out model.FilterOperands.SignalWaitTime);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_7").Value, out model.FilterOperands.TotalWaitTime);
			
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_4").Value, out model.FilterOperands.PercentOfCPUThreshold);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_8").Value, out model.FilterOperands.DifferentialWaitingTasksCount);

			return model;
		}

		/// <summary>
		/// Translates a Server view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchServerGridData(SystemLoadService svc, SystemLoadWaitsViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
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
	}
}