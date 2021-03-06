USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_SystemLoadSummary'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_SystemLoadSummary', 'AvgRAMAvailKB') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_SystemLoadSummary
		ADD AvgRAMAvailKB DECIMAL (10,0)
	END
END