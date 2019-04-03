USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'GetSQLServerSummaryData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

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
-- Description:	Getting SQLServerSummary data 
-- =============================================
ALTER PROCEDURE  [eddsdbo].[GetSQLServerSummaryData]
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
	
	DECLARE @DatabaseServerTypeId INT = 3	
	
	IF (@StartDate IS NULL AND @EndDate IS NULL)
	BEGIN
		--SELECT @StartDate = DATEADD(HH, - 23, GETUTCDATE())
		SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))
		SET @EndDate = @StartDate			
	END
	
	 -- Insert statements for procedure here
	 IF (@StartDate = @EndDate)		
		 BEGIN		 
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sss.SQLPageLifeExpectancy, -1) AS SQLPageLifeExpectancy
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.SQLServerSummary sss on sss.ServerID = s.ServerID 				
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1
			) as [ServerInfo]
			  left join eddsdbo.SQLServerSummary sss
			  on  DATEADD(MI, @TimeZoneOffset, DATEADD(HOUR, DATEPART(HOUR,MeasureDate), CONVERT(varchar(10), MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))
			  and [ServerInfo].ServerID = sss.ServerID				  
		END
	ELSE
		BEGIN	
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id								
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sss.SQLPageLifeExpectancy, -1) AS SQLPageLifeExpectancy
				, ISNULL(sss.LowMemorySignalStateRatio, 0) AS LowMemorySignalStateRatio						
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.SQLServerSummary sss on sss.ServerID = s.ServerID 
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
			) as [ServerInfo]
			left join (
				select sss.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate],
				 AVG(sss.SQLPageLifeExpectancy) AS SQLPageLifeExpectancy,
				 AVG(sss.LowMemorySignalStateRatio) AS LowMemorySignalStateRatio
				from eddsdbo.SQLServerSummary sss
				where DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
				group by sss.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
			) as sss on sss.MeasureDate = dr.DateRange and [ServerInfo].ServerID = sss.ServerID 
		END	
END

