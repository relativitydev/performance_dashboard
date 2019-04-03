/*
	PDB QA Scripts:
	TheFraudmaker

	Author: Joseph Low
	Date: 8/11/2014

	Comments: This script will assist in triggering fraud countermeasures. Highlight and execute
		desired sections to control the manner in which fraud occurs. See my comments in each section
		below for more details. To repair the environment after fraud has been detected, run the
		FCMReset procedure.
*/

/******************************************************************************************************/

/* MODIFY PROCEDURES
 *	See sections below to modify individual procedures. The altered versions will just print a message
 *	instead of doing work. I've only included four procedures here because these are the ones in
 *	EDDSPerformance with an impact on the score calculations. The other procedures in this database
 *	are basically cosmetic. Make sure you're executing these in EDDSPerformance. */

--Modify BuildAndRateSample
ALTER PROCEDURE eddsdbo.QoS_BuildAndRateSample
	@QoSHourID bigint
	,@summaryDayHour datetime
	,@GlassRunID INT 
AS
	PRINT 'Were you looking for something?'
GO

--Modify LookingGlass
ALTER PROCEDURE eddsdbo.QoS_LookingGlass
	@msThreshold int = 0,
	@beginDate datetime = 24943,
	@endDate datetime = 24943,
	@workspace varchar(18) = '',
	@cleanup bit = 0,
	@debug int = 0,
	@install varchar(7) = '',
	@logging bit = 1
AS
	PRINT 'Were you looking for something?'
GO

--Modify LookingGlassDateSource
ALTER PROCEDURE eddsdbo.QoS_LookingGlassDateSource
	@startDate datetime = 24943
AS
	PRINT 'Were you looking for something?'
GO

--Modify QualityDataGenerator
ALTER PROCEDURE eddsdbo.QoS_QualityDataGenerator
	@GlassRunDateTime datetime,
	@GlassRunID int,
	@summaryDayHour datetime,
	@isRetry int,
	@logging int = 0,
	@debug int
AS
	PRINT 'Were you looking for something?'
GO

/******************************************************************************************************/

/* CHANGE COLUMN VALUES
 *	If you change a critical column's value without updating the RowHash, the data set becomes
 *	inconsistent. This should trip FCM when scoring happens or someone tries to export Trust scores.
 *	Change the "TOP" number to control the number of rows that are updated. You can comment out any
 *	columns you don't want to update. */

--Change values on UserXInstanceSummary
UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary SET
ArrivalRate = ArrivalRate + 1,
ServerArtifactID = ServerArtifactID + 1,
QoSHourID = QoSHourID + 1,
SummaryDayHour = DATEADD(hh, -1, SummaryDayHour),
TotalExecutionTime = TotalExecutionTime + 1,
AVGConcurrency = AVGConcurrency + 1,
AvgSQScorePerUser = AvgSQScorePerUser + 1,
AvgCQScorePerUser = AvgCQScorePerUser + 1
WHERE UserXID IN (SELECT TOP 1 UserXID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

GO

--Change values on CasesToAudit
UPDATE EDDSPerformance.eddsdbo.QoS_CasesToAudit SET
IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
ServerName = 'Server',
ServerID = ServerID + 1,
CaseArtifactID = CaseArtifactID + 1,
DatabaseName = 'DBName',
WorkspaceName = 'Workspace',
AuditStartDate = DATEADD(hh, -1, AuditStartDate)
WHERE RowID IN (SELECT TOP 1 RowID FROM EDDSPerformance.eddsdbo.QoS_CasesToAudit WHERE IsCompleted = 1); 

GO

--Change values on SampleHistory
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory] SET
QoSHourID = QoSHourID + 1,
SummaryDayHour = DATEADD(hh, -1, SummaryDayHour),
ServerArtifactID = ServerArtifactID + 1,
ArrivalRate = ArrivalRate + 1,
AVGConcurrency = AVGConcurrency + 1
WHERE UserXID IN (SELECT TOP 1 UserXID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

GO

--Change values on SystemLoadSummary
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary] SET
ServerArtifactID = ServerArtifactID + 1,
ServerTypeID = CASE WHEN ServerTypeID = 1 THEN 3 ELSE 1 END,
AvgRAMPagesePerSec = AvgRAMPagesePerSec + 1,
AvgCPUpct = AvgCPUpct + 1,
AvgRAMpct = AvgRAMpct + 1,
RAMPagesScore = RAMPagesScore + 1,
RAMPctScore = RAMPctScore + 1,
CPUScore = CPUScore + 1,
QoSHourID = QoSHourID + 1,
SummaryDayHour = DATEADD(hh, -1, SummaryDayHour)
WHERE QSLSID IN (SELECT TOP 1 QSLSID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

GO

--Change values on Ratings
UPDATE [EDDSPerformance].[eddsdbo].[QoS_Ratings] SET
QoSHourID = QoSHourID + 1,
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
WeekSystemLoadScore = WeekSystemLoadScore + 1,
SummaryDayHour = DATEADD(hh, -1, SummaryDayHour)
WHERE QRatingID IN (SELECT TOP 1 QRatingID FROM EDDSPerformance.eddsdbo.QoS_Ratings);

GO

--Change values on UptimeRatings
UPDATE [EDDSPerformance].[eddsdbo].[QoS_UptimeRatings] SET
HoursDown = HoursDown + 1,
UptimeScore = UptimeScore + 1,
SummaryDayHour = DATEADD(hh, -1, SummaryDayHour)
WHERE UpRatingID IN (SELECT TOP 1 UpRatingID FROM EDDSPerformance.eddsdbo.QoS_UptimeRatings);

GO

/******************************************************************************************************/

/* CHANGE ROW HASH
 *	If you set the RowHash to something that doesn't match the data, the data set becomes inconsistent.
 *	This basically gives the same result as above, but the method is slightly different. */

--Change values on UserXInstanceSummary
UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary SET
RowHash = 0x0F64A8EFDC42A5BF7228A12512B3B9C6CFAE26F5
WHERE UserXID IN (SELECT TOP 1 UserXID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

GO

--Change values on CasesToAudit
UPDATE EDDSPerformance.eddsdbo.QoS_CasesToAudit SET
RowHash = 0x0F64A8EFDC42A5BF7228A12512B3B9C6CFAE26F5
WHERE RowID IN (SELECT TOP 1 RowID FROM EDDSPerformance.eddsdbo.QoS_CasesToAudit WHERE IsCompleted = 1); 

GO

--Change values on SampleHistory
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory] SET
RowHash = 0x0F64A8EFDC42A5BF7228A12512B3B9C6CFAE26F5
WHERE UserXID IN (SELECT TOP 1 UserXID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

GO

--Change values on SystemLoadSummary
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary] SET
RowHash = 0x0F64A8EFDC42A5BF7228A12512B3B9C6CFAE26F5
WHERE QSLSID IN (SELECT TOP 1 QSLSID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

GO

--Change values on Ratings
UPDATE [EDDSPerformance].[eddsdbo].[QoS_Ratings] SET
RowHash = 0x0F64A8EFDC42A5BF7228A12512B3B9C6CFAE26F5
WHERE QRatingID IN (SELECT TOP 1 QRatingID FROM EDDSPerformance.eddsdbo.QoS_Ratings);

GO

--Change values on UptimeRatings
UPDATE [EDDSPerformance].[eddsdbo].[QoS_UptimeRatings] SET
RowHash = 0x0F64A8EFDC42A5BF7228A12512B3B9C6CFAE26F5
WHERE UpRatingID IN (SELECT TOP 1 UpRatingID FROM EDDSPerformance.eddsdbo.QoS_UptimeRatings);

GO

/******************************************************************************************************/

/* CHANGE FCM HASH
 *	If you change a snapshot on the FCM table, the data set becomes inconsistent. */
 
UPDATE EDDSPerformance.eddsdbo.QoS_FCM
SET RowHash = 0x0F64A8EFDC42A5BF7228A12512B3B9C6CFAE26F5
WHERE FCMID IN (SELECT TOP 1 FCMID FROM EDDSPerformance.eddsdbo.QoS_FCM);

GO

/******************************************************************************************************/

/* CHANGE FCM COLUMN VALUES
 *	If you change the SummaryDayHour on the FCM table, the data set may become inconsistent. However,
 *	the hashes that become invalid may no longer be important to us. Rows dated within 90 days of a given
 *	hash are considered when that hash is validated. We validate the most recent hash when we generate a
 *	new one, and we also validate the hash associated with the current Trust scores when they're checked. */
 
--Modify most recent entry
UPDATE EDDSPerformance.eddsdbo.QoS_FCM
SET SummaryDayHour = DATEADD(dd, -90, SummaryDayHour)
WHERE FCMID IN (SELECT TOP 1 FCMID FROM EDDSPerformance.eddsdbo.QoS_FCM ORDER BY FCMID DESC);

GO

--Modify based on current hour
UPDATE EDDSPerformance.eddsdbo.QoS_FCM
SET SummaryDayHour = DATEADD(dd, -90, SummaryDayHour)
WHERE SummaryDayHour = '2014-08-11 12:00:00.000'

GO

/******************************************************************************************************/

/* DELETE FCM ROW
 *	If you remove an entry from the FCM table, the data set becomes inconsistent. This only affects
 *	validation on rows with a SummaryDayHour within 90 days of and greater than the row that's removed. */
 
DELETE FROM EDDSPerformance.eddsdbo.QoS_FCM
WHERE FCMID IN (SELECT TOP 1 FCMID FROM EDDSPerformance.eddsdbo.QoS_FCM);
 
GO
 
/******************************************************************************************************/

/* RESET
 *	After you're caught messing with the data, you can run this to "fix" everything. Modifications you've
 *	made to column values will not be fixed (e.g. if you change the score to 100 for a given hour, this
 *	doesn't know how to change it back). Row hashes and snapshots will be magically regenerated to make
 *	your data look valid.
 *
 *	BEFORE RUNNING: Delete all instances of the Performance Dashboard agent
 *	DURING RUN: Do not run Looking Glass or attempt to use the configuration page
 *	AFTER RUNNING: Recreate the Performance Dashboard agent  */
 
EXEC EDDSPerformance.eddsdbo.FCMReset;

GO