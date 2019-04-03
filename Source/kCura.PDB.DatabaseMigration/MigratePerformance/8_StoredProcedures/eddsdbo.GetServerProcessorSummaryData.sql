USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'GetServerProcessorSummaryData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO

-- =============================================
-- Author:		Jayesh Dhobi
-- Create date: 25th August 2011
-- Description:	Getting ServerProcessorSummary data 
-- =============================================
-- exec [eddsdbo].[GetServerProcessorSummaryData] '9/22/2011 12:00:00 AM','9/22/2011 12:00:00 AM',0
-- exec [eddsdbo].[GetServerProcessorSummaryData] null, null,0
ALTER PROCEDURE  [eddsdbo].[GetServerProcessorSummaryData]
(
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset INT	-- the difference between local time at the client and GMT (in minutes)
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
				, [ServerInfo].ServerID * 10 as ServerCoreId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sps.CPUProcessorTimePct, -1) AS CPUProcessorTimePct
			FROM [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
				CROSS JOIN 
				(
					SELECT DISTINCT 
						s.ServerID
						, sps.CoreNumber
						, s.ServerName
						, st.ServerTypeName
					FROM eddsdbo.Server s
						JOIN eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
						JOIN eddsdbo.ServerProcessorSummary sps on sps.ServerID = s.ServerID 
							AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate 
							AND DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1
					WHERE sps.CoreNumber = -1
				) as [ServerInfo]
				LEFT JOIN eddsdbo.ServerProcessorSummary sps
				  on  DATEADD(MI, @TimeZoneOffset,DATEADD(HOUR, DATEPART(HOUR, MeasureDate), CONVERT(varchar(10), MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))
				  AND [ServerInfo].ServerID = sps.ServerID
				  AND [ServerInfo].CoreNumber = sps.CoreNumber
		END
	ELSE
		BEGIN
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID * 10 as ServerCoreId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sps.CPUProcessorTimePct, -1) AS CPUProcessorTimePct
			FROM [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
				CROSS JOIN 
				(
					SELECT DISTINCT 
						s.ServerID
						, sps.CoreNumber
						, s.ServerName
						, st.ServerTypeName
					FROM eddsdbo.Server s
						JOIN eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
						JOIN eddsdbo.ServerProcessorSummary sps on sps.ServerID = s.ServerID 
							AND DATEADD(MI, @TimeZoneOffset,MeasureDate) >= @StartDate 
							AND DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
					WHERE sps.CoreNumber = -1
				) as [ServerInfo]
				LEFT JOIN 
				(
					SELECT 
						sps.ServerID
						, sps.CoreNumber
						, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate]
						, AVG(sps.CPUProcessorTimePct) [CPUProcessorTimePct]
					FROM eddsdbo.ServerProcessorSummary sps
					WHERE DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate 
						AND DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
					GROUP BY sps.ServerID
						, sps.CoreNumber
						, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
				) as sps on sps.MeasureDate = dr.DateRange 
					AND [ServerInfo].ServerID = sps.ServerID 
					AND [ServerInfo].CoreNumber = sps.CoreNumber
		END
END

