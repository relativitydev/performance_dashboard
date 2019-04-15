USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'IsComplex') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		ADD IsComplex BIT
	END
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	UPDATE eddsdbo.QoS_VarscatOutputDetailCumulative
	SET IsComplex =
		CASE WHEN ComplexityScore > 9 THEN 1
			ELSE 0
		END
END