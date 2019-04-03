USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_DBCCResults') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND name = N'IX_CaseArtifactID_LoggedDate_LastCleanDBCCDate')
	BEGIN			
		SET @SQL = 'CREATE UNIQUE NONCLUSTERED INDEX [IX_CaseArtifactID_LoggedDate_LastCleanDBCCDate] ON eddsdbo.QoS_DBCCResults
		(
			CaseArtifactID,
			LoggedDate,
			LastCleanDBCCDate,
			DBCCStatus
		) WITH ( IGNORE_DUP_KEY = ON )'		
			
		EXEC sp_executesql @SQL
	END
	ELSE
	BEGIN
		ALTER INDEX IX_CaseArtifactID_LoggedDate_LastCleanDBCCDate
		ON eddsdbo.QoS_DBCCResults
		REBUILD	WITH (IGNORE_DUP_KEY = ON)
	END
END