USE [EDDSPerformance];

IF COL_LENGTH ('eddsdbo.Hours' ,'Status') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Hours]
	ADD [Status] int NOT NULL DEFAULT(1)
END