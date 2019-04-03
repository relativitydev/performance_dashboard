USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutput', 'QTYOrderByIndexed') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutput
		ADD QTYOrderByIndexed INT
	END

	IF COL_LENGTH('eddsdbo.QoS_VarscatOutput', 'QTYIndexedSearchFields') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutput
		ADD QTYIndexedSearchFields INT
	END
END