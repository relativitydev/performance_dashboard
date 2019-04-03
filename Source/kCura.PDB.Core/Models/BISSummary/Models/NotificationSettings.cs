namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class NotificationSettings
	{
		public NotificationSettings()
		{
			WeeklyScoreAlert = new AlertConfiguration();
			SystemLoadForecast = new AlertConfiguration();
			UserExperienceForecast = new AlertConfiguration();
			QuarterlyScoreAlert = new AlertConfiguration();
			QuarterlyScoreStatus = new AlertConfiguration();
			BackupDBCCAlert = new AlertConfiguration();
			ConfigurationChangeAlert = new AlertConfiguration();
		}

		public AlertConfiguration WeeklyScoreAlert;
		public AlertConfiguration SystemLoadForecast;
		public AlertConfiguration UserExperienceForecast;
		public AlertConfiguration QuarterlyScoreAlert;
		public AlertConfiguration QuarterlyScoreStatus;
		public AlertConfiguration BackupDBCCAlert;
		public AlertConfiguration ConfigurationChangeAlert;
	}
}