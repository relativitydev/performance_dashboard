USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'CCompleted') IS NOT NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		DROP COLUMN CCompleted
	END
END