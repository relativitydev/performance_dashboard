USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[LatencyDW]') AND type in (N'U'))
DROP TABLE eddsdbo.LatencyDW