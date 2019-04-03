USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_MonitoringExclusions') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_MonitoringExclusions]') AND name = N'IX_SkipBackups')
	BEGIN			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [IX_SkipBackups] ON [eddsdbo].[QoS_MonitoringExclusions] 
		(
			[SkipBackups] ASC
		)
		INCLUDE ([ExclusionName])
		'
			
		EXEC sp_executesql @SQL
	END
END