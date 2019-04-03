USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'LoadErrorHealthDWData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO

ALTER PROCEDURE  [eddsdbo].[LoadErrorHealthDWData] 
	@ProcessExecDate DateTime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @Frequency int
	DECLARE @SummaryDayHour DATETIME = DATEADD(hh,DATEDIFF(hh, 0, @ProcessExecDate), 0);
	Select @Frequency= COALESCE(Frequency,0) From eddsdbo.Measure Where MeasureID = 2

	IF @Frequency > 0 
	BEGIN
		--this will give us the error count for the past five minutes for each workspace
		--we'll later need to update this script to filter for only "kickout" errors
		Insert eddsdbo.ErrorCountDW (MeasureDate, CaseArtifactID, ErrorCount, CreatedOn)
		SELECT @SummaryDayHour MeasureDate, C.ArtifactID AS CaseArtifactID, COALESCE(EC.ErrorCount,0) ErrorCount, GetUTCDate() as [CreatedOn]
		FROM EDDS.eddsdbo.[Case] C
		Left Join (SELECT
			CaseArtifactID,
			COUNT(ArtifactID) as ErrorCount
		FROM
			EDDS.eddsdbo.ExtendedError (NOLOCK)
		WHERE
			CreatedOn >= @ProcessExecDate AND CreatedOn <= DateAdd(Minute, @Frequency, @ProcessExecDate)
		 AND ((FullError Like '%Read Failed%'   
			OR FullError LIKE '%Delete Failed%'
			OR FullError LIKE '%Create Failed%'
			OR FullError LIKE '%Update Failed%'
			OR FullError LIKE '%object reference not set to an instance of an object%'
			OR FullError LIKE '%SQL Statement Failed%'
			OR FullError LIKE '%Unable to connect to the remote server%')
			AND Source <> 'Native Document Viewer')
		 AND
			CaseArtifactID IS NOT NULL
		GROUP BY
			CaseArtifactID) EC
			On C.ArtifactID = EC.CaseArtifactID	
			WHERE C.ArtifactID > 0	
	END		
END

