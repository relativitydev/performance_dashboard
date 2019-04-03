USE [EDDSPerformance]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT N'Create procedure [eddsdbo].[LoadBISSummary]'
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[LoadBISSummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[LoadBISSummary]
GO

-- =============================================
-- Author:		Ron@Milyli
-- Create date: 2012-06-01
-- Description:	
-- 2013-02-18 : Ron@Milyli : Added new TotalNRQCount field
-- 2013-02-18 : Ron@Milyli : Added the field encryption code
-- 2013-06-05 : David@Milyli.com : added support for a timezone offset
-- 2013-06-06 : Ron@Milyli : Added collection of Document Count
-- =============================================
CREATE PROCEDURE [eddsdbo].[LoadBISSummary]
	@ProcessExecDate DateTime, 
	@TimeZoneOffset int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @pwd NVarchar(100)
	SET @pwd = 'kCuraPassword1!'

	set @TimeZoneOffset  = DATEDIFF(MINUTE, GETUTCDATE(), GETDATE()) * -1
	
	Declare @SummaryMeasureDate DateTime
	Declare @MeasureDate Date
	
	Select @MeasureDate = Cast(@ProcessExecDate as Date)
	set @MeasureDate = DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, @MeasureDate)) ,0)
	Select @SummaryMeasureDate = @MeasureDate

	--BISSummary data is already converted to local time
	IF EXISTS(Select 1 From eddsdbo.BISSummary Where MeasureDate = @SummaryMeasureDate)
	BEGIN 
	  --Existing data for the date, run an update
	  BEGIN TRAN
		UPDATE BISSummary
		SET TQCount = COALESCE(LC.TQCount,0),
			TotalNRQCount = COALESCE(LC.TotalNRQCount, 0),
			NRLRQCount = COALESCE(LC.NRLRQCount,0),
			DocumentCount = COALESCE(CS.DocumentCount,0)
		FROM
			eddsdbo.BISSummary BS
			 LEFT JOIN
             (SELECT CaseArtifactID, DocumentCount
                FROM EDDS.eddsdbo.CaseStatistics
                WHERE (DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, CAST(timestamp AS Date))), 0) = @MeasureDate)) AS CS 
                ON BS.CaseArtifactID = CS.CaseArtifactID 
			 LEFT JOIN
			 (Select CaseArtifactID, SUM(TotalQCount) TQCount, SUM(TotalNRQCount) TotalNRQCount, SUM(LRQCount) LRQCount, SUM(NRLRQCount) NRLRQCount   
			    From eddsdbo.LRQCountDW where DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, MeasureDate)) ,0) = @MeasureDate 
			    Group By CaseArtifactID) LC
			    On BS.CaseArtifactID = LC.CaseArtifactID 
			AND BS.MeasureDate = @MeasureDate			
	  COMMIT TRAN 

	END

	ELSE

	BEGIN
	    --New day, so do an initial insert
		BEGIN TRAN
 			INSERT BISSummary (CreatedOn, CaseArtifactID, MeasureDate, TQCount, TotalNRQCount, NRLRQCount, StatusDay, Status90Days, StatusPercentageNRLRQDay, StatusPercentageNRLRQ90Days, DocumentCount)
			SELECT 
				GetUTCDate() CreatedOn, 
				C.ArtifactID CaseArtifactID, 	
				@SummaryMeasureDate MeasureDate, 
				COALESCE(LC.TQCount,0) TQCount, 
			    COALESCE(LC.TotalNRQCount, 0) TotalNRQCount,
				COALESCE(LC.NRLRQCount,0) NRLRQCount, 
				EncryptByPassPhrase(@pwd,N'-1',1,CONVERT( varbinary, @SummaryMeasureDate)) StatusDay,
				EncryptByPassPhrase(@pwd,N'-1',1,CONVERT( varbinary, @SummaryMeasureDate)) Status90Days,
				-1 StatusPercentageNRLRQDay,
				-1 StatusPercentageNRLRQ90Days,
				COALESCE(CS.DocumentCount,0) DocumentCount
			FROM
			  EDDS.eddsdbo.[Case] C
			   LEFT JOIN
                  (SELECT CaseArtifactID, DocumentCount
                    FROM EDDS.eddsdbo.CaseStatistics
                    WHERE (DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, CAST(timestamp AS Date))), 0) = @MeasureDate)) AS CS 
                    ON C.ArtifactID = CS.CaseArtifactID 
				LEFT JOIN 
				(Select CaseArtifactID, SUM(TotalQCount) TQCount, SUM(TotalNRQCount) TotalNRQCount, SUM(LRQCount) LRQCount, SUM(NRLRQCount) NRLRQCount   
				 From eddsdbo.LRQCountDW 
				 Where DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, MeasureDate)) ,0) = @MeasureDate
				 Group By CaseArtifactID) LC
			     On C.ArtifactID = LC.CaseArtifactID

		COMMIT TRAN
END
END
GO



PRINT N'Create procedure [eddsdbo].[UpdateBISScores]'
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[UpdateBISScores]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[UpdateBISScores]
GO


-- =============================================
-- Author:		Ron@Milyli
-- Create date: 2012-06-01
-- Description:	Populate BIS Values
-- 2013-02-18 : Ron@Milyli - Modified Percentage to use TotalNRQCount as denominator
-- 2013-02-18 : Ron@Milyli : Added the field encryption code
-- 2013-02-18 : Ron@Milyli : Updated to Josh's most up to date code
-- 2013-04-19 : Ron@Milyli : Added ISNULL check around BS.StatusPercentageNRLRQDay
-- 2013-04-19 : Ron@Milyli : Added limiter to NinetyDayCursor.
-- 2013-04-23 : Ron@Milyli : Removed 90 day running average code. Does not appear to be displayed in the GUI.
-- 2013-06-06 : Ron@Milyli : Added additional logic to take into account Document Counts   
-- =============================================
CREATE PROCEDURE [eddsdbo].[UpdateBISScores]
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @pwd NVarchar(100)
	SET @pwd = 'kCuraPassword1!'

	--=============================================================================================
	
	--Determine the percentage of NRLRQ per day and update the field
	UPDATE eddsdbo.BISSummary 
	SET StatusPercentageNRLRQDay = CONVERT(integer, (ISNULL((CONVERT(decimal, NRLRQCount) / NULLIF(CONVERT(decimal, TotalNRQCount),0)),0)) * 100)	

	--=============================================================================================

	--Determine the Status per day and update the field
	UPDATE eddsdbo.BISSummary
	SET StatusDay = 
		EncryptByPassPhrase(
			@pwd,
			CONVERT(nvarchar(max),
			CASE
				--Fail/Poor
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) <= 1000000) AND (StatusPercentageNRLRQDay >= 15) then 3 --Fail/Poor
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 1000001 AND 3000000) AND (StatusPercentageNRLRQDay >= 22.5) then 3 --Fail/Poor
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 3000001 AND 5000000) AND (StatusPercentageNRLRQDay >= 32.5) then 3 --Fail/Poor
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) >= 5000001) AND (StatusPercentageNRLRQDay >= 40) then 3 --Fail/Poor
				--Probation/Moderate
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) <= 1000000) AND (StatusPercentageNRLRQDay >= 10) then 2 --Probation/Moderate
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 1000001 AND 3000000) AND (StatusPercentageNRLRQDay >= 17.5) then 2 --Probation/Moderate 
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 3000001 AND 5000000) AND (StatusPercentageNRLRQDay >= 27.5) then 2 --Probation/Moderate
				WHEN (NRLRQCount >= 50) AND (COALESCE(DocumentCount,0) >= 5000001) AND (StatusPercentageNRLRQDay >= 35) then 2 --Probation/Moderate
				--Passed
				ELSE 1 --Passed
			END),
			1,
			CONVERT( varbinary, MeasureDate)
		)

	--=============================================================================================
	
END
GO