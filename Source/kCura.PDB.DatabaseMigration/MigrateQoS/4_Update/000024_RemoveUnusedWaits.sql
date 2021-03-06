USE EDDSQoS
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_Waits' and TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	DELETE FROM eddsdbo.QoS_Waits
	WHERE WaitType IN (
		'PAGEIOLATCH_NL'
		,'PAGELATCH_DT'
		,'PAGELATCH_EX'
		,'PAGELATCH_KP'
		,'PAGELATCH_NL'
		,'PAGELATCH_SH'
		,'PAGELATCH_UP'
		,'LCK_M_RIn_NL'
	)
END