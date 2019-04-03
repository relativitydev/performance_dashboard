namespace kCura.PDD.Web
{
	using System;
	using System.Linq;
	using System.Web;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDD.Web.Services;

	public partial class FileLatencyReport : PageBase
	{
		public FileLatencyReport()
			: base(true)
		{
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.FileLatencyReportState] as FileLatencyViewModel;
			var model = state ?? new FileLatencyViewModel();

			string serverName = null;
			if (HttpContext.Current.Request.QueryString.AllKeys.Contains("ServerName"))
				serverName = HttpContext.Current.Request.QueryString["ServerName"];

			Initialize(model, serverName);

			QoSNavButton.HRef = QosNavigationUrl;
			BtnServer.HRef = GetPageUrl(Names.Tab.QualityOfService, "SystemLoadServer");
			BtnWaits.HRef = GetPageUrl(Names.Tab.QualityOfService, "Waits");
		}

		private void Initialize(FileLatencyViewModel model, String serverName)
		{
			if (false == string.IsNullOrEmpty(serverName))
				model.FilterConditions[FileLatency.Columns.ServerName] = serverName;

			var json = model.ToJson();
			VarscatState.Value = json;
			TimezoneOffset.Value = RequestService.GetTimezoneOffset(this.Request).ToString();
			SampleStart.Value = base.GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).ToString("s");
		}
	}
}