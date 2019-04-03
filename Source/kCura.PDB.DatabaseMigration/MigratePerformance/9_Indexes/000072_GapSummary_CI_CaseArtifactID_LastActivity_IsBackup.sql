USE [EDDSPerformance]
GO

--Drop old versions of this index that didn't account for Invariant store databases
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_GapSummary]') AND name = N'CI_AllGaps_CaseArtifactID_LastActivity')
BEGIN
	DROP INDEX CI_AllGaps_CaseArtifactID_LastActivity ON eddsdbo.QoS_GapSummary
END

GO

DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_GapSummary') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_GapSummary]') AND name = N'CI_GapSummary')
	BEGIN
		SET @SQL = 'CREATE UNIQUE CLUSTERED INDEX CI_GapSummary ON eddsdbo.QoS_GapSummary
	(
		[DatabaseName] ASC,
		[CaseArtifactID] ASC,
		[LastActivityDate] ASC,
		[IsBackup] ASC
	) WITH ( IGNORE_DUP_KEY = ON )'
			
		EXEC sp_executesql @SQL
	END
END