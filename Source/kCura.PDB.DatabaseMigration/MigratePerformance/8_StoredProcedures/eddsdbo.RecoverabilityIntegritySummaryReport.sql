IF EXISTS (select 1 from sysobjects where [name] = 'RecoverabilityIntegritySummaryReport' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.RecoverabilityIntegritySummaryReport
END
GO

CREATE PROCEDURE eddsdbo.RecoverabilityIntegritySummaryReport
	/* Grid conditions */
	@SortColumn VARCHAR(50) = 'OverallScore',
	@SortDirection CHAR(4) = 'ASC',
	@TimezoneOffset INT = 0, --Offset to use (in minutes) for UTC dates
	@StartRow INT = 1,
	@EndRow INT = 25,
	/* Filter conditions */
	@SummaryDayHour DATETIME = NULL,
	@RecoverabilityIntegrityScore INT = NULL,
	@BackupFrequencyScore INT = NULL,
	@BackupCoverageScore INT = NULL,
	@DbccFrequencyScore INT = NULL,
	@DbccCoverageScore INT = NULL,
	@RPOScore INT = NULL,
	@RTOScore INT = NULL,
	/* Filter operands */
	@RecoverabilityIntegrityScoreOperand NVARCHAR(2) = '=',
	@BackupFrequencyScoreOperand NVARCHAR(2) = '=',
	@BackupCoverageScoreOperand NVARCHAR(2) = '=',
	@DbccFrequencyScoreOperand NVARCHAR(2) = '=',
	@DbccCoverageScoreOperand NVARCHAR(2) = '=',
	@RPOScoreOperand NVARCHAR(2) = '=',
	@RTOScoreOperand NVARCHAR(2) = '=',
	/* Time range */
	@StartHour DATETIME = NULL,
	@EndHour DATETIME = NULL
AS
BEGIN
	--Declarations
	DECLARE @SQL NVARCHAR(MAX) = N'',
		@Where NVARCHAR(MAX) = N'';
	
	--Massage start/end dates
	IF (@SummaryDayHour IS NOT NULL)
	BEGIN
		SET @SummaryDayHour = DATEADD(MINUTE, -1 * @TimezoneOffset, @SummaryDayHour);
	END
	
	-- Filter Sort Params
	IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
	BEGIN
		SET @SortDirection = 'ASC'
	END 
 
	IF LOWER(@SortColumn) NOT IN (N'summarydayhour', N'recoverabilityintegrityscore', N'overallscore', N'backupfrequencyscore', N'backupcoveragescore', N'dbccfrequencyscore', N'dbcccoveragescore', N'rposcore', N'rtoscore')
	BEGIN
		SET @SortColumn = 'OverallScore'
	END
	
	--Handle ambiguous sort columns
	IF (LOWER(@SortColumn) = 'recoverabilityintegrityscore')
		SET @SortColumn = 'OverallScore'
	IF (LOWER(@SortColumn) = 'summarydayhour')
		SET @SortColumn = 'h.[HourTimeStamp]'
	
	--Build SQL
	IF (@SummaryDayHour IS NOT NULL)
		SET @Where += '
		AND h.HourTimeStamp = @SummaryDayHour'
	IF (@RecoverabilityIntegrityScore IS NOT NULL)
		SET @Where += '
		AND CAST(OverallScore AS INT) ' + @RecoverabilityIntegrityScoreOperand + ' @RecoverabilityIntegrityScore';
	IF (@BackupFrequencyScore IS NOT NULL)
		SET @Where += '
		AND CAST(BackupFrequencyScore AS INT) ' + @BackupFrequencyScoreOperand + ' @BackupFrequencyScore';
	IF (@BackupCoverageScore IS NOT NULL)
		SET @Where += '
		AND CAST(BackupCoverageScore AS INT) ' + @BackupCoverageScoreOperand + ' @BackupCoverageScore';
	IF (@DbccFrequencyScore IS NOT NULL)
		SET @Where += '
		AND CAST(DbccFrequencyScore AS INT) ' + @DbccFrequencyScoreOperand + ' @DbccFrequencyScore';
	IF (@DbccCoverageScore IS NOT NULL)
		SET @Where += '
		AND CAST(DbccCoverageScore AS INT) ' + @DbccCoverageScoreOperand + ' @DbccCoverageScore';
	IF (@RPOScore IS NOT NULL)
		SET @Where += '
		AND CAST(RPOScore AS INT) ' + @RPOScoreOperand + ' @RPOScore';
	IF (@RTOScore IS NOT NULL)
		SET @Where += '
		AND CAST(RTOScore AS INT) ' + @RTOScoreOperand + ' @RTOScore';
	IF (@StartHour IS NOT NULL AND @EndHour IS NOT NULL)
		SET @Where += '
		AND h.HourTimeStamp BETWEEN @StartHour AND @EndHour';


	SET @SQL = N'
	DECLARE @Data TABLE
	(
		[RowNumber] INT,
		[TotalRows] INT,
		[SummaryDayHour] DATETIME,
		[RecoverabilityIntegrityScore] INT,
		[BackupFrequencyScore] INT,
		[BackupCoverageScore] INT,
		[DbccFrequencyScore] INT,
		[DbccCoverageScore] INT,
		[RPOScore] INT,
		[RTOScore] INT,
		[WorstRPODatabase] NVARCHAR(255),
		[WorstRTODatabase] NVARCHAR(255),
		[PotentialDataLossMinutes] INT,
		[EstimatedTimeToRecoverHours] INT
	);

	DECLARE @totalRows INT = (
		SELECT
			COUNT(*)
		FROM eddsdbo.Reports_RecoverabilityIntegritySummary ris WITH(NOLOCK)
		INNER JOIN eddsdbo.[Hours] h WITH(NOLOCK) on ris.HourId = h.Id
		WHERE 1 = 1
		' + ISNULL(@Where, '') + '
	);
	
	WITH Paging AS
	(
	SELECT
		ROW_NUMBER() OVER (ORDER BY	' + @SortColumn + ' ' + @SortDirection + ') AS RowNumber,
		@totalRows TotalRows,
		DATEADD(MINUTE, @TimezoneOffset, h.[HourTimeStamp]) SummaryDayHour,
		[OverallScore] as RecoverabilityIntegrityScore,
		[BackupFrequencyScore],
		[BackupCoverageScore],
		[DbccFrequencyScore],
		[DbccCoverageScore],
		[RPOScore],
		[RTOScore],		
		[WorstRPODatabase],
		[WorstRTODatabase],
		[RpoMaxDataLoss] as [PotentialDataLossMinutes],
		[RtoTimeToRecover] as [EstimatedTimeToRecoverHours]
	FROM eddsdbo.Reports_RecoverabilityIntegritySummary ris WITH(NOLOCK)
	INNER JOIN eddsdbo.[Hours] h WITH(NOLOCK) on ris.HourId = h.Id
	INNER JOIN eddsdbo.Reports_RecoverabilityIntegrityRpoSummary rp WITH(NOLOCK) on ris.HourId = rp.HourId
	INNER JOIN eddsdbo.Reports_RecoverabilityIntegrityRtoSummary rt WITH(NOLOCK) on ris.HourId = rt.HourId
	WHERE 1 = 1
	' + ISNULL(@Where, '') + '
	)
	INSERT INTO @Data
	SELECT *
	FROM Paging
	WHERE RowNumber BETWEEN @StartRow AND @EndRow
	
	SELECT
		[RowNumber],
		[SummaryDayHour],
		[RecoverabilityIntegrityScore],
		[BackupFrequencyScore],
		[BackupCoverageScore],
		[DbccFrequencyScore],
		[DbccCoverageScore],
		[RPOScore],
		[RTOScore],
		[WorstRPODatabase],
		[WorstRTODatabase],
		[PotentialDataLossMinutes],
		[EstimatedTimeToRecoverHours]
	FROM @Data
	
	SELECT TOP 1
		@StartRow AS StartIndex,
		@StartRow + @@ROWCOUNT - 1 AS EndIndex,
		TotalRows AS FilteredCount
	FROM @Data';

	PRINT @SQL;
	
	EXEC sp_executesql @SQL,
		N'@StartRow INT, @EndRow INT, @TimezoneOffset INT,
		  @SummaryDayHour DATETIME, @RecoverabilityIntegrityScore INT, @BackupFrequencyScore INT, @BackupCoverageScore INT,
		  @DbccFrequencyScore INT, @DbccCoverageScore INT, @RPOScore INT, @RTOScore INT, @StartHour DATETIME, @EndHour DATETIME',
		@StartRow,
		@EndRow,
		@TimezoneOffset,
		@SummaryDayHour,
		@RecoverabilityIntegrityScore,
		@BackupFrequencyScore,
		@BackupCoverageScore,
		@DbccFrequencyScore,
		@DbccCoverageScore,
		@RPOScore,
		@RTOScore,
		@StartHour,
		@EndHour;
END