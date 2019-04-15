USE EDDSPerformance;
GO

IF EXISTS (select 1 from sysobjects where [name] = 'FCMReset' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.FCMReset
END
GO
CREATE PROCEDURE EDDSDBO.FCMReset
WITH ENCRYPTION
AS
BEGIN
/* *************************************************** 
 * Steps before running:
 *	DELETE all instances of the PDB agent
 * ***************************************************/

/* Fix the hashes */
UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QSampleID, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(ArrivalRate, 0) AS varchar) +
	CAST(ISNULL(AVGConcurrency, 0) AS varchar)
	)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_SystemLoadSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QSLSID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(ServerTypeID, 0) AS varchar) +
	CAST(ISNULL(AvgRAMPagesePerSec, 0) AS varchar) +
	CAST(ISNULL(AvgCPUpct, 0) AS varchar) +
	CAST(ISNULL(AvgRAMpct, 0) AS varchar) +
	CAST(ISNULL(RAMPagesScore, 0) AS varchar) +
	CAST(ISNULL(RAMPctScore, 0) AS varchar) +
	CAST(ISNULL(CPUScore, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
	)
	
UPDATE [EDDSPerformance].[eddsdbo].[QoS_UserXInstanceSummary]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UserXID, 0) AS varchar) +
	CAST(ISNULL(ServerArtifactID, 0) AS varchar) +
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(TotalExecutionTime, 0) AS varchar) +
	CAST(ISNULL(AVGConcurrency, 0) AS varchar) +
	CAST(ISNULL(ArrivalRate, 0) AS varchar) +
	CAST(ISNULL(AvgSQScorePerUser, 0) AS varchar) +
	CAST(ISNULL(AvgCQScorePerUser, 0) AS varchar)
	)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_Ratings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(QoSHourID, 0) AS varchar) +
	CAST(ISNULL(UserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(UserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(SystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(BackupScore, 0) AS varchar) +
	CAST(ISNULL(DBCCScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperience4SLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekUserExperienceSLRQScore, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreWeb, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScoreSQL, 0) AS varchar) +
	CAST(ISNULL(WeekSystemLoadScore, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar) +
	CAST(ISNULL(QRatingID, 0) AS varchar)
)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_UptimeRatings]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(UpRatingID, 0) AS varchar) +
	CAST(ISNULL(HoursDown, 0) AS varchar) +
	CAST(ISNULL(UptimeScore, 0) AS varchar) +
	CAST(ISNULL(SummaryDayHour, 0) AS varchar)
	)

UPDATE [EDDSPerformance].[eddsdbo].[QoS_CasesToAudit]
SET RowHash = HASHBYTES('SHA1',
	CAST(ISNULL(RowID, 0) AS varchar) +
	CAST(ISNULL(ServerName, 0) AS varchar) +
	CAST(ISNULL(ServerID, 0) AS varchar) +
	CAST(ISNULL(CaseArtifactID, 0) AS varchar) +
	CAST(ISNULL(DatabaseName, 0) AS varchar) +
	CAST(ISNULL(WorkspaceName, 0) AS varchar) +
	CAST(ISNULL(AuditStartDate, 0) AS varchar) +
	CAST(ISNULL(IsActive, 0) AS varchar)
	)
WHERE IsCompleted = 1

/* Fix the FCM snapshots */

TRUNCATE TABLE EDDSPerformance.eddsdbo.QoS_FCM;

DECLARE @s datetime = (SELECT MIN(AuditStartDate) FROM EDDSPerformance.eddsdbo.QoS_CasesToAudit WHERE IsCompleted = 1 AND IsActive = 1);
DECLARE @e datetime = (SELECT MAX(AuditStartDate) FROM EDDSPerformance.eddsdbo.QoS_CasesToAudit WHERE IsCompleted = 1 AND IsActive = 1);

WHILE (@s <= @e)
BEGIN
	EXEC EDDSPerformance.eddsdbo.GenerateIntegritySnapshot @s;
	SET @s = DATEADD(hh, 1, @s);
END

/* Fix stored procedures */

DECLARE @id bigint;

SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_BuildandRateSample.sql');

UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
WHERE id = @id;

SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_QualityDataGenerator.sql');

UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
WHERE id = @id;

SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_LookingGlass_3.0.1.sql');

UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
WHERE id = @id;

SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_LookingGlass SourceDate.sql');

UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
WHERE id = @id;

/* Force the user to fix the backup scripts */

UPDATE EDDSPerformance.eddsdbo.Configuration
SET Value = 'X'
WHERE Section = 'kCura.PDB' AND Name = 'AdminScriptsVersion'

/* Reset the QoS extended property */

IF EXISTS (
	SELECT name, value
	FROM fn_listextendedproperty(default, default, default, default, default, default, default)
	WHERE objtype is null and objname is null and name = 'QoS'
)
BEGIN
	EXEC sys.sp_updateextendedproperty @name = 'QoS', @value = '';
END

/* *************************************************** 
 * Steps upon completion:
 *	Install backup/DBCC scripts via custom pages
 *	Recreate the Performance Dashboard agent 
 *	Verify that EDDSPerformance migrations succeed
 * ***************************************************/
END