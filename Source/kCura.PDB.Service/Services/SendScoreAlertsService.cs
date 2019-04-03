namespace kCura.PDB.Service.Services
{
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Services;

	public class SendScoreAlertsService : ISendScoreAlertsService
	{
		private readonly IEmailNotificationService emailNotificationService;
		private readonly IProcessControlWrapper processControlWrapper;

		public SendScoreAlertsService(
			IEmailNotificationService emailNotificationService,
			IProcessControlWrapper processControlWrapper)
		{
			this.emailNotificationService = emailNotificationService;
			this.processControlWrapper = processControlWrapper;
		}

		public int SendNotifications(int hourId)
		{
			// Quarterly Score Alerts
			this.processControlWrapper.Execute(
				ProcessControlId.QuarterlyScoreAlerts,
				this.emailNotificationService.SendQuarterlyScoreAlerts);

			// Quarterly Score Status
			this.processControlWrapper.Execute(
				ProcessControlId.QuarterlyScoreStatus,
				this.emailNotificationService.SendQuarterlyScoreStatus);

			// Configuration Change Alerts
			this.processControlWrapper.Execute(
				ProcessControlId.ConfigurationChangeAlerts,
				date => this.emailNotificationService.SendConfigurationChangeAlerts(date));

			// Weekly Score Alerts
			this.processControlWrapper.Execute(
				ProcessControlId.WeeklyScoreAlerts,
				this.emailNotificationService.SendWeeklyScoreAlerts);

			// Infrastructure Performance Forecast
			this.processControlWrapper.Execute(
				ProcessControlId.InfrastructurePerformanceForecast,
				this.emailNotificationService.SendInfrastructurePerformanceForecast);

			// User Experience Forecast
			this.processControlWrapper.Execute(
				ProcessControlId.UserExperienceForecast,
				() => this.emailNotificationService.SendUserExperienceForecast());

			// Recoverability Integrity Alerts
			this.processControlWrapper.Execute(
				ProcessControlId.RecoverabilityIntegrityAlerts,
				this.emailNotificationService.SendRecoverabilityIntegrityAlerts);

			return hourId;
		}

	}
}
