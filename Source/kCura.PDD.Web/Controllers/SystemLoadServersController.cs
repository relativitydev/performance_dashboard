namespace kCura.PDD.Web.Controllers
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Web;
	using System.Web.SessionState;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Models.HealthChecks;
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

	public class SystemLoadServersController : SystemLoadController
	{
		public string ServerInfoFormat = @"<div title=""{0}"">{1}</div>";
		public string ServerHealthLinkFormat = @"<a href=""/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ServerHealth.aspx?StandardsCompliance=true&startDate={0}&endDate={1}"">{2}</a>";

		private readonly IRequestService requestService;

		public SystemLoadServersController()
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
			var svc = new SystemLoadService(sqlRepo);
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

			var model = PopulateServerModelSettings();

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.SystemLoadServerState] = model;

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
			message.Content.Headers.ContentDisposition.FileName = string.Format("SystemLoadServers-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		private string FetchFileData(SystemLoadService svc, SystemLoadServerViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
		{
			var filterResult = FilterData(svc, viewModel, true, qualityIndicatorService);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Hour", "Server", "Server Type", "Overall Score", "CPU Utilization Score",
					"RAM Utilization Score", "SQL Memory Signal Score", "SQL Waits Score",
					"Virtual Log Files Score", "Latency Score", "Weekly Sample"
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		private FilterResult FilterData(SystemLoadService svc, SystemLoadServerViewModel viewModel, bool IsDownload, IQualityIndicatorService qualityIndicatorService)
		{
			var grid = svc.ServerHours(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var sysInfo = svc.ReadSystemInformation().ToList();
			var aaData = grid.Data.Select(d => new string[] {
				IsDownload
					? $"{d.SummaryDayHour.ToShortDateString()} {d.SummaryDayHour.ToString("t")}"
					: $"<a href='{GetPageUrl(Names.Tab.ServerHealth, "ServerHealth", $"startDate={d.SummaryDayHour.ToShortDateString()}&endDate={d.SummaryDayHour.ToShortDateString()}")}' target='_parent'>{ d.SummaryDayHour.ToShortDateString()} {d.SummaryDayHour.ToString("t")} </a>",
				IsDownload
					? string.Format("{0} ({1})", d.Server, d.ServerId)
					: string.Format(@"<div style=""display:inline-block"" title=""{0}"" data-placement=""right"">{1}</div>",
						(sysInfo.FirstOrDefault(i => i.ServerArtifactId == d.ServerId) ?? new SystemInformation()).SummaryHtml,
						string.Format("{0} ({1})", d.Server, d.ServerId)
					  ),
				d.ServerType,
				IsDownload ?
					d.OverallScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.OverallScore, false)}'>{d.OverallScore}</span>",
				IsDownload ?
				d.CPUScore.ToString()
				: $"<a href='{GetPageUrl(Names.Tab.ServerHealth, "ServerHealth", $"startDate={d.SummaryDayHour.ToShortDateString()}&endDate={d.SummaryDayHour.ToShortDateString()}&measureType={(int)MeasureTypes.Processor}")}' target='_parent'>" +
						$"<span class='{qualityIndicatorService.GetCssClassForScore(d.CPUScore, false)}'>{d.CPUScore}</span>" +
					$"</a>",
				IsDownload ?
				d.RAMScore.ToString()
				: $"<a href='{GetPageUrl(Names.Tab.ServerHealth, "ServerHealth", $"startDate={d.SummaryDayHour.ToShortDateString()}&endDate={d.SummaryDayHour.ToShortDateString()}&measureType={(int)MeasureTypes.Ram}")}' target='_parent'>" +
						$"<span class='{qualityIndicatorService.GetCssClassForScore(d.RAMScore, false)}'>{d.RAMScore}</span>" +
					$"</a>",
				IsDownload || d.ServerType != "SQL"
					? d.FriendlyMemorySignalStateScore
					: $"<div style='display:inline-block' title='{d.MemorySignalStateRatio}% occurence of low memory<br/>{d.Pageouts} pageout(s)' data-placement='right'>" +
							$"<span class='{qualityIndicatorService.GetCssClassForScore(d.MemorySignalStateScore, false)}'>{d.MemorySignalStateScore}</span>" +
						$"</div>",
				IsDownload || d.ServerType != "SQL"
				? d.FriendlyWaitsScore
				: $"<a href='{GetPageUrl(d.ServerId, "Waits", $"Server={d.ServerId}&Hour={d.SummaryDayHour.ToString("s")}")}' target='_self'>" +
						$"<b class='{qualityIndicatorService.GetCssClassForScore(d.WaitsScore, false)}'>{d.FriendlyWaitsScore}</b>" +
					$"</a>",
				IsDownload || d.ServerType != "SQL"
					? d.FriendlyVirtualLogFilesScore
					:$"<div style='display:inline-block' title='{(d.MaxVirtualLogFiles > 0 ? d.MaxVirtualLogFiles + "virtual log file(s)" : string.Empty)}' data-placement='right'>" +
						$"<b class='{qualityIndicatorService.GetCssClassForScore(d.VirtualLogFilesScore, false)}'>{d.FriendlyVirtualLogFilesScore}</b>" +
					$"</div>",
				IsDownload || d.ServerType != "SQL"
					? d.FriendlyLatencyScore
					: $"<div style='display:inline-block' " +
							$"title='{(!string.IsNullOrEmpty(d.HighestLatencyDatabase) ? string.Format("{0} ({1} File)<br/>Read Latency: {2} ms<br/>Write Latency: {3} ms", d.HighestLatencyDatabase, d.DatabaseFileType, d.ReadLatencyMs, d.WriteLatencyMs) : string.Empty)} ' data-placement='right'>" +
							$"<b class='{qualityIndicatorService.GetCssClassForScore(d.LatencyScore, false)}'>{d.FriendlyLatencyScore}</b>" +
					$"</div>",
				d.IsActiveWeeklySample ? "Yes" : "No"
			}).ToArray();
			return new kCura.PDD.Web.Controllers.FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Server View
		/// </summary>
		/// <returns></returns>
		private SystemLoadServerViewModel PopulateServerModelSettings()
		{
			var model = new SystemLoadServerViewModel();

			var queryParams = this.requestService.GetQueryParamsDecoded(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (LoadViewColumns?)System.Enum.Parse(typeof(LoadViewColumns), iSortCol.Value);
			var sortBy = sortColumn?.ToString();
			model.GridConditions.SortIndex = iSortCol.Value;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDir.Value;

			//Filter conditions
			var hourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var serverFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var typeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var scoreFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var cpuFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var ramFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var memSignalFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");
			var waitsFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_7");
			var vlfFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_8");
			var latencyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_9");
			var weeklyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_10");

			int score, cpu, ram, pages, waits, vlf, latency;
			DateTime hour;
			model.FilterConditions = new LoadViewFilterConditions()
			{
				SummaryDayHour = DateTime.TryParse(HttpUtility.UrlDecode(hourFilter.Value), out hour)
					? hour
					: (DateTime?)null,
				Server = serverFilter.Value,
				ServerType = typeFilter.Value,
				OverallScore = int.TryParse(scoreFilter.Value, out score)
					? score
					: (int?)null,
				CPUScore = int.TryParse(cpuFilter.Value, out cpu)
					? cpu
					: (int?)null,
				RAMScore = int.TryParse(ramFilter.Value, out ram)
					? ram
					: (int?)null,
				MemorySignalScore = int.TryParse(memSignalFilter.Value, out pages)
					? pages
					: (int?)null,
				WaitsScore = int.TryParse(waitsFilter.Value, out waits)
					? waits
					: (int?)null,
				VirtualLogFilesScore = int.TryParse(vlfFilter.Value, out vlf)
					? vlf
					: (int?)null,
				LatencyScore = int.TryParse(latencyFilter.Value, out latency)
					? latency
					: (int?)null,
				IsActiveWeeklySample = string.IsNullOrEmpty(weeklyFilter.Value)
					? null
					: (bool?)weeklyFilter.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
			};

			//Filter operands
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_3").Value, out model.FilterOperands.OverallScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_4").Value, out model.FilterOperands.CPUUtilizationScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_5").Value, out model.FilterOperands.RAMUtilizationScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_6").Value, out model.FilterOperands.MemorySignalScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_7").Value, out model.FilterOperands.WaitsScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_8").Value, out model.FilterOperands.VirtualLogFilesScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_9").Value, out model.FilterOperands.LatencyScore);

			return model;
		}

		/// <summary>
		/// Translates a Server view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchServerGridData(SystemLoadService svc, SystemLoadServerViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
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