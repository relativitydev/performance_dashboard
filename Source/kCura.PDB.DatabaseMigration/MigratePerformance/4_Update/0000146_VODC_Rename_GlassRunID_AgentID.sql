USE EDDSPerformance
GO

IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND COLUMN_NAME = 'GlassRunID')
BEGIN
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutputDetailCumulative.GlassRunID', 'AgentID', 'COLUMN'
END