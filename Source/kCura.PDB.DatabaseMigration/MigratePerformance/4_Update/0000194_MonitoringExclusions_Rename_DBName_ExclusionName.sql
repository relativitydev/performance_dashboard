USE EDDSPerformance
GO

IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_MonitoringExclusions' AND COLUMN_NAME = 'DBName')
BEGIN
	EXEC sp_rename 'eddsdbo.QoS_MonitoringExclusions.DBName', 'ExclusionName', 'COLUMN'
END