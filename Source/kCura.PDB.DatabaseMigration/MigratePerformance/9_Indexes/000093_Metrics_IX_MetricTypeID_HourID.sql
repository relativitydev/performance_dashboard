USE [EDDSPerformance]

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'Metrics') 
	AND NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[Metrics]') AND name = N'Metrics_IX_MetricTypeID_HourID')
BEGIN
    CREATE NONCLUSTERED INDEX [Metrics_IX_MetricTypeID_HourID] ON [eddsdbo].[Metrics]
    (
        [MetricTypeID] ASC,
        [HourID] ASC
    )
    INCLUDE ( 	[ID]) ON [PRIMARY]
END



