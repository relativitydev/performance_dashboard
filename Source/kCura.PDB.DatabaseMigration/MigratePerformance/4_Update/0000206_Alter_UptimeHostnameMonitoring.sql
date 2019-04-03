USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.Server' ,'UptimeMonitoringResourceUseHttps') IS NULL
BEGIN
    ALTER TABLE eddsdbo.[Server]
    ADD UptimeMonitoringResourceUseHttps bit null
END

IF COL_LENGTH ('eddsdbo.Server' ,'UptimeMonitoringResourceHost') IS NULL
BEGIN
    ALTER TABLE eddsdbo.[Server]
    ADD UptimeMonitoringResourceHost varchar(max) null
END