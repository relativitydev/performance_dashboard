USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'DatabaseName'
	AND CHARACTER_MAXIMUM_LENGTH < 128)
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	ALTER COLUMN DatabaseName nvarchar(128)
END