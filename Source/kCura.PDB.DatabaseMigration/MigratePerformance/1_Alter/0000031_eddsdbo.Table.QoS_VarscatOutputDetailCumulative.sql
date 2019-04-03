USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [EDDSDBO].[QoS_VarscatOutputDetailCumulative]
	(
		VODCID INT IDENTITY (1, 1) PRIMARY KEY,
		ServerName nvarchar(150),
		QoSHourID bigint,
		SummaryDayHour datetime,
		GlassRunID int,
		CaseArtifactID int,
		SearchArtifactID int,
		SearchName nvarchar(max),
		AuditID int, 
		UserID int,
		ComplexityScore int,
		ExecutionTime int,--(rounded to nearest second unless it is less than 1, then it gets set to 1.)
		[Timestamp] datetime,
		Split int,
		Finish int,
		Bound bit,
		QoSAction int,
		IsCount bit,
		IsLongRunning bit,
		IsComplex bit,
		IsErrored int
	)
END
GO