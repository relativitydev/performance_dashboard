IF EXISTS (select 1 from sysobjects where [name] = 'QoS_UserExperienceWorkspaceReport' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_UserExperienceWorkspaceReport
END
GO
CREATE PROCEDURE EDDSDBO.QoS_UserExperienceWorkspaceReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'TotalRuns',
	@SortDirection CHAR(4) = 'DESC',
	@TimeZoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@SummaryDayHour DATETIME = NULL,
	@Server INT, --ArtifactID of the server we want data for
	@Workspace NVARCHAR(150) = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@TotalRunTime BIGINT = NULL,
	@AverageRunTime INT = NULL,
	@TotalRuns INT = NULL,
	@IsComplex BIT = NULL,
	@IsActiveArrivalRateSample BIT = NULL,
	/* Filter operands */
	@TotalRunTimeOperand NVARCHAR(2) = '=',
	@AverageRunTimeOperand NVARCHAR(2) = '=',
	@TotalRunsOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'',
		@SearchId INT = NULL;
	
	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'ASC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'totalruns', N'summarydayhour', N'databasename', N'searchname', N'iscomplex', N'totallrqruntime', N'averageruntime', N'isactiveweeklysample')
	BEGIN
		SET @SortColumn = 'TotalRuns'
	END
	
	--Handle ambiguous sort columns
	IF (@SortColumn = 'AverageRunTime')
		SET @SortColumn = 'qsvodc2.TotalLRQRunTime/qsvodc2.TotalRuns';
	IF (@SortColumn = 'SummaryDayHour')
		SET @SortColumn = 'H.HourTimeStamp';
	IF (LOWER(@SortColumn) = 'isactiveweeklysample')
		SET @SortColumn = 'IsActiveArrivalRateSample'

	--Support ArtifactID filtering
	IF (ISNUMERIC(@Search) = 1)
		SET @SearchId = CAST(@Search as int);

	--Prepare string filter inputs
	SET @Workspace = '%' + REPLACE(@Workspace, '[', '[[]') + '%';
	SET @Search = '%' + REPLACE(@Search, '[', '[[]') + '%';
	
	--Massage start/end dates
	IF (@SummaryDayHour IS NOT NULL)
	BEGIN
		SET @SummaryDayHour = DATEADD(MINUTE, -1 * @TimezoneOffset, @SummaryDayHour);
		IF (@StartHour IS NULL OR @StartHour < @SummaryDayHour)
			SET @StartHour = @SummaryDayHour;
		IF (@EndHour IS NULL OR @SummaryDayHour < @EndHour)
			SET @EndHour = @SummaryDayHour;
	END
	
	--Build SQL
	IF (@Workspace IS NOT NULL)
		SET @Where += '
		AND DatabaseName LIKE @Workspace';
	IF (@Search IS NOT NULL)
		SET @Where += '
		AND (SearchName LIKE @Search OR SearchArtifactID = @SearchId)'
	IF (@TotalRunTime IS NOT NULL)
		SET @Where += '
		AND qsvodc2.TotalLRQRunTime ' + @TotalRunTimeOperand + ' @TotalRunTime';
	IF (@AverageRunTime IS NOT NULL)
		SET @Where += '
		AND qsvodc2.TotalLRQRunTime/qsvodc2.TotalRuns ' + @AverageRunTimeOperand + ' @AverageRunTime';
	IF (@TotalRuns IS NOT NULL)
		SET @Where += '
		AND ISNULL(TotalRuns, 1) ' + @TotalRunsOperand + ' @TotalRuns';
	IF (@IsComplex IS NOT NULL)
		SET @Where += '
		AND IsComplex = @IsComplex';
	IF (@StartHour IS NOT NULL AND @EndHour IS NOT NULL)
		SET @Where += '
		AND h.[HourTimeStamp] BETWEEN @StartHour AND @EndHour';
	IF (@IsActiveArrivalRateSample IS NOT NULL)
		SET @Where += '
		AND IsActiveArrivalRateSample = @IsActiveArrivalRateSample';
	
	SET @SQL = N'
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.QoS_VarscatOutputCumulative AS qsvodc2
		INNER JOIN eddsdbo.Hours h
			ON qsvodc2.SummaryDayHour = h.HourTimeStamp and h.Status != 4
        INNER JOIN eddsdbo.QoS_SampleHistoryUX AS sh 
			ON sh.ServerId = qsvodc2.ServerID 
			AND sh.HourId = h.Id
		INNER JOIN eddsdbo.Server s WITH(NOLOCK)
			ON sh.ServerId = s.ServerID
		CROSS APPLY ( SELECT
								          CAST(MIN(CAST(qsvodc.IsComplex as tinyint)) as bit) isComplex
                                      FROM      eddsdbo.QoS_VarscatOutputDetailCumulative
                                                AS qsvodc
                                      WHERE     qsvodc.ServerID = qsvodc2.ServerID
												AND qsvodc.SummaryDayHour = qsvodc2.SummaryDayHour
												AND qsvodc.SearchArtifactID = qsvodc2.SearchArtifactID
												AND ''EDDS'' + CAST(qsvodc.CaseArtifactID as varchar) = qsvodc2.DatabaseName
                                                AND qsvodc.QoSAction IN ( 281, 282, 283 )
                                    ) ca ( isComplex )
        WHERE h.HourTimeStamp > DATEADD(dd, -90, getutcdate())
			AND s.[ArtifactID] = @Server'
			+ ISNULL(@Where, '') + '
	)

;WITH PAGING AS (
						SELECT
                        qsvodc2.kVOID ,
                        DATEADD(MINUTE, @TimezoneOffset, h.HourTimeStamp) SummaryDayHour ,
                        qsvodc2.DatabaseName,
                        qsvodc2.SearchName ,
                        qsvodc2.SearchArtifactID ,
                        ca.isComplex ,
                        qsvodc2.TotalLRQRunTime TotalRunTime ,
						qsvodc2.TotalLRQRunTime/qsvodc2.TotalRuns averageRunTime ,
                        ISNULL(qsvodc2.TotalRuns, 1) TotalRuns,
                        sh.IsActiveArrivalRateSample,
						ROW_NUMBER() OVER (ORDER BY ' + @SortColumn + ' ' + @SortDirection + ') RowNumber,
						@totalRows FilteredCount
               FROM     eddsdbo.QoS_VarscatOutputCumulative AS qsvodc2
						INNER JOIN eddsdbo.Hours h
							ON qsvodc2.SummaryDayHour = h.HourTimeStamp and h.Status != 4
                        INNER JOIN eddsdbo.QoS_SampleHistoryUX AS sh 
							ON qsvodc2.ServerID = sh.ServerId
							AND h.Id = sh.HourId
						INNER JOIN eddsdbo.Server s WITH(NOLOCK)
							ON sh.ServerId = s.ServerID
                        CROSS APPLY ( SELECT
								          CAST(MIN(CAST(qsvodc.IsComplex as tinyint)) as bit) isComplex
                                      FROM      eddsdbo.QoS_VarscatOutputDetailCumulative
                                                AS qsvodc
                                      WHERE     qsvodc.ServerID = qsvodc2.ServerID
												AND qsvodc.SummaryDayHour = qsvodc2.SummaryDayHour
												AND qsvodc.SearchArtifactID = qsvodc2.SearchArtifactID
												AND ''EDDS'' + CAST(qsvodc.CaseArtifactID as varchar) = qsvodc2.DatabaseName
                                                AND qsvodc.QoSAction IN ( 281, 282, 283 )
                                    ) ca ( isComplex )
               WHERE h.HourTimeStamp > DATEADD(dd, -90, getutcdate())
               AND s.[ArtifactID] = @Server'
	+ ISNULL(@Where, '') + '
	),
        PageFilter
          AS ( SELECT   p.kVOID
               FROM     PAGING p
               WHERE    p.RowNumber BETWEEN @StartRow AND @EndRow
             )
    SELECT  pa.RowNumber,
			pa.FilteredCount,
			pa.DatabaseName,
			pa.SearchArtifactID ,
            pa.SearchName ,
            pa.TotalRunTime ,
            pa.AverageRunTime ,
            pa.TotalRuns ,
            pa.IsComplex ,
            pa.SummaryDayHour,
            pa.IsActiveArrivalRateSample
    INTO #data
    FROM PAGING pa
    JOIN PageFilter pf ON pf.kVOID = pa.kVOID
	
	SELECT * FROM #data;
	
	SELECT TOP 1 @StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		FilteredCount
	FROM #data;';
	
	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT, @Server INT, @Workspace NVARCHAR(MAX), @Search NVARCHAR(MAX), @SearchId INT, @TotalRunTime BIGINT, @AverageRunTime INT, @TotalRuns INT, @IsComplex BIT, @IsActiveArrivalRateSample BIT, @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@Server,
		@Workspace,
		@Search,
		@SearchId,
		@TotalRunTime,
		@AverageRunTime,
		@TotalRuns,
		@IsComplex,
		@IsActiveArrivalRateSample,
		@StartHour,
		@EndHour;
END