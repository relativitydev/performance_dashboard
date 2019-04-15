namespace kCura.PDD.Web
{
	using System;
	using System.Web.Script.Serialization;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDB.Core.Constants;
	using kCura.PDD.Web.Services;

	public partial class EnvironmentCheck : PageBase
	{
		public EnvironmentCheck()
			: base(lookingGlassDependency: true)
		{
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.UptimeState] as UptimeReportViewModel;
			var model = state ?? new UptimeReportViewModel();

			Initialize(model);

			var environmentCheckServerInfoUrl = GetPageUrl(Names.Tab.EnvironmentCheck, "EnvironmentCheckServerInfo");

			QoSNavButton.HRef = QosNavigationUrl;
			EnvCheckServerInfoButton.HRef = environmentCheckServerInfoUrl;
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