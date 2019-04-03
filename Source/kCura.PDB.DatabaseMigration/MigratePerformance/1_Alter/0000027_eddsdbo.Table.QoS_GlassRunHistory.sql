USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_GlassRunHistory' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE EDDSDBO.QoS_GlassRunHistory
	(
		GlassRunID INT IDENTITY ( 1 , 1 ),PRIMARY KEY (GlassRunID)
		,RunDateTime datetime
		,RunDuration int
		,FailedCases int
		,isActive bit
	)
END
GO