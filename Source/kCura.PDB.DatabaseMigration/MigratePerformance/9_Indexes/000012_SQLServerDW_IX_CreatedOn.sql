USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[SQLServerDW]') AND name = N'IX_MeasureDate')
BEGIN
	DROP INDEX IX_MeasureDate ON eddsdbo.SQLServerDW
END

GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[SQLServerDW]') AND name = N'IX_MeasureDate')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_MeasureDate] ON [eddsdbo].[SQLServerDW]
	(
		[CreatedOn] ASC
	)
	INCLUDE ( [ServerID], [LowMemorySignalState] )
END