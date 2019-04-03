USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserXInstanceSummary]') AND name = N'NCI_QoSHourID')
BEGIN
	DROP INDEX NCI_QoSHourID ON eddsdbo.QoS_UserXInstanceSummary
END