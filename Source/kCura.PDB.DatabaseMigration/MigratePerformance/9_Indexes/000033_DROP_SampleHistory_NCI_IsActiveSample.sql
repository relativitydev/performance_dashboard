USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND name = N'NCI_IsActiveSample')
BEGIN
	DROP INDEX NCI_IsActiveSample ON eddsdbo.QoS_SampleHistory
END