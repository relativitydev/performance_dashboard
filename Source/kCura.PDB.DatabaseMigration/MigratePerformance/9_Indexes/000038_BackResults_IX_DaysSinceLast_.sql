USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_BackResults') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND name = N'IX_DaysSinceLast')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [IX_DaysSinceLast] ON [eddsdbo].[QoS_BackResults]
		(
			[DaysSinceLast] ASC
		)
		INCLUDE ([CaseArtifactID], [LastBackupDate], [ServerName])
		'		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END