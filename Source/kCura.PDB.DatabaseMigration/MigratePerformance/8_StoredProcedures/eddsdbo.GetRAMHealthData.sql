USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'GetRAMHealthData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

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
-- Description:	Getting ServerSummary data 
-- =============================================
-- exec [eddsdbo].[GetRAMHealthData] null, null , -300
-- exec [eddsdbo].[GetRAMHealthData] '2011-09-01', '2011-10-01' , -300
ALTER PROCEDURE  [eddsdbo].[GetRAMHealthData]
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
		 SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))		
		 SET @EndDate = @StartDate
	END	
	 --select @StartDate
    -- Insert statements for procedure here
	 IF (@StartDate = @EndDate)		
		 BEGIN		 
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(ss.RAMPagesPerSec,-1) as RAMPagesPerSec 
				, ISNULL(ss.RAMPageFaultsPerSec,-1) as RAMPageFaultsPerSec
				, ISNULL(ss.RAMPct,-1) AS RAMPct	
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.ServerSummary ss on ss.ServerID = s.ServerID 								
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1				
			) as [ServerInfo]
			  left join eddsdbo.ServerSummary ss
			  on  DATEADD(MI, @TimeZoneOffset, DATEADD(HOUR, DATEPART(HOUR,MeasureDate), CONVERT(varchar(10), ss.MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))				    			  
			  and [ServerInfo].ServerID = ss.ServerID				  
			  
		END
	ELSE
		BEGIN	
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id								
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(SS.RAMPagesPerSec,-1) AS RAMPagesPerSec
				, ISNULL(SS.RAMPageFaultsPerSec,-1) AS RAMPageFaultsPerSec
				, ISNULL(SS.RAMPct,-1) AS RAMPct
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.ServerSummary SS on SS.ServerID = s.ServerID 
				AND DATEADD(MI, @TimeZoneOffset,MeasureDate) >=  @StartDate and DATEADD(MI, @TimeZoneOffset,MeasureDate)  < (@EndDate + 1)
			) as [ServerInfo]
			left join (
				select SS.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate],
				 AVG(SS.RAMPagesPerSec) AS RAMPagesPerSec , AVG(SS.RAMPageFaultsPerSec) AS RAMPageFaultsPerSec, AVG(SS.RAMPct) AS RAMPct
				from eddsdbo.ServerSummary SS
				where DATEADD(MI, @TimeZoneOffset,MeasureDate) >=  @StartDate and DATEADD(MI, @TimeZoneOffset,MeasureDate)  < (@EndDate + 1)
				group by SS.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
			) as SS on SS.MeasureDate = dr.DateRange and [ServerInfo].ServerID = SS.ServerID 
		END	
END

