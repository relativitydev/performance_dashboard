USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'GetServerDiskSummaryData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO

-- =============================================
-- Author:		Konstantin Kekhaev
-- Create date: 06 October 2011
-- Description:	Getting ServerDiskSummary data 
-- =============================================
-- exec [eddsdbo].[GetServerDiskSummaryData] '2011-09-15','2011-10-04' ,330
ALTER PROCEDURE  [eddsdbo].[GetServerDiskSummaryData]
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
				, ISNULL(sds.DiskSecPerRead, 0) AS DiskAvgSecPerRead
				, ISNULL(sds.DiskSecPerWrite, 0) AS DiskAvgSecPerWrite
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
				, ISNULL(sds.[DiskAvgSecPerRead], 0) AS DiskAvgSecPerRead
				, ISNULL(sds.[DiskAvgSecPerWrite], 0) AS DiskAvgSecPerWrite
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
					   AVG(sds.DiskSecPerRead) [DiskAvgSecPerRead], AVG(sds.DiskSecPerWrite) [DiskAvgSecPerWrite]
				from eddsdbo.ServerDiskSummary sds
				where DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
				group by sds.ServerID, sds.DiskNumber, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
			) as sds on sds.MeasureDate = dr.DateRange and [ServerInfo].ServerID = sds.ServerID and [ServerInfo].DiskNumber = sds.DiskNumber

		END
END

