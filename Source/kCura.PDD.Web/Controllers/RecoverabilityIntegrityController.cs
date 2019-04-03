using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using kCura.PDD.Web.Models.BISSummary;
using kCura.PDD.Web.Services;
using kCura.PDD.Web.Filters;

namespace kCura.PDD.Web
{
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.BISSummary;

	[AuthenticateUserAttribute]
	public class RecoverabilityIntegrityController : ApiController
	{
	    private readonly IRequestService requestService;

	    public RecoverabilityIntegrityController()
	    {
            // TODO - Get rid of inheritence
            this.requestService = new RequestService();
        }

		protected HttpSessionState GetSession()
		{
			return HttpContext.Current.Session;
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Scores()
		{
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			var svc = new BestInServiceReportingService(sqlRepo);
			
			var queryParams =  this.requestService.GetQueryParamsDecoded(Request);
			var timezoneOffset = QueryParamsParsingService.TimeZoneOffset(queryParams);
			var scores = svc.GetScoreHistory(queryParams);

			var hours = scores.OrderBy(x => x.SummaryDayHour)
				.Select(x => x.SummaryDayHour)
				.Distinct();
			var model = new ScoreChartViewModel
			{
				Labels = hours.Select(x => x.AddMinutes(timezoneOffset).ToString("g"))
			};

			var dataSets = (from score in scores
											group score by score.ServerArtifactId into serverScores
											orderby serverScores.Key
											select new ScoreChartDataSet
											{
												label = "Quarterly Recoverability/Integrity Score",
												data = from hour in hours select (int?)
													(serverScores.FirstOrDefault(x => x.SummaryDayHour.Equals(hour))
													?? new ScoreChartModel()).BackupDbccScore,
												sample = from hour in hours
																 select (bool?)false
											}).ToArray();

			int max = 0, min = 100;
			for (var i = 0; i < dataSets.Count(); i++)
			{
				var localMin = dataSets[i].data.Min(x => x.GetValueOrDefault(100));
				if (localMin < min)
					min = localMin;
				var localMax = dataSets[i].data.Max(x => x.GetValueOrDefault(100));
				if (localMax > max)
					max = localMax;

				var color = ChartColorExtensions.ChartColorHex(i);
				dataSets[i].pointColor = color;
				dataSets[i].strokeColor = color;
			}
			if (min > max)
				min = max;

			model.DataSets = dataSets.Take(1);
			model.MinValue = min;
			model.MaxValue = max;

			//Serialize response
			var json = model.ToJson();
			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			return response;
		}
	}
}