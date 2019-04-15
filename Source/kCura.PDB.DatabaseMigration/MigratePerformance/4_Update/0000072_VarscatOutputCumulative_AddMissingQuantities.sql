USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputCumulative', 'QTYOrderByIndexed') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
		ADD QTYOrderByIndexed INT
	END

	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputCumulative', 'QTYIndexedSearchFields') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
		ADD QTYIndexedSearchFields INT
	END
END