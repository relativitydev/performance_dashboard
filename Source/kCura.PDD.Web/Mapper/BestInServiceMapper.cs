namespace kCura.PDD.Web.Mapper
{
	using System.Linq;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDD.Web.Services;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Service.BISSummary;
	using PDB.Core.Extensions;

	public class BestInServiceMapper
	{
		public static int BackupDBCCWindow = 9;

		public static QualityOfServiceViewModel ToQualityOfServiceReportModel(BestInServiceReportingService service, int timezoneOffset)
		{
			//Retrieve header information
			var partner = service.GetPartnerName();
			var instance = service.GetInstanceName();

			//Retrieve scores
			var scores = service.GetQualityOfServiceScores();

			//Take the lowest score for each category
			var userExperienceScore = scores.Min(x => x.UserExperienceScore).GetValueOrDefault(100);
			var weeklyUserExperienceScore = scores.Min(x => x.WeeklyUserExperienceScore).GetValueOrDefault(100);
			var systemLoadScore = scores.Min(x => x.SystemLoadScore).GetValueOrDefault(100);
			var weeklySystemLoadScore = scores.Min(x => x.WeeklySystemLoadScore).GetValueOrDefault(100);
			var integrityScore = scores.Min(x => x.IntegrityScore).GetValueOrDefault(100);
			var weeklyIntegrityScore = scores.Min(x => x.WeeklyIntegrityScore).GetValueOrDefault(100);
			var uptimeScore = scores.Min(x => x.UptimeScore).GetValueOrDefault(100);
			var weeklyUptimeScore = scores.Min(x => x.WeeklyUptimeScore).GetValueOrDefault(100);

			//Average the lowest category scores
			var overallWeeklyScore = (weeklyUserExperienceScore + weeklySystemLoadScore + weeklyIntegrityScore + weeklyUptimeScore) / 4;
			var overallQuarterlyScore = (userExperienceScore + systemLoadScore + integrityScore + uptimeScore) / 4;

			//Retrieve grid data
			var monitoringInfo = service.SummarizeBackupIntegrityInfo();
			var worstUptime = service.ListWorstUptimeDays(timezoneOffset);
			var uptimePercentages = service.CalculateUptimePercentages();

			//Check DBCC monitoring configuration
			var dbccEnabled = service.IsDBCCMonitoringEnabled();

			//Construct view model
			var model = new QualityOfServiceViewModel
			{
				OverallScore = (int)overallQuarterlyScore,
				WeeklyScore = (int)overallWeeklyScore,
				PartnerName = partner,
				InstanceName = instance,
				BackupWindow = BackupDBCCWindow,
				UserExperience = new ServerDetailCategory
				{
					QuarterlyScore = (int)userExperienceScore,
					WeeklyScore = (int)weeklyUserExperienceScore,
					Servers = (from server in scores.OrderBy(x => x.UserExperienceScore).Take(10)
							   select new ServerScore
							   {
								   ArtifactId = server.ServerArtifactId,
								   Name = server.ServerName.Truncate(30),
								   QuarterlyScore = (int)server.UserExperienceScore.GetValueOrDefault(100),
								   WeeklyScore = (int)server.WeeklyUserExperienceScore.GetValueOrDefault(100),
								   ServerUrl = UrlHelper.GetPageUrl(service, Names.Tab.UserExperience, "UserExperienceServer", $"Server={server.ServerArtifactId}")
							   })
													 .ToList(),
					PageUrl = UrlHelper.GetPageUrl(service, Names.Tab.UserExperience, "UserExperienceServer")
				},
				SystemLoad = new ServerDetailCategory
				{
					QuarterlyScore = (int)systemLoadScore,
					WeeklyScore = (int)weeklySystemLoadScore,
					Servers = (from server in scores.OrderBy(x => x.SystemLoadScore).Take(10)
							   select new ServerScore
							   {
								   ArtifactId = server.ServerArtifactId,
								   Name = server.ServerName.Truncate(30),
								   QuarterlyScore = (int)server.SystemLoadScore.GetValueOrDefault(100),
								   WeeklyScore = (int)server.WeeklySystemLoadScore.GetValueOrDefault(100),
								   ServerUrl = UrlHelper.GetPageUrl(service, Names.Tab.InfrastructurePerformance, "SystemLoadServer", $"Server={server.ServerArtifactId}")
							   }).ToList(),
					PageUrl = UrlHelper.GetPageUrl(service, Names.Tab.InfrastructurePerformance, "SystemLoadServer")
				},
				Backup = new MaintenanceCategory
				{
					Score = (int)integrityScore,
					WeeklyScore = (int)weeklyIntegrityScore,
					BackupFrequencyScore = monitoringInfo.BackupFrequencyScore,
					DbccFrequencyScore = monitoringInfo.DbccFrequencyScore,
					BackupCoverageScore = monitoringInfo.BackupCoverageScore,
					DbccCoverageScore = monitoringInfo.DbccCoverageScore,
					MissedBackups = monitoringInfo.MissedBackups,
					MissedIntegrityChecks = monitoringInfo.MissedIntegrityChecks,
					RPOScore = monitoringInfo.RPOScore,
					RTOScore = monitoringInfo.RTOScore,
					MaxDataLossMinutes = monitoringInfo.MaxDataLossMinutes,
					TimeToRecoverHours = monitoringInfo.TimeToRecoverHours,
					FailedDatabases = monitoringInfo.FailedDatabases,
					FailedServers = monitoringInfo.FailedServers,
					DBCCMonitoringEnabled = dbccEnabled,
					PageUrl = UrlHelper.GetPageUrl(service, Names.Tab.RecoverabilityIntegrity, "Backup")
				},
				Uptime = new AvailabilityCategory
				{
					Score = (int)uptimeScore,
					WeeklyScore = (int)weeklyUptimeScore,
					UptimePercentage = uptimePercentages.QuarterlyUptimePercent,
					WeeklyUptimePercentage = uptimePercentages.WeeklyUptimePercent,
					DatesToReview = (from day in worstUptime.OrderBy(x => x.ScoreDate).GroupBy(x => x.ScoreDate.Month)
									 select new DateToReview
									 {
										 Date = day.First().ScoreDate.ToString("MMMM yyyy"),
										 HoursDown = day.Sum(x => x.HoursDown)
									 }).ToList(),
					PageUrl = UrlHelper.GetPageUrl(service, Names.Tab.Uptime, "Uptime")
				},
			};

			return model;
		}
	}
}