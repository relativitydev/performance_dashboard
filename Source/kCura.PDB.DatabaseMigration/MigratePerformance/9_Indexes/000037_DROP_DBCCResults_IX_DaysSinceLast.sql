USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND name = N'IX_DaysSinceLast')
BEGIN
	DROP INDEX IX_DaysSinceLast ON eddsdbo.QoS_DBCCResults
END