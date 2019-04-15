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
using System.Web.Http;
using System.Web.SessionState;
using kCura.PDD.Web.Extensions;

namespace kCura.PDD.Web.Controllers
{
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
    using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDD.Web.Filters;
	using kCura.PDD.Web.Services;

    public class EnvironmentCheckController : BaseApiController
    {
        private readonly IRequestService requestService;

        public EnvironmentCheckController()
        {
            this.requestService = new RequestService();
        }

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Recommendations()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new EnvironmentCheckService(sqlRepo);
			var model = PopulateRecommendationModelSettings();

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.UptimeState] = model;

			//Get the data
			var dtResponse = FetchRecommendationCheckGridData(svc, model);

			UpdateRecommendationData(dtResponse.aaData);

			//Serialize response
			var json = dtResponse.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage ServerInfo()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new EnvironmentCheckService(sqlRepo);
			var model = PopulateServerModelSettings();

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.UptimeState] = model;

			//Get the data
			var dtResponse = FetchServerCheckGridData(svc, model);

			//Serialize response
			var json = dtResponse.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage DatabaseInfo()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new EnvironmentCheckService(sqlRepo);
			var model = PopulateDatabaseModelSettings();

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.UptimeState] = model;

			//Get the data
			var dtResponse = FetchDatabaseCheckGridData(svc, model);

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
			var svc = new EnvironmentCheckService(sqlRepo);
			var model = PopulateModelSettings();
			model.GridConditions.StartRow = 1;
			model.GridConditions.EndRow = int.MaxValue;

			//Get the data
			String fileData;
			if (model.ReportType == EnvironmentCheckReportType.Recommendations)
			{
				model = PopulateRecommendationModelSettings();
				model.GridConditions.EndRow = 0;
				model.GridConditions.StartRow = 0;
				fileData = FetchFileRecommendationData(svc, model);
			}
			else if (model.ReportType == EnvironmentCheckReportType.Server)
			{
				model = PopulateServerModelSettings();
				model.GridConditions.EndRow = 0;
				model.GridConditions.StartRow = 0;
				fileData = FetchFileServerData(svc, model);
			}
			else if (model.ReportType == EnvironmentCheckReportType.Database)
			{
				model = PopulateDatabaseModelSettings();
				model.GridConditions.EndRow = 0;
				model.GridConditions.StartRow = 0;
				fileData = FetchFileDatabaseData(svc, model);
			}
			else
				throw new Exception("Invalid report type.");

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fileData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("EnvironmentCheck-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage TuningForkLastRun(Boolean config)
		{
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new EnvironmentCheckService(sqlRepo);

			//get data
			DateTime lastRunTime;
			if (config)
				lastRunTime = svc.TuningForkLastRun(ProcessControlId.EnvironmentCheckRelativityConfig, ProcessControlId.EnvironmentCheckSqlConfig);
			else
				lastRunTime = svc.TuningForkLastRun(ProcessControlId.EnvironmentCheckServerInfo);

			//Serialize response
			var json = lastRunTime.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		[System.Web.Http.HttpPost]
		[ValidateAjaxAntiForgeryToken]
		public HttpResponseMessage ResetTuningForkLastRun(Boolean config)
		{
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new EnvironmentCheckService(sqlRepo);

			//get data
			DateTime lastRunTime;
			if (config)
				lastRunTime = svc.ResetTuningForkLastRun(ProcessControlId.EnvironmentCheckRelativityConfig, ProcessControlId.EnvironmentCheckSqlConfig);
			else
				lastRunTime = svc.ResetTuningForkLastRun(ProcessControlId.EnvironmentCheckServerInfo);

			//Serialize response
			var json = lastRunTime.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

		protected EnvironmentCheckViewModel PopulateModelSettings()
		{
			var model = new EnvironmentCheckViewModel();
			var queryParams = this.requestService.GetQueryParams(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			if (queryParams.Any(x => x.Key == "gridType"))
			{
				var reportType = queryParams.FirstOrDefault(x => x.Key == "gridType");
				if (false == String.IsNullOrEmpty(reportType.Value))
					model.ReportType = (EnvironmentCheckReportType)System.Enum.Parse(typeof(EnvironmentCheckReportType), reportType.Value, true);
				else
					throw new Exception("grid type required.");
			}
			else
			{
				throw new Exception("grid type required.");
			}


			return model;
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the System View
		/// </summary>
		/// <returns></returns>
		protected EnvironmentCheckViewModel PopulateRecommendationModelSettings()
		{
			var model = new EnvironmentCheckViewModel();
			var queryParams = this.requestService.GetQueryParams(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var iSortColValue = string.IsNullOrEmpty(iSortCol.Value) ? "0" : iSortCol.Value;
			var sSortDirValue = string.IsNullOrEmpty(sSortDir.Value) ? "desc" : sSortDir.Value;

			var sortColumn = (EnvironmentCheckRecommendationColumns?)System.Enum.Parse(typeof(EnvironmentCheckRecommendationColumns), iSortColValue);
			String sortBy;
			sortBy = sortColumn.ToString();

			model.GridConditions.SortIndex = iSortColValue;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDirValue;

			//Filter conditions
			// Status=0, Scope=1, Section=2, Name=3, Description=4, Value=5, Recommendation=6
			var statusFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var scopeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var sectionFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var nameFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var desciptionFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var valueFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var recommendationFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");

			model.RecommendationFilterConditions = new EnvironmentCheckRecommendationFilterConditions()
			{
				Description = desciptionFilter.Value,
				Name = nameFilter.Value,
				Recommendation = recommendationFilter.Value,
				Scope = scopeFilter.Value,
				Section = sectionFilter.Value,
				Status = statusFilter.Value,
				Value = valueFilter.Value,
			};

			return model;
		}


		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Server View
		/// </summary>
		/// <returns></returns>
		protected EnvironmentCheckViewModel PopulateServerModelSettings()
		{
			var model = new EnvironmentCheckViewModel();
			var queryParams = this.requestService.GetQueryParams(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (EnvironmentCheckServerColumns?)System.Enum.Parse(typeof(EnvironmentCheckServerColumns), iSortCol.Value);
			var sortBy = sortColumn.HasValue
				? sortColumn.Value.ToString()
				: null;

			model.GridConditions.SortIndex = iSortCol.Value;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDir.Value;

			//Filter conditions
			var serverNameFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var osNameFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var osVersionFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var logicalProcessorsFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var hyperthreadedFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");

			model.ServerFilterConditions = new EnvironmentCheckServerFilterConditions()
			{
				ServerName = serverNameFilter.Value,
				OSName = osNameFilter.Value,
				OSVersion = osVersionFilter.Value,
				LogicalProcessors = logicalProcessorsFilter.Value,
				Hyperthreaded = hyperthreadedFilter.Value
			};

			//Filter operands
			var logicalProcessorsOperand = queryParams.FirstOrDefault(k => k.Key == "sOperand_3");
			model.ServerFilterOperands = new EnvironmentCheckServerFilterOperands()
			{
				LogicalProcessors = false == String.IsNullOrEmpty(logicalProcessorsOperand.Value) ? (FilterOperand)System.Enum.Parse(typeof(FilterOperand), logicalProcessorsOperand.Value) : FilterOperand.Equals,
			};

			return model;
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Database View
		/// </summary>
		/// <returns></returns>
		protected EnvironmentCheckViewModel PopulateDatabaseModelSettings()
		{
			var model = new EnvironmentCheckViewModel();
			var queryParams = this.requestService.GetQueryParams(Request);

			//Grid conditions
			model.GridConditions = PopulateCommonGridConditions(queryParams);

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (EnvironmentCheckDatabaseColumns?)System.Enum.Parse(typeof(EnvironmentCheckDatabaseColumns), iSortCol.Value);
			var sortBy = sortColumn.HasValue
				? sortColumn.Value.ToString()
				: null;

			model.GridConditions.SortIndex = iSortCol.Value;
			model.GridConditions.SortColumn = sortBy;
			model.GridConditions.SortDirection = sSortDir.Value;

			//Filter conditions
			var serverNameFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var sqlVersionFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var adhocWorkloadFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var maxServerMemoryFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var maxDegreeOfParallelismFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var tempDBDataFilesFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var lastSQLRestartFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");

			model.DatabaseFilterConditions = new EnvironmentCheckDatabaseFilterConditions()
			{
				ServerName = serverNameFilter.Value,
				SQLVersion = sqlVersionFilter.Value,
				AdHocWorkload = adhocWorkloadFilter.Value,
				MaxServerMemory = maxServerMemoryFilter.Value,
				MaxDegreeOfParallelism = maxDegreeOfParallelismFilter.Value,
				tempDBDataFiles = tempDBDataFilesFilter.Value,
				LastSqlRestart = lastSQLRestartFilter.Value
			};

			//Filter operands
			var adhocWorkloadOperand = queryParams.FirstOrDefault(k => k.Key == "sOperand_2");
			var maxServerMemoryOperand = queryParams.FirstOrDefault(k => k.Key == "sOperand_3");
			var maxDegreeOfParallelismOperand = queryParams.FirstOrDefault(k => k.Key == "sOperand_4");
			var tempDBDataFilesOperand = queryParams.FirstOrDefault(k => k.Key == "sOperand_5");
			var lastSQLRestartOperand = queryParams.FirstOrDefault(k => k.Key == "sOperand_6");

			model.DatabaseFilterOperands = new EnvironmentCheckDatabaseFilterOperands()
			{
				AdHocWorkload = false == String.IsNullOrEmpty(adhocWorkloadOperand.Value) ? (FilterOperand)System.Enum.Parse(typeof(FilterOperand), adhocWorkloadOperand.Value) : FilterOperand.Equals,
				MaxServerMemory = false == String.IsNullOrEmpty(maxServerMemoryOperand.Value) ? (FilterOperand)System.Enum.Parse(typeof(FilterOperand), maxServerMemoryOperand.Value) : FilterOperand.Equals,
				MaxDegreeOfParallelism = false == String.IsNullOrEmpty(maxDegreeOfParallelismOperand.Value) ? (FilterOperand)System.Enum.Parse(typeof(FilterOperand), maxDegreeOfParallelismOperand.Value) : FilterOperand.Equals,
				TempDBDataFiles = false == String.IsNullOrEmpty(tempDBDataFilesOperand.Value) ? (FilterOperand)System.Enum.Parse(typeof(FilterOperand), tempDBDataFilesOperand.Value) : FilterOperand.Equals,
				LastSQLRestart = false == String.IsNullOrEmpty(lastSQLRestartOperand.Value) ? (FilterOperand)System.Enum.Parse(typeof(FilterOperand), lastSQLRestartOperand.Value) : FilterOperand.Equals,
			};

			return model;
		}

		private string FetchFileRecommendationData(EnvironmentCheckService svc, EnvironmentCheckViewModel viewModel)
		{
			var filterResult = FilterRecommendationData(svc, viewModel);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Status", "Scope", "Section", "Name", "Description", "Value", "Recommendation",
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		private string FetchFileServerData(EnvironmentCheckService svc, EnvironmentCheckViewModel viewModel)
		{
			var grid = svc.ServerDetails(viewModel.GridConditions, viewModel.ServerFilterConditions, viewModel.ServerFilterOperands);

			var aaDataRel = grid.Data.Select(d => new String[] {
				d.ServerName, d.OSName, d.OSVersion, d.LogicalProcessors.ToString(), d.Hyperthreaded.ToString(),
			}).ToArray();

			var filterResult = new FilterResult() { Data = aaDataRel, TotalRecordCount = grid.Count };

			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Server Name", "Operating System Name", "Operating System Version",  "Logical Processors", "Hyperthreaded",
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		private string FetchFileDatabaseData(EnvironmentCheckService svc, EnvironmentCheckViewModel viewModel)
		{
			var grid = svc.DatabaseDetails(viewModel.GridConditions, viewModel.DatabaseFilterConditions, viewModel.DatabaseFilterOperands);

			var aaDataRel = grid.Data.Select(d => new String[] {
				d.ServerName, d.SQLVersion, d.AdHocWorkLoad.ToString(), d.MaxServerMemory.ToString("f2"),
				d.MaxDegreeOfParallelism.ToString(), d.TempDBDataFiles.ToString(), d.LastSQLRestart.ToString("MMM dd, yyyy")
			}).ToArray();

			var filterResult = new FilterResult() { Data = aaDataRel, TotalRecordCount = grid.Count };

			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Server Name", "SQL Version", "Ad Hoc Workload", "Max Server Memory (GB)", "Max Degree of Parallelism", "TempDB Data Files", "Last SQL Restart",
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		/// <summary>
		/// Translates a recommendation view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchRecommendationCheckGridData(EnvironmentCheckService svc, EnvironmentCheckViewModel viewModel)
		{
			var data = FilterRecommendationData(svc, viewModel);
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




		private DataTableResponse FetchServerCheckGridData(EnvironmentCheckService svc, EnvironmentCheckViewModel viewModel)
		{
			var grid = svc.ServerDetails(viewModel.GridConditions, viewModel.ServerFilterConditions, viewModel.ServerFilterOperands);

			var aaDataRel = grid.Data.Select(d => new String[] {
				d.ServerName, d.OSName, d.OSVersion, d.LogicalProcessors.ToString(), d.Hyperthreaded.ToString(),
			}).ToArray();

			var data = new FilterResult() { Data = aaDataRel, TotalRecordCount = grid.Count };

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

		private DataTableResponse FetchDatabaseCheckGridData(EnvironmentCheckService svc, EnvironmentCheckViewModel viewModel)
		{
			var grid = svc.DatabaseDetails(viewModel.GridConditions, viewModel.DatabaseFilterConditions, viewModel.DatabaseFilterOperands);

			var aaDataRel = grid.Data.Select(d => new String[] {
				d.ServerName, d.SQLVersion, d.AdHocWorkLoad.ToString(), d.MaxServerMemory.ToString("f2"),
				d.MaxDegreeOfParallelism.ToString(), d.TempDBDataFiles.ToString(), d.LastSQLRestart.ToString("MMM dd, yyyy")
			}).ToArray();

			var data = new FilterResult() { Data = aaDataRel, TotalRecordCount = grid.Count };

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

		private FilterResult FilterRecommendationData(EnvironmentCheckService svc, EnvironmentCheckViewModel viewModel)
		{
			var grid = svc.Recommendations(viewModel.GridConditions, viewModel.RecommendationFilterConditions);

			var aaDataRel = grid.Data.Select(d => new String[] {
				 d.Status, d.Scope, d.Section, d.Name, d.Description, d.Value, d.Recommendation
			}).ToArray();

			return new FilterResult() { Data = aaDataRel, TotalRecordCount = grid.Count };
		}

		private void UpdateRecommendationData(String[][] filteredData)
		{
			var i = 0;
			foreach (var row in filteredData)
			{
				row[(int)EnvironmentCheckRecommendationColumns.Description] =
					String.Format("<span class=\"moreless moreless-group-{0}\">{1}</span>",
					i,
					row[(int)EnvironmentCheckRecommendationColumns.Description]);

				row[(int)EnvironmentCheckRecommendationColumns.Recommendation] =
					String.Format("<span class=\"moreless moreless-group-{0}\">{1}</span>",
					i,
					row[(int)EnvironmentCheckRecommendationColumns.Recommendation]);

				i++;
			}
		}

		private String GetSysValue(Int32? value, Int32? valueInUse, Boolean includeValueInUse)
		{
			string restartMsg = "Value in use is different than the value configured. Restart SQL Server for changes to take effect.";

			if (includeValueInUse)
			{
				if (value.HasValue && valueInUse.HasValue && value.Value == valueInUse.Value)
					return value.Value.ToString();
				else if (value.HasValue && valueInUse.HasValue && value.Value != valueInUse.Value)
					return String.Format("<span title='{2}'>{0} ({1})</span>", value.Value, valueInUse.Value, restartMsg);
				else if (value.HasValue && false == valueInUse.HasValue)
					return String.Format("<span title='{1}'>{0} (<i>null</i>)</span>", value.Value, restartMsg);
				else if (false == value.HasValue && valueInUse.HasValue)
					return String.Format("<span title='{1}'><i>null</i> ({0})</span>", valueInUse.Value, restartMsg);
				else
					return "<span><i>null</i></span>";
			}
			else
			{
				return value.HasValue ? value.Value.ToString() : "";
			}
		}


	}
}