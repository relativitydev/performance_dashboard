namespace kCura.PDD.Web
{
	using System;
	using System.Reflection;
	using System.Web;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Enum;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDD.Web.Services;
	using PDB.Data.Services;

	public class PageBase : System.Web.UI.Page
	{
		public const string ErrorPageUrlFormatString = "~/Error.aspx?mode={0}";
		public bool LookingGlassDependency = false;
		public bool IsFraudDetected = false;
		public string DateFormat = "m/d/Y";
		public string TimeFormat = "g A";
		public LookingGlassInformation GlassInfo;
		protected ISqlServerRepository SqlRepo;
		protected AdministrationInstallationService ScriptInstallService;
		private readonly BestInServiceReportingService reportingService;
		private IVersionCheckService versionCheckService;
		protected IConnectionFactory connectionFactory;

		public PageBase()
		{
			connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			SqlRepo = new SqlServerRepository(connectionFactory);
			reportingService = new BestInServiceReportingService(SqlRepo);
		}

		public PageBase(bool lookingGlassDependency)
			: this()
		{
			LookingGlassDependency = lookingGlassDependency;
		}

		protected override void OnLoad(EventArgs e)
		{
			//Auto-detect formats for date pickers based on server-side culture
			var dt = new DateTime(1900, 10, 20, 11, 0, 0);
			DateFormat = dt
				.ToString("d", System.Globalization.CultureInfo.CurrentCulture)
				.Replace("1900", "Y")
				.Replace("20", "d")
				.Replace("10", "m");

			TimeFormat = dt
				.ToString("t", System.Globalization.CultureInfo.CurrentCulture)
				.Replace("11:00 AM", "g A")
				.Replace("11:00", "H:i");

			if (!IsPostBack)
			{
				//Display an error if EDDSPerformance is missing
				if (!SqlRepo.PerformanceExists())
					Response.Redirect(string.Format(ErrorPageUrlFormatString, ErrorMode.General));

				//Redirect if elevated permissions are needed for script updates
				if (!SqlRepo.AdminScriptsInstalled())
					Response.Redirect("~/AdministrationInstall.aspx");

				// Set Timezone offset
				SetTimeZoneOffset();
			}

			//Initialize the reporting service
			var service = new BestInServiceReportingService(SqlRepo);

			//If the page requires QoS data, perform additional checks
			if (LookingGlassDependency)
			{
				//Redirect if LookingGlass hasn't created all the data tables or hasn't run successfully
				if (!service.LookingGlassHasRun() && !IsPostBack)
					Response.Redirect(string.Format(ErrorPageUrlFormatString, ErrorMode.ReportDataPending));
			}

			//Sample range information should be available for pages that don't depend on QoS data (e.g. backfill page)
			var range = service.GetSampleRange(RequestService.GetTimezoneOffset(this.Request));
			GlassInfo = new LookingGlassInformation(range);

			//Invoke base method
			base.OnLoad(e);
		}

		protected void SetTimeZoneOffset()
		{
			if (Page.Request.Cookies["TimeZoneOffset"] != null) return;
			var cookie = new HttpCookie("TimeZoneOffset", $"{TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes}")
			{
				Expires = DateTime.Now.AddDays(1)
			};
			Page.Request.Cookies.Add(cookie);
		}

		protected string GetPageUrl(string tabName, string pageName, string additionalParamerters = null)
		{
			return UrlHelper.GetPageUrl(reportingService, tabName, pageName, additionalParamerters);
		}

		protected string QosNavigationUrl => GetPageUrl(Names.Tab.QualityOfService, "ServiceQuality");
	}
}
