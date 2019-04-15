USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND type in (N'U'))
BEGIN
	UPDATE eddsdbo.QoS_DBCCResults
	SET LoggedDate = dateadd(hour,datediff(hour,0,LoggedDate),0)
END