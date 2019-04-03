USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.Server' ,'AdminScriptsVersion') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Server]
    ADD AdminScriptsVersion varchar(20) null
END