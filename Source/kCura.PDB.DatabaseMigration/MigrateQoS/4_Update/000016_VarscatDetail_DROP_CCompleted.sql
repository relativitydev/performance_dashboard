USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = 'QoS_VarscatOutputDetail'
	AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetail', 'CCompleted') IS NOT NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetail
		DROP COLUMN CCompleted
	END
END