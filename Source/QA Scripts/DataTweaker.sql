USE EDDSPerformance
GO

--Change values on UserXInstanceSummary
UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary SET
ArrivalRate = ArrivalRate + 1
WHERE UserXID IN (SELECT TOP 1 UserXID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

GO

--Change values on CasesToAudit
UPDATE EDDSPerformance.eddsdbo.QoS_CasesToAudit SET
IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
WHERE RowID IN (SELECT TOP 1 RowID FROM EDDSPerformance.eddsdbo.QoS_CasesToAudit WHERE IsCompleted = 1); 

GO

--Change values on SampleHistory
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory] SET
ArrivalRate = ArrivalRate + 1
WHERE QSampleID IN (SELECT TOP 1 QSampleID FROM EDDSPerformance.eddsdbo.[QoS_SampleHistory]);

GO

--Change values on SystemLoadSummary
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary] SET
AvgRAMPagesePerSec = AvgRAMPagesePerSec + 1
WHERE QSLSID IN (SELECT TOP 1 QSLSID FROM EDDSPerformance.eddsdbo.[QoS_SystemLoadSummary]);

GO

--Change values on Ratings
UPDATE [EDDSPerformance].[eddsdbo].[QoS_Ratings] SET
UserExperience4SLRQScore = UserExperience4SLRQScore + 1,
UserExperienceSLRQScore = UserExperienceSLRQScore + 1,
SystemLoadScoreWeb = SystemLoadScoreWeb + 1,
SystemLoadScoreSQL = SystemLoadScoreSQL + 1,
SystemLoadScore = SystemLoadScore + 1,
BackupScore = BackupScore + 1,
DBCCScore = DBCCScore + 1,
WeekUserExperience4SLRQScore = WeekUserExperience4SLRQScore + 1,
WeekUserExperienceSLRQScore = WeekUserExperienceSLRQScore + 1,
WeekSystemLoadScoreWeb = WeekSystemLoadScoreWeb + 1,
WeekSystemLoadScoreSQL = WeekSystemLoadScoreSQL + 1,
WeekSystemLoadScore = WeekSystemLoadScore + 1
WHERE QRatingID IN (SELECT TOP 1 QRatingID FROM EDDSPerformance.eddsdbo.QoS_Ratings);

GO

--Change values on UptimeRatings
UPDATE [EDDSPerformance].[eddsdbo].[QoS_UptimeRatings] SET
HoursDown = HoursDown + 1,
UptimeScore = UptimeScore + 1
WHERE UpRatingID IN (SELECT TOP 1 UpRatingID FROM EDDSPerformance.eddsdbo.QoS_UptimeRatings);