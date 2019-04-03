namespace kCura.PDD.Web
{
	using System;
	using System.Web.Script.Serialization;
	using global::Relativity.CustomPages;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.ViewModels;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Constants;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDD.Web.Services;

	public partial class RecoverabilityIntegrityReport : PageBase
	{
		#region Private Members
		private readonly BackupDbccService _backupDbcc;
		private readonly IQualityIndicatorService _indicatorService;
		#endregion

		public RecoverabilityIntegrityReport()
			: base(lookingGlassDependency: true)
		{
			_backupDbcc = this.GetBackupDbccService();
			_indicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(this.SqlRepo.ConfigurationRepository));
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.RecoverabilityIntegrityState] as RecoverabilityIntegrityViewModel;
			var model = state ?? new RecoverabilityIntegrityViewModel();

			Initialize(model);

			var scores = _backupDbcc.GetOverallScores();

			QoSNavButton.HRef = QosNavigationUrl;

			QuarterlyScore.Attributes["class"] = _indicatorService.GetCssClassForScore(scores.QuarterlyIntegrityScore, true);
			QuarterlyScore.InnerText = _indicatorService.GetIndicatorForScore(scores.QuarterlyIntegrityScore) != PDB.Core.Enumerations.QualityIndicator.None
										? scores.QuarterlyIntegrityScore.ToString() 
										: "N/A";
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

		private void Initialize(RecoverabilityIntegrityViewModel model)
		{
			var json = new JavaScriptSerializer().Serialize(model);
			VarscatState.Value = json;

			TimezoneOffset.Value = RequestService.GetTimezoneOffset(this.Request).ToString();
			SampleStart.Value = base.GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).ToString("s");
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
	}
}