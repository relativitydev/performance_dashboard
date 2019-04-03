USE EDDSPerformance

DECLARE @failedDatabases int = 0,
	@failedServers int = 0,
	@serverErrorMsg nvarchar(max),
	@databaseErrorMsg nvarchar(max);

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_FailedDatabases]') AND type in (N'U'))
BEGIN
	SELECT @failedDatabases = COUNT(*),
		@databaseErrorMsg = MIN(Errors)
	FROM eddsdbo.QoS_FailedDatabases;
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_tempServers]') AND type in (N'U'))
BEGIN
	SELECT @failedServers = COUNT(*),
		@serverErrorMsg = MIN(Errors)
	FROM eddsdbo.QoS_tempServers
WHERE Failed = 1;
END

SELECT @failedServers FailedServers,
	@failedDatabases FailedDatabases,
	@serverErrorMsg ServerErrors,
	@databaseErrorMsg DatabaseErrors