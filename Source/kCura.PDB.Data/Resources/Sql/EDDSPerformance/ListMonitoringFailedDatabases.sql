USE EDDSPerformance

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_FailedDatabases' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	SELECT DBName
	FROM eddsdbo.QoS_FailedDatabases WITH(NOLOCK)
	ORDER BY DBName;
END