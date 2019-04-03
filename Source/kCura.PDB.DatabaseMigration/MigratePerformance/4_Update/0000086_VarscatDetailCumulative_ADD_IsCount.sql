USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'TopCount') IS NOT NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		DROP COLUMN TopCount
	END
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'IsCount') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		ADD IsCount BIT
	END
END