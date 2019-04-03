USE [EDDSPerformance]
GO
/****** Object:  StoredProcedure [eddsdbo].[GetServerDiskSummaryDataHourly]    Script Date: 02/12/2013 13:06:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [eddsdbo].[GetServerDiskSummaryDataHourly]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@TimeZoneOffset INT	-- the difference between local time at the client and GMT (in minutes)
)
AS
BEGIN

	SELECT
		ROW_NUMBER() OVER(ORDER BY [ServerDiskSummary].ServerID, 
		 DATEADD(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate)		
		) AS Id
		,convert(integer, convert(varchar, [ServerDiskSummary].ServerID)+convert(varchar,ServerDiskSummary.DiskNumber)) as ServerDiskId
		, [ServerDiskSummary].ServerID
		, [Server].ServerName
		, [ServerType].ServerTypeName
		, DATEADD(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate) as MeasureDate
		, ServerDiskSummary.DiskNumber AS DiskNumber
		, ServerDiskSummary.DriveLetter AS DriveLetter
		, ServerDiskSummary.DiskAvgSecPerRead AS DiskAvgSecPerRead
		, ServerDiskSummary.DiskAvgSecPerWrite AS DiskAvgSecPerWrite
	FROM eddsdbo.ServerDiskSummary AS [ServerDiskSummary]
		INNER JOIN eddsdbo.Server AS [Server] ON ServerDiskSummary.ServerID = [Server].ServerID
		INNER JOIN eddsdbo.ServerType AS [ServerType] ON [Server].ServerTypeID = [ServerType].ServerTypeID
	WHERE( DATEADD(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate) >= @StartDate 
	AND DATEADD(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate) < @EndDate)
	
END
GO
/****** Object:  StoredProcedure [eddsdbo].[GetServerDiskSummaryDataDaily]    Script Date: 02/12/2013 13:06:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [eddsdbo].[GetServerDiskSummaryDataDaily]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@TimeZoneOffset INT	-- the difference between local time at the client and GMT (in minutes)
)
AS
BEGIN

	SELECT
		ROW_NUMBER() OVER(ORDER BY [ServerDiskSummary].ServerID, [Server].ServerName, [ServerType].ServerTypeName, 
		CONVERT(VARCHAR(10), dateadd(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate), 101)
		,ServerDiskSummary.DiskNumber) AS Id
		,convert(integer, convert(varchar, [ServerDiskSummary].ServerID)+convert(varchar,ServerDiskSummary.DiskNumber)) as ServerDiskId
		, [ServerDiskSummary].ServerID
		, [Server].ServerName
		, [ServerType].ServerTypeName
		, CAST( CONVERT(VARCHAR(10), dateadd(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate), 101) AS DATETIME) AS MeasureDate			
		, ServerDiskSummary.DiskNumber
		, ServerDiskSummary.DriveLetter 
		, AVG( ServerDiskSummary.DiskAvgSecPerRead ) AS DiskAvgSecPerRead
		, AVG( ServerDiskSummary.DiskAvgSecPerWrite ) AS DiskAvgSecPerWrite
	FROM eddsdbo.ServerDiskSummary AS [ServerDiskSummary]
		INNER JOIN eddsdbo.Server AS [Server] ON ServerDiskSummary.ServerID = [Server].ServerID
		INNER JOIN eddsdbo.ServerType AS [ServerType] ON [Server].ServerTypeID = [ServerType].ServerTypeID
	WHERE
	( CAST( CONVERT(VARCHAR(10), DATEADD(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate), 101) AS DATETIME) >= @StartDate 
		AND CAST( CONVERT(VARCHAR(10), DATEADD(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate), 101) AS DATETIME) < (@EndDate + 1) )
	
	GROUP BY [ServerDiskSummary].ServerID
		, [Server].ServerName
		, [ServerType].ServerTypeName
		, ServerDiskSummary.DiskNumber 
		, ServerDiskSummary.DriveLetter
		, CONVERT(VARCHAR(10), DATEADD(MI, @TimeZoneOffset, [ServerDiskSummary].MeasureDate), 101)
	
END
GO
/****** Object:  StoredProcedure [eddsdbo].[GetServerDiskSummaryData]    Script Date: 02/12/2013 13:06:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Konstantin Kekhaev
-- Create date: 06 October 2011
-- Description:	Getting ServerDiskSummary data 
-- =============================================
-- exec [eddsdbo].[GetServerDiskSummaryData] '2011-09-15','2011-10-04' ,330
ALTER PROCEDURE [eddsdbo].[GetServerDiskSummaryData]
(
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset int 
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	IF (@StartDate IS NULL AND @EndDate IS NULL)
	BEGIN
		-- hourly
		--SELECT @StartDate = DATEADD(HH, - 23, GETUTCDATE())
		SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))				
		SET @EndDate = @StartDate			
	END
	
    -- Insert statements for procedure here
     IF (@StartDate = @EndDate)		
		BEGIN
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID * 10 + [ServerInfo].DiskNumber as ServerDiskId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, [ServerInfo].DiskNumber
				, [ServerInfo].DriveLetter
				, ISNULL(sds.DiskAvgSecPerRead, -1) AS DiskAvgSecPerRead
				, ISNULL(sds.DiskAvgSecPerWrite, -1) AS DiskAvgSecPerWrite
				from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
				cross join (
					select distinct s.ServerID, sds.DiskNumber, sds.DriveLetter, s.ServerName, st.ServerTypeName
					from eddsdbo.Server s
					join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
					join eddsdbo.ServerDiskSummary sds on sds.ServerID = s.ServerID 
					AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1
				) as [ServerInfo]
				left join eddsdbo.ServerDiskSummary sds
				  on  DATEADD(MI, @TimeZoneOffset, DATEADD(HOUR, DATEPART(HOUR,MeasureDate), CONVERT(varchar(10), MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))
				  and [ServerInfo].ServerID = sds.ServerID
				  and [ServerInfo].DiskNumber = sds.DiskNumber
		END
	ELSE
		BEGIN
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange, [ServerInfo].DiskNumber) AS Id				
				, [ServerInfo].ServerID * 10 + [ServerInfo].DiskNumber as ServerDiskId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, [ServerInfo].DiskNumber
				, [ServerInfo].DriveLetter
				, ISNULL(sds.DiskAvgSecPerRead, -1) AS DiskAvgSecPerRead
				, ISNULL(sds.DiskAvgSecPerWrite, -1) AS DiskAvgSecPerWrite
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, sds.DiskNumber, sds.DriveLetter, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.ServerDiskSummary sds on sds.ServerID = s.ServerID 
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
			) as [ServerInfo]
			left join (
				select sds.ServerID, sds.DiskNumber, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate],
					   AVG(sds.DiskAvgSecPerRead) [DiskAvgSecPerRead], AVG(sds.DiskAvgSecPerWrite) [DiskAvgSecPerWrite]
				from eddsdbo.ServerDiskSummary sds
				where DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
				group by sds.ServerID, sds.DiskNumber, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
			) as sds on sds.MeasureDate = dr.DateRange and [ServerInfo].ServerID = sds.ServerID and [ServerInfo].DiskNumber = sds.DiskNumber

		END
END
GO
