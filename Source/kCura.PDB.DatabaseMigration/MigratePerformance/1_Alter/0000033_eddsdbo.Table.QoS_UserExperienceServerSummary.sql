USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_UserExperienceServerSummary' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE eddsdbo.QoS_UserExperienceServerSummary
	(
		[ServerSummaryID] INT IDENTITY (1, 1) PRIMARY KEY,
		[ServerArtifactID] INT,
		[Server] NVARCHAR(150),
		[CaseArtifactID] INT,
		[Workspace] NVARCHAR(150),
		[Score] INT,
		[TotalLongRunning] INT,
		[TotalUsers] INT,
		[TotalSearchAudits] INT,
		[TotalNonSearchAudits] INT,
		[TotalAudits] INT,
		[TotalExecutionTime] BIGINT,
		[SummaryDayHour] DATETIME,
		[QoSHourID] BIGINT
	);
END
GO