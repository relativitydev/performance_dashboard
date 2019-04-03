USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'ServerName'
	AND CHARACTER_MAXIMUM_LENGTH > 150)
BEGIN
	--There shouldn't be any rows that satisfy this condition, but just in case...
	UPDATE eddsdbo.QoS_VarscatOutputDetailCumulative
	SET ServerName = SUBSTRING(ServerName, 1, 150)
	WHERE LEN(ServerName) > 150

	--Resize ServerName column
	ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
	ALTER COLUMN ServerName nvarchar(150)
END