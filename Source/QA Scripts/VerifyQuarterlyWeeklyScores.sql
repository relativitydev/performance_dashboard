/*
	PDB QA Scripts:
	Verify Quarterly/Weekly Scores

	Author: Joseph Low
	Date: 7/15/2014

	Comments: This script assumes you have run the SampleDataGenerator script previously, executed a particular test scenario in 7DayQoSDataScenarios, and then run the GenerateRatings script.
		See further comments below for what each of these queries means.
*/

--Current scores for a particular server
SELECT TOP 1
 ServerArtifactID,
 CASE
			WHEN qr.BackupScore >= 0 AND qr.DBCCScore >= 0
				THEN ((UserExperience4SLRQScore + UserExperienceSLRQScore)/2 +
						SystemLoadScore +
						(qr.BackupScore + qr.DBCCScore)/2 +
						UptimeScore)/4
			WHEN qr.BackupScore >= 0
				THEN ((UserExperience4SLRQScore + UserExperienceSLRQScore)/2 +
						SystemLoadScore +
						qr.BackupScore +
						UptimeScore)/4
			WHEN qr.DBCCScore >= 0
				THEN ((UserExperience4SLRQScore + UserExperienceSLRQScore)/2 +
						SystemLoadScore +
						qr.DBCCScore +
						UptimeScore)/4
			ELSE ((UserExperience4SLRQScore + UserExperienceSLRQScore)/2 + 
					SystemLoadScore +
					UptimeScore)/3
		END AS OverallScore,
 (UserExperience4SLRQScore + UserExperienceSLRQScore)/2 UserExperienceQuarterlyScore,
 (WeekUserExperience4SLRQScore + WeekUserExperienceSLRQScore)/2 UserExperienceWeeklyScore,
 SystemLoadScore SystemLoadQuarterlyScore,
 WeekSystemLoadScore SystemLoadWeeklyScore,
 CASE
	WHEN BackupScore >= 0 AND DBCCScore >= 0 THEN (BackupScore + DBCCScore)/2 
	WHEN BackupScore >= 0 THEN BackupScore
	WHEN DBCCScore >= 0 THEN DBCCScore
	ELSE -1
 END AS BackupAndDBCCScore,
 UptimeScore
 FROM EDDSPerformance.eddsdbo.QoS_Ratings QR
 INNER JOIN EDDSPerformance.eddsdbo.QoS_UptimeRatings UR
ON QR.SummaryDayHour = UR.SummaryDayHour
WHERE QR.ServerArtifactID = 1015096
ORDER BY QRatingID DESC

--All System Load Data
SELECT * FROM [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary]
ORDER BY SummaryDayHour

--All User Experience Data
SELECT * FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
WHERE ServerArtifactID = 1015543
ORDER BY SummaryDayHour

-- System Load score calculation for a particular server
SELECT AVG(CPUscore) CPUScore, AVG(RAMPagesScore) RAMPagesScore, AVG(RAMPctScore) RamPctScore, (AVG(CPUscore) + AVG(RAMPagesScore) + AVG(RAMPctScore))/3 SystemLoadScore
FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary sls
INNER JOIN EDDSperformance.eddsdbo.QoS_SampleHistory sh
ON sls.ServerArtifactID = sh.ServerArtifactID AND sls.SummaryDayHour = sh.SummaryDayHour
WHERE IsActiveWeeklySample = 1
	AND sh.ServerArtifactID = 1015543
	AND sls.ServerTypeID = 1 --1 is web servers (produces [WeekSystemLoadScoreWeb]), 3 is SQL servers (produces [WeekSystemLoadScoreSQL])

--User Experience Score calculation for a particular server
SELECT AVG(AvgSQScorePerUser) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary uxis
INNER JOIN EDDSperformance.eddsdbo.QoS_SampleHistory sh
ON uxis.ServerArtifactID = sh.ServerArtifactID AND uxis.SummaryDayHour = sh.SummaryDayHour
 --Use sh.IsActiveWeeklySample = 1 to check WeekUserExperienceSLRQScore
 --Use sh.IsActiveWeekly4Sample = 1 to check WeekUserExperience4SLRQScore
WHERE sh.IsActiveWeeklySample = 1
 --You can look at a different server depending on what shows on the Delivery Metrics page (1015543 is K12-R81-2, 1015096 is K12-R81-1)
	AND sh.ServerArtifactID = 1015543

--Groups User Experience data by day so you can check activity in the data set (arrival rate of 0 is inactive, won't go into sample)
SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, SummaryDayHour)) SummaryDay, MAX(ArrivalRate) ArrivalRate
FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, SummaryDayHour))

--Shows all sample data by hour
SELECT ServerArtifactID, QoSHourID, SummaryDayHour, ArrivalRate, IsActiveSample, IsActive4Sample, IsActiveWeeklySample, IsActiveWeekly4Sample
FROM [EDDSPerformance].[eddsdbo].[QoS_SampleHistory]
ORDER BY ArrivalRate DESC, SummaryDayHour

--Shows all sample data grouped by day (if any hour is in a particular sample set, this will show 1 for that set)
--Inactive days will show nulls for the sample columns
SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, uxis.SummaryDayHour)) SummaryDay,
	MAX(CAST(IsActiveSample as tinyint)) IsActiveSample, --90 day sample (top 20%)
	MAX(CAST(IsActive4Sample as tinyint)) IsActive4Sample, --90 day sample (top 4%)
	MAX(CAST(IsActiveWeeklySample as tinyint)) IsActiveWeeklySample, --Weekly sample (top 20%)
	MAX(CAST(IsActiveWeekly4Sample as tinyint)) IsActiveWeekly4Sample --Weekly sample (top 4%)
FROM [EDDSPerformance].[eddsdbo].[QoS_SampleHistory] sh
RIGHT JOIN EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary uxis
ON sh.ServerArtifactID = uxis.ServerArtifactID AND sh.SummaryDayHour = uxis.SummaryDayHour
GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, uxis.SummaryDayHour))
ORDER BY DATEADD(dd, 0, DATEDIFF(dd, 0, uxis.SummaryDayHour))

--View all scores
SELECT * FROM [EDDSPerformance].[eddsdbo].[QoS_Ratings]

--User Experience - Underperforming Hours
EXEC EDDSPerformance.eddsdbo.QoS_ReportDrill
	@drillMo = 'UserEx',
	@serverArtifactID = 1015096,
	@depth = 5
	
--System Load - Underperforming Hours
EXEC EDDSPerformance.eddsdbo.QoS_ReportDrill
	@drillMo = 'ServerPerformance',
	@serverArtifactID = 1015096,
	@depth = 5
	
--Backup/DBCC - Biggest Issues
EXEC EDDSPerformance.eddsdbo.QoS_ReportDrill
	@drillMo = 'BackupDBCC',
	@depth = 5
	
--Uptime - Worst Hours
EXEC EDDSPerformance.eddsdbo.QoS_ReportDrill
	@drillMo = 'Uptime',
	@depth = 5