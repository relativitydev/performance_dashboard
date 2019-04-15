namespace kCura.PDD.Web.Controllers
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Web.SessionState;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.ViewModels;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Extensions;
	using kCura.PDD.Web.Models;
	using kCura.PDD.Web.Services;

	public class RecoveryObjectivesReportController : RecoverabilityIntegrityController
    {
        private readonly IRequestService requestService;

        public RecoveryObjectivesReportController()
        {
            // TODO - Get rid of inheritence
            this.requestService = new RequestService();
        }

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Databases()
		{
			//Initialize service and model properties
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var reportRepository = new LegacyRecoverabilityIntegrityReportRepository(connectionFactory);
			var svc = new BackupDbccService(sqlRepo, reportRepository);
			var model = PopulateModelSettings();
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.RecoveryObjectivesState] = model;

			//Get the data
			var dtResponse = FetchGridData(svc, model, qualityIndicatorService);

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
			var reportRepository = new LegacyRecoverabilityIntegrityReportRepository(connectionFactory);
			var svc = new BackupDbccService(sqlRepo, reportRepository);
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

			var model = PopulateModelSettings();
			model.GridConditions.StartRow = 1;
            model.GridConditions.EndRow = int.MaxValue;
			//Get the data
			var fetchedData = FetchFileData(svc, model, qualityIndicatorService);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fetchedData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("RecoveryObjectives-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

        private string FetchFileData(BackupDbccService svc, RecoveryObjectivesViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
		{
			var filterResult = FilterData(svc, viewModel, true, qualityIndicatorService);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Server", "Database Name", "RPO Score", "RTO Score", "Potential Data Loss (Minutes)", "Estimated Time to Recover (Hours)"
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

        private FilterResult FilterData(BackupDbccService svc, RecoveryObjectivesViewModel viewModel, bool isDownload, IQualityIndicatorService qualityIndicatorService)
		{
			var grid = svc.RecoveryObjectivesSummary(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var aaData = grid.Data.Select(d => new string[] {
               $"{d.ServerName} ({d.ServerId})",
				d.DatabaseName,
                isDownload ?
                    d.RPOScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.RPOScore, false)}'>{d.RPOScore}</span>",
                isDownload ?
                    d.RTOScore.ToString()
					:$"<span class='{qualityIndicatorService.GetCssClassForScore(d.RTOScore, false)}'>{d.RTOScore}</span>",
                d.PotentialDataLossMinutes.ToString(),
                d.EstimatedTimeToRecoverHours.ToString()
			}).ToArray();
			return new kCura.PDD.Web.Controllers.FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}

	    /// <summary>
	    /// Pulls query parameters from the request and converts them to grid and filter conditions for the Backup/DBCC View
	    /// </summary>
	    /// <returns></returns>
	    private RecoveryObjectivesViewModel PopulateModelSettings()
	    {
		    var queryParams = this.requestService.GetQueryParamsDecoded(Request);
		    return BackupDbccService.PopulateRecoveryObjectivesModelSettings(queryParams);
	    }
		
	    /// <summary>
		/// Translates a Backup/DBCC view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
        private DataTableResponse FetchGridData(BackupDbccService svc, RecoveryObjectivesViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
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