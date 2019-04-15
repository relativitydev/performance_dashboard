USE EDDSPerformance
Go

IF EXISTS (select 1 from sysobjects where [name] = 'QoS_ReportDrill' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_ReportDrill
END
GO
Create Procedure EDDSDBO.QoS_ReportDrill
	--This procedure returns data for each of the four detail reports. It takes as parameters a resource server artifact ID and a drill mode. The options for drill mode are the following strings: UserEx, ServerPerformance, BackupDBCC, Uptime.  Depth controls the number of items to return
	@serverArtifactID int = -1
	,@drillmo varchar (22)
	,@depth int = 2147483647
	,@timezoneOffset int = 0 --Timezone offset to use in minutes, exactly like the offset for Application Performance and Server Health (e.g. -300)
AS
BEGIN

DECLARE @window tinyint = 9, --Backup/DBCC window is no longer configurable. This is always 9 days.
	@lastHourProcessed datetime = (SELECT MAX(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_Ratings WITH(NOLOCK)),
	@sqlTimezoneOffset int = DATEDIFF(MINUTE, getutcdate(), getdate()); --Difference in minutes between SQL local time and UTC

IF @drillmo = 'Uptime'
BEGIN
	--Retrieve daily downtime information, largest and most recent outages first
	SELECT TOP (@depth)
		SUM(HoursDown) AS HoursDown,
		AVG(UptimeScore) AS Score,
		DATEADD(hh,
			-1 * DATEPART(hh, DATEADD(MINUTE, @timezoneOffset, SummaryDayHour)),
			DATEADD(MINUTE, @timezoneOffset,SummaryDayHour)
		) AS SummaryDayHour
	FROM EDDSDBO.QoS_UptimeRatings WITH(NOLOCK)
	WHERE SummaryDayHour > DateAdd(DY, -90, @lastHourProcessed)
	GROUP BY DATEADD(hh, -1 * DATEPART(hh, DATEADD(MINUTE, @timezoneOffset, SummaryDayHour)), DATEADD(MINUTE, @timezoneOffset, SummaryDayHour))
	ORDER BY SUM(HoursDown) DESC, SummaryDayHour DESC
END

IF @drillmo = 'UptimeDetail'
BEGIN
	--Retrieve downtime in the last 90 days, most recent outages first
	SELECT TOP (@depth)
		UptimeScore AS Score,
		HoursDown AS HoursDown,
		DATEADD(MINUTE, @timezoneOffset, SummaryDayHour) AS SummaryDayHour
	FROM EDDSDBO.QoS_UptimeRatings WITH(NOLOCK)
	WHERE SummaryDayHour > DATEADD(dd, -90, @lastHourProcessed)
		AND HoursDown > 0
	ORDER BY SummaryDayHour DESC
END

IF @drillmo = 'UptimePercentage'
BEGIN
	SELECT
		(SELECT (2160.0 - SUM(HoursDown))*100.0/2160.0
			FROM EDDSDBO.QoS_UptimeRatings WITH(NOLOCK)
			WHERE SummaryDayHour > DATEADD(dd, -90, @lastHourProcessed)
				AND SummaryDayHour <= @lastHourProcessed
		) QuarterlyUptimePercent,
		(SELECT (168.0 - SUM(HoursDown))*100.0/168.0
			FROM EDDSDBO.QoS_UptimeRatings WITH(NOLOCK)
			WHERE SummaryDayHour > DATEADD(dd, -7, @lastHourProcessed)
				AND SummaryDayHour <= @lastHourProcessed
		) WeeklyUptimePercent
END

IF @drillmo = 'ServerInfo'
BEGIN
	SELECT
		S.ArtifactID,
		S.ServerName,
		CASE
			WHEN S.ServerTypeID = 1 THEN 'Web'
			ELSE 'SQL'
		END ServerType,
		ROUND(SS.TotalPhysicalMemory / 1024.0 / 1024.0, 3) MemoryGB,
		SPS.CPUName,
		ROUND(DiskFreeMegabytes / 1024.0, 3) DiskFreeGB
	FROM eddsdbo.[Server] S
	INNER JOIN eddsdbo.[ServerSummary] SS WITH(NOLOCK)
		ON S.ServerID = SS.ServerID
	INNER JOIN eddsdbo.[ServerProcessorSummary] SPS WITH(NOLOCK)
		ON S.ServerID = SPS.ServerID AND SS.MeasureDate = SPS.MeasureDate
	INNER JOIN eddsdbo.[ServerDiskSummary] SDS WITH(NOLOCK)
		ON S.ServerID = SDS.ServerID AND SS.MeasureDate = SDS.MeasureDate
	WHERE SS.MeasureDate = (SELECT TOP 1 MeasureDate FROM eddsdbo.[ServerSummary] SS WITH(NOLOCK) ORDER BY ServerSummaryID DESC)
		AND (S.IgnoreServer IS NULL OR S.IgnoreServer = 0)
		AND S.DeletedOn IS NULL
		AND S.ServerTypeID IN (1, 3)
END

IF @drillmo = 'Servers'
BEGIN
SELECT
	S.ArtifactID,
	S.ServerName
  FROM EDDSPerformance.eddsdbo.[Server] S WITH(NOLOCK)
  WHERE
	S.ServerTypeId = 3
	AND COALESCE(S.IgnoreServer, 0) != 1
	AND S.DeletedOn IS NULL
  GROUP BY S.ArtifactID, S.ServerName
  UNION
  SELECT
	-1 AS ArtifactID, 'Web Servers'
END
END
