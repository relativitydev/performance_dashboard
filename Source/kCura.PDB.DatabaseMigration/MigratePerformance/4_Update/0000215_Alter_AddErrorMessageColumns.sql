USE [EDDSPerformance]
GO

IF COL_LENGTH('eddsdbo.ProcessControl', 'LastExecSucceeded') IS NULL
BEGIN
	ALTER TABLE eddsdbo.ProcessControl
	ADD LastExecSucceeded bit
END

IF COL_LENGTH('eddsdbo.ProcessControl', 'LastErrorMessage') IS NULL
BEGIN
	ALTER TABLE eddsdbo.ProcessControl
	ADD LastErrorMessage varchar(max)
END
