using System;
using System.Web.Script.Serialization;
using kCura.PDD.Web.Constants;
using kCura.PDD.Web.Models.BISSummary;
using kCura.PDB.Service;
using kCura.PDB.Core.Enumerations;
using kCura.PDB.Core.Interfaces.Services;

namespace kCura.PDD.Web
{
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Services;

	public partial class Uptime : PageBase
	{
		#region Private Members
		private readonly UptimeService _uptime;
		private readonly IQualityIndicatorService _indicatorService;
		#endregion

		public Uptime()
			: base(lookingGlassDependency: true)
		{
			_uptime = new UptimeService(this.SqlRepo);
			_indicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(this.SqlRepo.ConfigurationRepository));
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.UptimeState] as UptimeReportViewModel;
			var model = state ?? new UptimeReportViewModel();

			Initialize(model);

			var scores = _uptime.GetOverallScores();

			QoSNavButton.HRef = QosNavigationUrl;

			QuarterlyScore.Attributes["class"] = _indicatorService.GetCssClassForScore(scores.QuarterlyUptimeScore, true);
			QuarterlyScore.InnerText = _indicatorService.GetIndicatorForScore(scores.QuarterlyUptimeScore) != QualityIndicator.None ? 
													scores.QuarterlyUptimeScore.ToString() :
													"N/A";
			
			QuarterlyScore.Attributes["href"] = QosNavigationUrl;


			DateFormatString.Value = DateFormat;
			TimeFormatString.Value = TimeFormat;

			var timezoneOffset = RequestService.GetTimezoneOffset(this.Request);
			startDate.Value = model.GridConditions.StartDate.HasValue
				? model.GridConditions.StartDate.Value.AddMinutes(timezoneOffset).ToShortDateString()
				: GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).AddMinutes(timezoneOffset).ToShortDateString();
			endDate.Value = model.GridConditions.EndDate.HasValue
				? model.GridConditions.EndDate.Value.AddMinutes(timezoneOffset).ToShortDateString()
				: DateTime.UtcNow.AddMinutes(timezoneOffset).ToShortDateString();
		}

		private void Initialize(UptimeReportViewModel model)
		{
			var json = new JavaScriptSerializer().Serialize(model);
			VarscatState.Value = json;

			TimezoneOffset.Value = RequestService.GetTimezoneOffset(this.Request).ToString();
			SampleStart.Value = base.GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).ToString("s");
		}
	}
}