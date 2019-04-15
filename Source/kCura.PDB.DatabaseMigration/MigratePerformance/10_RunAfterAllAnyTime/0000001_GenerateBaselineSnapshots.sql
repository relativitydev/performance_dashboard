USE EDDSPerformance
GO

--Remote FCM table if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_FCM]') AND type in (N'U'))
BEGIN
	DROP TABLE [eddsdbo].[QoS_FCM]
END