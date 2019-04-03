USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_DBCCHistory') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCHistory]') AND name = N'IX_DBName')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [IX_DBName] ON [eddsdbo].[QoS_DBCCHistory]
		(
			[DBName] ASC
		)
		INCLUDE ([CompletedOn])
		'		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END