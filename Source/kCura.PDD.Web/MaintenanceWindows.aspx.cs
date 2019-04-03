namespace kCura.PDD.Web
{
	using System;
	using System.Web;
	using Constants;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Service.BISSummary;
	using Models.BISSummary;

	public partial class MaintenanceWindows : PageBase
	{
		public MaintenanceWindows()
		{
			_svc = new BestInServiceReportingService(this.SqlRepo);
		}

		private BestInServiceReportingService _svc;

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.UptimeState] as UptimeReportViewModel;
			var model = state ?? new UptimeReportViewModel();

			QoSNavButton.HRef = QosNavigationUrl;

			// TODO - Make url factory for local web navigation testing (outside of Relativity)
			// "/MaintenanceWindowScheduling.html"; // TEMP for local build
			// note: GetPageUrl is not used because it only handles .aspx pages but MaintenanceWindowScheduling is an .html page
			var createUrl =
				$"/Relativity/External.aspx?AppID=-1&ArtifactID=-1&SelectedTab={_svc.GetTabArtifactId(Names.Tab.MaintenanceScheduling)}&DirectTo=" +
				$"{HttpUtility.UrlEncode("%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/mvc/MaintenanceWindowScheduler")}";
            CreateMWButton.HRef = createUrl;
		}
    }
}