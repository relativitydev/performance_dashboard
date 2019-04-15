USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_SystemLoadSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_SystemLoadSummary
	(
		QSLSID INT IDENTITY ( 1 , 1 ),PRIMARY KEY (QSLSID)
		,ServerArtifactID int
		,ServerTypeID int
		,AvgRAMPagesePerSec DECIMAL
		,AvgCPUpct DECIMAL (5, 2)
		,AvgRAMpct DECIMAL (5, 2)
		,AvgRAMAvailKB DECIMAL (10,0)
		,RAMPagesScore DECIMAL (5, 2)
		,RAMPctScore DECIMAL (5, 2)
		,CPUScore DECIMAL (5, 2)
		,QoSHourID bigint
		,SummaryDayHour datetime
		,RowHash binary(20)
	)
END
GO