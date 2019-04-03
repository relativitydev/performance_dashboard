USE [EDDSPerformance]
GO

DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_BackResults') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND name = N'IX_CaseArtifactID_LoggedDate_DBName')
	BEGIN
		SET @SQL = 'CREATE UNIQUE NONCLUSTERED INDEX [IX_CaseArtifactID_LoggedDate_DBName] ON eddsdbo.QoS_BackResults
		(
			CaseArtifactID ASC,
			LoggedDate ASC,
			DBName
		) INCLUDE (ServerName, BackupStatus, LastBackupDate)
		WITH ( IGNORE_DUP_KEY = ON )
		';
			
		EXEC sp_executesql @SQL
	END
	ELSE
	BEGIN
		ALTER INDEX IX_CaseArtifactID_LoggedDate_DBName
		ON eddsdbo.QoS_BackResults
		REBUILD	WITH (IGNORE_DUP_KEY = ON)
	END
END