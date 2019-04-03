USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND type in (N'U'))
BEGIN
	UPDATE eddsdbo.QoS_BackResults
	SET LoggedDate = dateadd(hour,datediff(hour,0,LoggedDate),0)
END