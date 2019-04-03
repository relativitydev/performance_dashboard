USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'GetApplicationHealthData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO


ALTER PROCEDURE  [eddsdbo].[GetApplicationHealthData]

	@StartDate	DATETIME = NULL, --This is the start date from the page's date control (local time)
	@EndDate	DATETIME = NULL, --This is the end date from the page's date control (local time)
	@TimeZoneOffset int --This is the number of minutes we need to add to UTC to get local time

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	/*
		7/29/2014 - Joseph Low -
			Removed vestigial latency data
	
		7/15/2014 - Jacob Malliet -
			Removed BISSummary details from the script as we no longer need this information being pulled

		7/7/2014 - Joseph Low -
			Using a LEFT JOIN on BISSummary in case where Start = End so data is still returned
			Fixing some timezone conversion discrepancies
	
		5/8/2014 - Joseph Low -
			Removed BISIndicator

		10/31/2013 - Ryan Flint -  
			Fixed condition when @StartDate != @EndDate by changing Where clause 
			on the dates from a BETWEEN to explicit >= AND <
	*/
	
	
	SET NOCOUNT ON;

	DECLARE @SQLTimeZoneOffset INT
	Set @SQLTimeZoneOffset  = DATEDIFF(MINUTE, GETUTCDATE(), GETDATE()) 

	IF (@StartDate IS NULL AND @EndDate IS NULL OR @StartDate = CAST(DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE()) as DATE))
	BEGIN
		--Default to the past 24 hours when date parameters are null
		IF(@StartDate IS NULL AND @EndDate IS NULL)
		BEGIN
			SELECT @StartDate = DATEADD(HH,-24,DATEADD(HH, DATEDIFF(HH, 0, GETUTCDATE()) ,0))					
			SELECT @EndDate = DATEADD(HH, 0, DATEADD(HH, DATEDIFF(HH, 0, GETUTCDATE()) ,0))
		END
		ELSE	-- Pull data since midnight today
		BEGIN
			--If we're specifying a local time for @StartDate, convert it to UTC
			SET @StartDate = DATEADD(MINUTE, -@TimeZoneOffset, @StartDate)
			SET @EndDate = DATEADD(DD, 1, @StartDate)
		END
		
		SELECT
			ROW_NUMBER() OVER(ORDER BY ps.CaseArtifactID, ps.measureDate) AS Id				
			, ISNULL(ps.CaseArtifactID,0) as CaseArtifactID
			, ISNULL(w.WorkspaceName,'') as WorkspaceName				
			, ISNULL(w.[DatabaseLocation],'') as [DatabaseLocation]
			, DATEADD(MINUTE, @timezoneoffset, ps.MeasureDate) as MeasureDate --convert to local time
			, ISNULL(PS.UserCount, -1) AS UserCount
			, ISNULL(PS.ErrorCount, -1) AS ErrorCount
			, ISNULL(PS.LRQCount, -1) AS LRQCount
			FROM [EDDSPerformance].[eddsdbo].[PerformanceSummary] ps
			JOIN eddsperformance.eddsdbo.EDDSWorkspace w
			ON ps.CaseArtifactID = w.CaseArtifactID
			LEFT JOIN EDDS.eddsdbo.CaseStatistics CS
				ON cs.timestamp = ps.MeasureDate			    			  
				AND ps.CaseArtifactID = CS.CaseArtifactID	
			--Here, @StartDate and @EndDate are UTC, so we can compare safely with MeasureDate
			WHERE ps.MeasureDate between @StartDate and @EndDate		
	END

	ELSE
	BEGIN
	--When start and end are the same, get data from StartDate to StartDate + 24 hours
	IF (@StartDate = @EndDate AND @endDate != DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))		
	BEGIN
		SET @EndDate = DATEADD(DD, 1, @StartDate)		
		
		SELECT
			ROW_NUMBER() OVER(ORDER BY ps.CaseArtifactID, ps.measureDate) AS Id				
			, ISNULL(ps.CaseArtifactID,0) as CaseArtifactID
			, ISNULL(w.WorkspaceName,'') as WorkspaceName				
			, ISNULL(w.[DatabaseLocation],'') as [DatabaseLocation]
			, DATEADD(MINUTE, @timezoneoffset, ps.[MeasureDate]) as MeasureDate	--convert display date to local time
			, ISNULL(PS.UserCount, -1) AS UserCount
			, ISNULL(PS.ErrorCount, -1) AS ErrorCount
			, ISNULL(PS.LRQCount, -1) AS LRQCount
			FROM [EDDSPerformance].[eddsdbo].[PerformanceSummary] ps
			JOIN eddsperformance.eddsdbo.EDDSWorkspace w
			ON ps.CaseArtifactID = w.CaseArtifactID	
			--Take UTC hours that will result in midnight-to-midnight range after local time conversion
			--@StartDate and @EndDate are local times, but MeasureDate is in UTC. To compare, we have to take out the offset.
			WHERE ps.MeasureDate >= DATEADD(MINUTE, -@TimeZoneOffset, @StartDate)
			AND ps.MeasureDate < DATEADD(MINUTE, -@TimeZoneOffset, @EndDate)
	END
	ELSE		
	BEGIN	
		SELECT
			ROW_NUMBER() OVER(ORDER BY ps.CaseArtifactID, ps.measureDate) AS Id				
			,ps.[CaseArtifactID]
			,w.WorkspaceName
			,w.DatabaseLocation
			,ps.MeasureDate
			,ps.[UserCount]
			,ps.[ErrorCount]
			,ps.[LRQCount]		
			FROM 
			(
				SELECT 
					P.CaseArtifactID
					, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, p.MeasureDate))) [MeasureDate] --convert display date to local time
					, MAX(P.UserCount) AS UserCount
					, SUM(P.ErrorCount) AS ErrorCount
					, SUM(P.LRQCount) AS LRQCount
				FROM eddsdbo.PerformanceSummary P
				WHERE
					--Take UTC hours that will result in midnight-to-midnight range after local time conversion
					--@StartDate and @EndDate are local times, but MeasureDate is in UTC. To compare, we have to take out the offset.
					p.MeasureDate >= DATEADD(MINUTE, -@TimeZoneOffset, @StartDate) 
					AND p.MeasureDate < DATEADD(MINUTE, -@TimeZoneOffset, @EndDate + 1)
				GROUP BY P.CaseArtifactID
					, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, p.MeasureDate )))
			) as ps
			JOIN eddsperformance.eddsdbo.EDDSWorkspace w
			ON ps.CaseArtifactID = w.CaseArtifactID
	END	
	END
END


