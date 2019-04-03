USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_BackSummary') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackSummary]') AND name = N'IX_GapResolvedDate')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [IX_GapResolvedDate] ON [eddsdbo].[QoS_BackSummary] 
		(
			[GapResolvedDate] ASC
		)
		INCLUDE ( [LastBackupDate])
		'		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END