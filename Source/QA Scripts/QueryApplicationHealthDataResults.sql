USE EDDSPerformance
GO

IF OBJECT_ID('tempdb..#tempHealth') IS NULL
CREATE TABLE #tempHealth
(
	Id int,
	CaseArtifactID int,
	WorkspaceName varchar(max),
	DatabaseLocation varchar(max),
	MeasureDate datetime,
	UserCount int,
	ErrorCount int,
	LRQCount int
)
GO

DECLARE @startDate datetime = dateadd(dd, -7, getutcdate());
DECLARE @endDate datetime = getutcdate();
DECLARE @offset int = -300;

INSERT INTO #tempHealth
EXEC eddsdbo.GetApplicationHealthData
	@startDate,
	@endDate,
	@offset
	
SELECT COUNT(DISTINCT CaseArtifactID) FROM #tempHealth

/* Some other queries here? */
	
DROP TABLE #tempHealth