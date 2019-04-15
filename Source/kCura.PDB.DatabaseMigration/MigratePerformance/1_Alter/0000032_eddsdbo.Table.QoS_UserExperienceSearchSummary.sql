USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_UserExperienceSearchSummary' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE eddsdbo.QoS_UserExperienceSearchSummary
	(
		[SearchSummaryID] INT IDENTITY (1, 1) PRIMARY KEY,
		[CaseArtifactID] INT,
		[SearchArtifactID] INT,
		[Search] NVARCHAR(150),
		[LastAuditID] INT,
		[UserArtifactID] INT,
		[User] NVARCHAR(150),
		[TotalRunTime] BIGINT,
		[AverageRunTime] INT,
		[TotalRuns] INT,
		[PercentLongRunning] INT,
		[IsComplex] BIT,
		[SummaryDayHour] DATETIME,
		[QoSHourID] BIGINT
	)
END
GO