namespace kCura.PDD.Web
{
	using System;
	using System.Linq;
	using System.Web.Script.Serialization;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Services;

	public partial class SystemLoadServers : PageBase
	{
		#region Private Members
		private readonly SystemLoadService _systemLoad;
		private readonly IQualityIndicatorService _indicatorService;
		#endregion

		public SystemLoadServers()
			: base(true)
		{
			_systemLoad = new SystemLoadService(this.SqlRepo);
			_indicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(this.SqlRepo.ConfigurationRepository));
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.SystemLoadServerState] as SystemLoadServerViewModel;
			var serverParam = Request.Params["Server"] ?? string.Empty;

			//If any query string parameters differ from previous model state filters, build a new model
			if (state != null && state.FilterConditions.Server != serverParam)
				state = null;
			var model = state ?? new SystemLoadServerViewModel();

			Initialize(model, serverParam);

			var scores = _systemLoad.GetOverallScores();

			QoSNavButton.HRef = QosNavigationUrl;
			BtnWaits.HRef = GetPageUrl(Names.Tab.QualityOfService, "Waits");
			BtnFileLatency.HRef = GetPageUrl(Names.Tab.QualityOfService, "FileLatencyReport");

			QuarterlyScore.Attributes["class"] = _indicatorService.GetCssClassForScore(scores.QuarterlySystemLoadScore, true);
			QuarterlyScore.InnerText = _indicatorService.GetIndicatorForScore(scores.QuarterlySystemLoadScore) != PDB.Core.Enumerations.QualityIndicator.None ?
										scores.QuarterlySystemLoadScore.ToString() : "N/A";
			QuarterlyScore.Attributes["href"] = QosNavigationUrl;

			var servers = _systemLoad.ListAllServers().Where(x => x.ArtifactId > 0).ToList();
			pageServerSelect.DataValueField = "ArtifactId";
			pageServerSelect.DataTextField = "Name";
			pageServerSelect.DataSource = servers;
			pageServerSelect.DataBind();
			if (!string.IsNullOrEmpty(model.FilterConditions.Server))
			{
				var selectedServer = pageServerSelect.Items.FindByValue(model.FilterConditions.Server);
				if (selectedServer != null)
					selectedServer.Selected = true;
			}

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

		private void Initialize(SystemLoadServerViewModel model, string serverParam)
		{
			if (!string.IsNullOrEmpty(serverParam))
				model.FilterConditions.Server = serverParam;

			var json = new JavaScriptSerializer().Serialize(model);
			VarscatState.Value = json;

			TimezoneOffset.Value = RequestService.GetTimezoneOffset(this.Request).ToString();
			SampleStart.Value = base.GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).ToString("s");
		}
	}
}