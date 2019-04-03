USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'LoadApplicationHealthSummary', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO

-- =============================================
-- Author:		Murali Shesham
-- Create date: 08/31/2011
-- Description:	
-- 2012-05-30 : Ron @ Milyli : Altered to add new fields
-- 2013-02-15 : Ron @ Milyli : Added new TotalNRQCount (total non-relational queries)
-- =============================================
ALTER PROCEDURE  [eddsdbo].[LoadApplicationHealthSummary]
	@ProcessExecDate DateTime 
AS
BEGIN
	SET NOCOUNT ON;
	
	--Measure the previous hour
	DECLARE @SummaryDayHour DATETIME = DATEADD(hh,DATEDIFF(hh, 0, @ProcessExecDate)-1, 0);
	DECLARE @MeasureDate DATE = CAST(@SummaryDayHour AS DATE);
	DECLARE @MeasureHour INT = DATEPART(HH,@SummaryDayHour);

	--This procedure now depends on VARSCAT output
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND type in (N'U'))
		RETURN;

	IF EXISTS(Select 1 From eddsdbo.PerformanceSummary Where MeasureDate = @SummaryDayHour)
	BEGIN
		PRINT 'Cannot load data more than once in an hour'
		UPDATE PS
		SET LRQCount = COALESCE(LC.LRQCount, 0)
		FROM [eddsdbo].[PerformanceSummary] AS PS  
		LEFT JOIN (
			SELECT CaseArtifactID,
				SummaryDayHour MeasureDate,
				COUNT(CaseArtifactID) LRQCount
			FROM eddsdbo.QoS_VarscatOutputDetailCumulative
			WHERE IsLongRunning = 1
			GROUP BY CaseArtifactID, SummaryDayHour
		) AS LC
		ON PS.CaseArtifactID = LC.CaseArtifactID 
		AND LC.MeasureDate = @SummaryDayHour
		WHERE PS.MeasureDate = @SummaryDayHour
	END
	ELSE
	BEGIN
		BEGIN TRAN
			IF DATEPART(HOUR, GETDATE()) = 2 -- 2AM local SQL time
			BEGIN
				PRINT 'Updating PerformanceSummary table'
				UPDATE PS
				SET LRQCount = COALESCE(LC.LRQCount, 0)
				FROM [eddsdbo].[PerformanceSummary] AS PS
				LEFT JOIN (
					SELECT CaseArtifactID,
						SummaryDayHour MeasureDate,
						COUNT(CaseArtifactID) LRQCount
					FROM eddsdbo.QoS_VarscatOutputDetailCumulative
					WHERE IsLongRunning = 1
					GROUP BY CaseArtifactID, SummaryDayHour
				) AS LC
				ON PS.CaseArtifactID = LC.CaseArtifactID 
				AND LC.MeasureDate = PS.MeasureDate
			END
		
 			INSERT INTO eddsdbo.PerformanceSummary
			SELECT 
			GetUTCDate() CreatedOn,
			C.ArtifactID CaseArtifactID, 	
			@SummaryDayHour MeasureDate, 
			COALESCE(UC.UserCount,0) UserCount,
			COALESCE(EC.ErrorCount,0) ErrorCount,
			COALESCE(LC.LRQCount,0) LRQCount
			From EDDS.eddsdbo.[Case] C
			Left Join 
			(Select CaseArtifactID, AVG(UserCount) UserCount   
				From eddsdbo.UserCountDW 
				Where MeasureDate = @SummaryDayHour Group By CaseArtifactID) UC
			On C.ArtifactID = UC.CaseArtifactID
			Left Join (Select CaseArtifactID, SUM(ErrorCount) ErrorCount    
				From eddsdbo.ErrorCountDW 
				Where MeasureDate = @SummaryDayHour  Group By CaseArtifactID) EC
			On C.ArtifactID = EC.CaseArtifactID
			LEFT JOIN (
				SELECT CaseArtifactID,
					COUNT(CaseArtifactID) LRQCount
				FROM eddsdbo.QoS_VarscatOutputDetailCumulative
				WHERE SummaryDayHour = @SummaryDayHour
				AND IsLongRunning = 1
				GROUP BY CaseArtifactID			
			) LC
			ON C.ArtifactID = LC.CaseArtifactID
			WHERE C.ArtifactID > 0
		COMMIT TRAN
	END
END

