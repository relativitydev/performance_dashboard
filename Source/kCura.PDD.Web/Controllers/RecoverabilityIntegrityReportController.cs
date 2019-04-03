namespace kCura.PDD.Web.Controllers
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Web.SessionState;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.ViewModels;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Extensions;
	using kCura.PDD.Web.Models;
	using kCura.PDD.Web.Services;

	public class RecoverabilityIntegrityReportController : RecoverabilityIntegrityController
	{
		private readonly IRequestService requestService;

		public RecoverabilityIntegrityReportController()
		{
			// TODO - Get rid of inheritence
			this.requestService = new RequestService();
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Hours()
		{
			//Initialize service and model properties	
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var svc = this.GetBackupDbccService();
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));
			
			var model = PopulateHoursModelSettings();

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.RecoverabilityIntegrityState] = model;

			//Get the data
			// Pass QualityService
			var dtResponse = FetchHoursGridData(svc, model, qualityIndicatorService);

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
			var svc = this.GetBackupDbccService();
			var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));
			
			var model = PopulateHoursModelSettings();
			model.GridConditions.StartRow = 1;
			model.GridConditions.EndRow = int.MaxValue;

			//Get the data
			// Pass QualityService
			var fetchedData = FetchFileData(svc, model, qualityIndicatorService);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fetchedData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("RecoverabilityIntegrity-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		private BackupDbccService GetBackupDbccService()
		{
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var toggleProvider = new PdbSqlToggleProvider(connectionFactory);
			var reportRepositoryFactory = new RecoverabilityIntegrityReportReaderFactory(toggleProvider, connectionFactory);
			var reportRepository = reportRepositoryFactory.Get();

			return new BackupDbccService(sqlRepo, reportRepository);
		}

		private string FetchFileData(BackupDbccService svc, RecoverabilityIntegrityViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
		{
			var filterResult = FilterData(svc, viewModel, true, qualityIndicatorService);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Hour", "Overall Score", "Backup Frequency Score", "Backup Coverage Score",
					"DBCC Frequency Score", "DBCC Coverage Score", "RPO Score", "RTO Score"
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		private FilterResult FilterData(BackupDbccService svc, RecoverabilityIntegrityViewModel viewModel, bool isDownload, IQualityIndicatorService qualityIndicatorService)
		{
			var grid = svc.RecoverabilityIntegritySummary(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var aaData = grid.Data.Select(d => new string[] {
				d.FriendlySummaryDayHour,
				isDownload ?
					d.RecoverabilityIntegrityScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.RecoverabilityIntegrityScore, false)}'>{d.RecoverabilityIntegrityScore}</span>",
				isDownload ?
					d.BackupFrequencyScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.BackupFrequencyScore, false)}'>{d.BackupFrequencyScore}</span>",
				isDownload ?
					d.BackupCoverageScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.BackupCoverageScore, false)}'>{d.BackupCoverageScore}</span>",
				isDownload ?
					d.DbccFrequencyScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.DbccFrequencyScore, false)}'>{d.DbccFrequencyScore}</span>",
				isDownload ?
					d.DbccCoverageScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.DbccCoverageScore, false)}'>{d.DbccCoverageScore}</span>",
				isDownload ?
					d.RPOScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.RPOScore, false)}' title='{(d.RPOScore < 100 ? d.PotentialDataLossMinutes + " minute(s) of potential data loss on "+ d.WorstRPODatabase : String.Empty)}'>{d.RPOScore}</span>",
				isDownload ?
					d.RTOScore.ToString()
					: $"<span class='{qualityIndicatorService.GetCssClassForScore(d.RTOScore, false)}' title='{(d.RTOScore < 100 ? "Estimated " + d.EstimatedTimeToRecoverHours + " hours(s) to restore "+ d.WorstRTODatabase : String.Empty)}'>{d.RTOScore}</span>"
			}).ToArray();
			return new kCura.PDD.Web.Controllers.FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Backup/DBCC View
		/// </summary>
		/// <returns></returns>
		private RecoverabilityIntegrityViewModel PopulateHoursModelSettings()
		{
			var queryParams = this.requestService.GetQueryParamsDecoded(Request);
			return BackupDbccService.PopulateRecoverabilityIntegrityHoursModelSettings(queryParams);
		}

		
		
		/// <summary>
		/// Translates a Backup/DBCC view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchHoursGridData(BackupDbccService svc, RecoverabilityIntegrityViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
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