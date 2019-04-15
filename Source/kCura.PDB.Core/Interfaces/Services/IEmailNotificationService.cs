namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Collections.Generic;
	using System.Net.Mail;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public interface IEmailNotificationService
	{
		void SendQuarterlyScoreStatus();

		void SendQuarterlyScoreAlerts();

		void SendConfigurationChangeAlerts(DateTime auditStartTime);

		void SendConfigurationChangeAlerts(IList<ConfigurationAudit> changes);

		void SendWeeklyScoreAlerts();

		void SendInfrastructurePerformanceForecast();

		void SendUserExperienceForecast();

		void SendRecoverabilityIntegrityAlerts();

		IList<string> GetEmailList(string recipients);

		MailAddress MailAddressFromString(string address);

		MailMessage BuildMailMessage(SmtpSettings settings, string recipientList, string subject, string message);

		MailMessage BuildMailMessage(SmtpSettings settings, IList<string> recipients, string subject, string message);

		string BuildInfrastructurePerformanceForecast(string instanceName, int scoreThreshold, IList<SystemLoadForecast> forecasts);

		string BuildUserExperienceForecast(string instanceName, int scoreThreshold, IList<KeyValuePair<string, int>> forecasts);

	}
}
