USE [EDDSPerformance]
GO

IF COL_LENGTH('eddsdbo.Server', 'LastServerBackup') IS NULL
BEGIN
	ALTER TABLE eddsdbo.Server
	ADD LastServerBackup Datetime
END
