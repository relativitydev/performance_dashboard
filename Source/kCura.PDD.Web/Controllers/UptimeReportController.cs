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
    using kCura.PDB.Service;
    using kCura.PDB.Service.BISSummary;
    using kCura.PDB.Service.Services;
    using kCura.PDD.Web.Constants;
    using kCura.PDD.Web.Enum;
    using kCura.PDD.Web.Extensions;
    using kCura.PDD.Web.Models;
    using kCura.PDD.Web.Models.BISSummary;
    using kCura.PDD.Web.Services;

    public class UptimeReportController : UptimeController
    {
        private readonly IRequestService requestService;

        public UptimeReportController()
        {
            // TODO - Get rid of inheritence
            this.requestService = new RequestService();
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Hours()
        {
            //Initialize service and model properties
            var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
            var sqlRepo = new SqlServerRepository(connectionFactory);
            var svc = new UptimeService(sqlRepo);
            var qualityIndicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));

            var model = PopulateServerModelSettings();

            var session = (HttpSessionState)GetSession();
            session[DataTableSessionConstants.UptimeState] = model;

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
            var svc = new UptimeService(sqlRepo);
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
            message.Content.Headers.ContentDisposition.FileName = string.Format("UptimeReport-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            return message;
        }

        private string FetchFileData(UptimeService svc, UptimeReportViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
        {
            var filterResult = FilterData(svc, viewModel, true, qualityIndicatorService);
            using (var sw = new System.IO.StringWriter())
            {
                var heaaderArr = new string[]
                {
                    "Hour", "Score", "Status", "Uptime"
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
        protected UptimeReportViewModel PopulateServerModelSettings()
        {
            var model = new UptimeReportViewModel();
            var queryParams = this.requestService.GetQueryParamsDecoded(Request);

            //Grid conditions
            model.GridConditions = PopulateCommonGridConditions(queryParams);

            var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
            var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");

            var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
                ? null
                : (UptimeViewColumns?)System.Enum.Parse(typeof(UptimeViewColumns), iSortCol.Value);
            var sortBy = sortColumn?.ToString();
            model.GridConditions.SortIndex = iSortCol.Value;
            model.GridConditions.SortColumn = sortBy;
            model.GridConditions.SortDirection = sSortDir.Value;

            //Filter conditions
            var hourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
            var scoreFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
            var statusFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
            var uptimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");

            int score;
            double uptime;
            DateTime hour;
            model.FilterConditions = new UptimeViewFilterConditions()
            {
                SummaryDayHour = DateTime.TryParse(HttpUtility.UrlDecode(hourFilter.Value), out hour)
                    ? hour
                    : (DateTime?)null,
                Score = int.TryParse(scoreFilter.Value, out score)
                    ? score
                    : (int?)null,
                Status = string.IsNullOrEmpty(statusFilter.Value)
                    ? null
                    : statusFilter.Value,
                Uptime = double.TryParse(uptimeFilter.Value, out uptime)
                    ? uptime
                    : (double?)null
            };

            //Filter operands
            System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_1").Value, out model.FilterOperands.Score);
            System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_3").Value, out model.FilterOperands.Uptime);

            return model;
        }

        /// <summary>
        /// Translates a Server view into grid data
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private DataTableResponse FetchServerGridData(UptimeService svc, UptimeReportViewModel viewModel, IQualityIndicatorService qualityIndicatorService)
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

        private FilterResult FilterData(UptimeService svc, UptimeReportViewModel viewModel, bool isDownload, IQualityIndicatorService qualityIndicatorService)
        {
            var grid = svc.UptimeHours(viewModel.GridConditions, viewModel.FilterConditions, viewModel.FilterOperands);

            var aaData = grid.Data.Select(d => new string[]
            {
                $"{d.SummaryDayHour.ToShortDateString()} {d.SummaryDayHour.ToString("t")}",
                isDownload
                    ? $"{d.Score} {(d.AffectedByMaintenanceWindow ? "(Affected By Maintenance Window)" : string.Empty)}"
                    : $"<span class='{qualityIndicatorService.GetCssClassForScore(d.Score, false)}'>{d.Score} {(d.AffectedByMaintenanceWindow ? "(Affected By Maintenance Window)" : string.Empty)}</span>",
                d.Status.ToString(),
                d.Uptime.ToString("N2")
            }).ToArray();

            return new FilterResult() { Data = aaData, TotalRecordCount = grid.Count };
        }
    }
}