using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using kCura.PDD.Web.Constants;
using kCura.PDD.Web.Models;
using kCura.PDD.Web.Models.BISSummary;
using kCura.PDD.Web.Services;
using kCura.PDD.Web.Enum;
using kCura.PDD.Web.Extensions;
using kCura.PDD.Web.Controllers;

namespace kCura.PDD.Web
{
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
    using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.ViewColumns;
	using kCura.PDB.Core.Models.BISSummary.ViewModels;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Data.Services;
    using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;

	public class BackupDbccReportController : RecoverabilityIntegrityController
	{
		public string DatabaseInfoFormat = @"<div style=""display: inline-block;"" title=""{0}"">{1}</div>";

	    private readonly IRequestService requestService;

	    public BackupDbccReportController()
	    {
            // TODO - Get rid of inheritence
            this.requestService = new RequestService();
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

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Gaps()
		{
			//Initialize service and model properties
			var svc = this.GetBackupDbccService();
			var model = PopulateGapModelSettings();

			var session = (HttpSessionState)GetSession();
			session[DataTableSessionConstants.BackupDbccState] = model;

			//Get the data
			var dtResponse = FetchGapGridData(svc, model);

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
			var svc = this.GetBackupDbccService();
			var model = PopulateGapModelSettings();
			model.GridConditions.StartRow = 1;
            model.GridConditions.EndRow = int.MaxValue;
			//Get the data
			var fetchedData = FetchFileData(svc, model);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fetchedData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("BackupDBCC-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		private string FetchFileData(BackupDbccService svc, BackupDbccViewModel viewModel)
		{
			var filterResult = FilterData(svc, viewModel, true);
			using (var sw = new System.IO.StringWriter())
			{
				var heaaderArr = new string[]
				{
					"Server", "Database", "Activity Type", "Last Activity",
					"Resolution Date", "Gap Size"
				};
				sw.WriteLine(string.Join(",", heaaderArr));
				foreach (var row in filterResult.Data)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		private FilterResult FilterData(BackupDbccService svc, BackupDbccViewModel viewModel, bool IsDownload)
		{
			var grid = svc.BackupDbccHistory(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);
			var aaData = grid.Data.Select(d => new string[] {
				String.Format("{0} ({1})", d.Server, d.ServerId),
				IsDownload
					? d.Database
					: String.Format(DatabaseInfoFormat, d.Workspace, d.Database),
				d.FriendlyIsBackup,
				d.FriendlyLastActivityDate,
				d.FriendlyGapResolutionDate,
				IsDownload
					? d.GapSize.ToString()
					: string.Format(@"<b class=""{0}"">{1}</b>",
							d.GapSize <= 9
								? "grid-item-healthy"
								:	d.GapResolutionDate.HasValue
									? "grid-item-warning"
									: "grid-item-critical",
							d.GapSize.ToString())
			}).ToArray();
			return new kCura.PDD.Web.Controllers.FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Backup/DBCC View
		/// </summary>
		/// <returns></returns>
		private BackupDbccViewModel PopulateGapModelSettings()
		{
			var queryParams = this.requestService.GetQueryParamsDecoded(Request);
			return BackupDbccService.PopulateGapModelSettings(queryParams);
		}

		/// <summary>
		/// Translates a Backup/DBCC view into grid data
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private DataTableResponse FetchGapGridData(BackupDbccService svc, BackupDbccViewModel viewModel)
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
	}
}