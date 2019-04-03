namespace kCura.PDD.Web
{
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Models.BISSummary;
	using System;
	using System.Web.Script.Serialization;
	using kCura.PDB.Core.Constants;
	using kCura.PDD.Web.Services;

	public partial class EnvironmentCheckServerInfo : PageBase
	{
		public EnvironmentCheckServerInfo()
			: base(true)
		{
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.UptimeState] as UptimeReportViewModel;
			var model = state ?? new UptimeReportViewModel();

			Initialize(model);

			var environmentCheckUrl = GetPageUrl(Names.Tab.EnvironmentCheck, "EnvironmentCheck");

			QoSNavButton.HRef = QosNavigationUrl;
			EnvCheckButton.HRef = environmentCheckUrl;
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