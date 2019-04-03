USE EDDSPerformance

DECLARE @summaryDayHour DATETIME = DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0);
DECLARE @end DATETIME = DATEADD(hh, DATEDIFF(hh, 0, getdate()), 0);
DECLARE @start DATETIME = DATEADD(hh, -1, @end),
	@searchTerm NVARCHAR(MAX) = N'A significant part of SQL server process memory has been paged out',
	@serverId INT,
	@maxServerId INT,
	@serverName NVARCHAR(MAX),
	@SQL NVARCHAR(MAX),
	@parameterDefinition NVARCHAR(MAX) = N'@start DATETIME, @end DATETIME, @searchTerm NVARCHAR(MAX), @serverId INT';


--Track pageouts for the current hour
INSERT INTO eddsdbo.SQLServerPageouts
	(ServerID, SummaryDayHour, QoSHourID, Pageouts)
SELECT
	ServerID,
	@summaryDayHour,
	EDDSQoS.eddsdbo.QoS_GetServerHourID(rs.ArtifactID, @summaryDayHour),
	0
FROM EDDSPerformance.eddsdbo.[Server] as s
inner join edds.eddsdbo.ResourceServer as rs on rs.ArtifactID = s.ArtifactID
WHERE ServerTypeID = 3 --SQL servers
	AND COALESCE(IgnoreServer, 0) != 1
	AND DeletedOn IS NULL

SELECT @serverId = MIN(SSPID),
	@maxServerId = MAX(SSPID)
FROM eddsdbo.SQLServerPageouts
WHERE SummaryDayHour = @summaryDayHour

--This holds temporary xp_readerrorlog results
CREATE TABLE #errorLog
(
	LogDate DATETIME,
	LogProcess SYSNAME,
	LogText NVARCHAR(MAX)
)

--Loop through servers and read the error log for pageout warnings
WHILE (@serverId <= @maxServerId)
BEGIN
	--First, get the server's name
	SELECT
		@serverName = s.ServerName
	FROM eddsdbo.SQLServerPageouts ssp
	INNER JOIN eddsdbo.[Server] s
		ON ssp.ServerID = s.ServerID
	inner join edds.eddsdbo.ResourceServer as rs on rs.ArtifactID = s.ArtifactID
	WHERE ssp.SSPID = @serverId;

	--Then use the error log procedure to look for pageout warnings...
	SET @SQL = '
	INSERT INTO #errorLog
	EXEC ' + QUOTENAME(@serverName) + '.[{{resourcedbname}}].eddsdbo.QoS_ReadErrorLog
		@start,
		@end,
		@searchTerm';

	EXEC sp_executesql @SQL, @parameterDefinition,
		@start, @end, @searchTerm, @serverId;

	--If we found pageouts, track them
	UPDATE eddsdbo.SQLServerPageouts
	SET Pageouts = (SELECT COUNT(*) FROM #errorLog)
	WHERE SSPID = @serverId;

	--Clear out temporary data and increment the loop variable
	TRUNCATE TABLE #errorLog
	SET @serverId = ISNULL(
		(SELECT MIN(SSPID)
		 FROM eddsdbo.SQLServerPageouts
		 WHERE SSPID > @serverId),
		@maxServerId + 1
	);
END