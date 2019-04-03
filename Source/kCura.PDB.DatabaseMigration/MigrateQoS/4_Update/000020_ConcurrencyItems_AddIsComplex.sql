USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_ConcurrencyItems'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_ConcurrencyItems', 'IsComplex') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_ConcurrencyItems
		ADD IsComplex BIT
	END
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_ConcurrencyItems'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_ConcurrencyItems', 'IsComplex') IS NOT NULL
	BEGIN
		UPDATE ci
		SET ci.IsComplex = vod.IsComplex
		FROM eddsdbo.QoS_ConcurrencyItems ci
		INNER JOIN eddsdbo.QoS_VarscatOutputDetail vod
		ON ci.QoS_VODID = vod.VODID
	END
END