USE EDDSPerformance
GO

IF COL_LENGTH('eddsdbo.Server', 'LastChecked') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Server]
	ADD LastChecked DATETIME
END