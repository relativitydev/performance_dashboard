using kCura.PDD.Web.Constants;
using kCura.PDD.Web.Enum;
using kCura.PDD.Web.Models;
using kCura.PDD.Web.Models.BISSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.SessionState;
using kCura.PDD.Web.Extensions;

namespace kCura.PDD.Web.Controllers
{
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Extensions;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
    using kCura.PDB.Service.BISSummary;
    using kCura.PDD.Web.Services;

    public class FileLatencyController : BaseApiController
    {
        private readonly IRequestService requestService;

        public FileLatencyController()
        {
            this.requestService = new RequestService();
        }

        [System.Web.Http.HttpGet]
		public HttpResponseMessage GetDatabaseLatencyReport()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new FileLatencyService(sqlRepo);

			var model = PopulateFileLatencyModelSettings();

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.FileLatencyReportState] = model;

			//Get the data
			var dtResponse = GetFileLatencyGridData(svc, model);

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
			var svc = new FileLatencyService(sqlRepo);
			var model = PopulateFileLatencyModelSettings();
			model.GridConditions.StartRow = 1;
			model.GridConditions.EndRow = int.MaxValue;

			//Get the data
			model.GridConditions.EndRow = 0;
			model.GridConditions.StartRow = 0;
			String fileData = GetFileLatencyCsvData(svc, model);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fileData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("FileLatencyReport-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage FileLatencyLastRun()
		{
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new FileLatencyService(sqlRepo);
			
			//get data
			DateTime lastRunTime = svc.FileLatencyLastRun();

			//Serialize response
			var json = lastRunTime.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		[System.Web.Http.HttpPost]
		public HttpResponseMessage ResetFileLatencyLastRun()
		{
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new FileLatencyService(sqlRepo);

			//get data
			DateTime lastRunTime = svc.ResetFileLatencyLastRun();

			//Serialize response
			var json = lastRunTime.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		private string GetFileLatencyCsvData(FileLatencyService svc, FileLatencyViewModel viewModel)
		{
			var filterResult = GetFilteredFileLatencyData(svc, viewModel);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Server Name", "Database Name", "Score", "Data Read Latency", "Data Write Latency", "Log Read Latency", "Log Write Latency",
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the System View
		/// </summary>
		/// <returns></returns>
		protected FileLatencyViewModel PopulateFileLatencyModelSettings()
		{
			var model = new FileLatencyViewModel();
			var queryParams = this.requestService.GetQueryParams(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var iSortColValue = string.IsNullOrEmpty(iSortCol.Value) ? 2 : Convert.ToInt32(iSortCol.Value);
			var sSortDirValue = string.IsNullOrEmpty(sSortDir.Value) ? "desc" : sSortDir.Value;

			//ServerName =0, DatabaseName=1, Score=2, DataReadLatency=3, DataWriteLatency=4, LogReadLatency=5, LogWriteLatency=6
			//var viewMap = new FileLatency.Columns[] { FileLatency.Columns.DatabaseName, FileLatency.Columns.Score, FileLatency.Columns.DataReadLatency, FileLatency.Columns.DataWriteLatency, FileLatency.Columns.LogReadLatency, FileLatency.Columns.LogWriteLatency };
			var viewMap = System.Enum.GetValues(typeof(FileLatency.Columns)).Cast<FileLatency.Columns>().ToList();

			var sortColumn = viewMap[iSortColValue];
			String sortBy;
			sortBy = sortColumn.ToString();

			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDirValue;

			//Filter conditions
			//ServerName =0, DatabaseName=1, Score=2, DataReadLatency=3, DataWriteLatency=4, LogReadLatency=5, LogWriteLatency=6
			viewMap
				.Select((flc, i) => new { flc, search = queryParams.FirstOrDefault(k => k.Key == "sSearch_" + i).Value }).ToList()
				.ForEach(flc => model.FilterConditions[flc.flc] = flc.search);

			viewMap
				.Select((flc, i) => new { flc, op = queryParams.FirstOrDefault(k => k.Key == "sOperand_" + i).Value }).ToList()
				.Where(flc => false == String.IsNullOrEmpty(flc.op)).ToList()
				.ForEach(flc => model.FilterOperands[flc.flc] = (FilterOperand)System.Enum.Parse(typeof(FilterOperand), flc.op));

			return model;
		}

		/// <summary>
		/// Translates a recommendation view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse GetFileLatencyGridData(FileLatencyService svc, FileLatencyViewModel viewModel)
		{
			var data = GetFilteredFileLatencyData(svc, viewModel);
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

		private FilterResult GetFilteredFileLatencyData(FileLatencyService svc, FileLatencyViewModel viewModel)
		{
			var grid = svc.FileLatencies(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);

			var aaDataRel = grid.Data.Select(d => new String[] {
				d.ServerName,
				d.DatabaseName,
				d.Score.HasValue ? d.Score.Value.ToString("F2") : String.Empty,
				d.DataReadLatency.HasValue ? d.DataReadLatency.ToString() : String.Empty,
				d.DataWriteLatency.HasValue ? d.DataWriteLatency.ToString() : String.Empty,
				d.LogReadLatency.HasValue ? d.LogReadLatency.ToString() : String.Empty,
				d.LogWriteLatency.HasValue ? d.LogWriteLatency.ToString() : String.Empty
			}).ToArray();

			return new FilterResult() { Data = aaDataRel, TotalRecordCount = grid.Count };
		}
	}
}