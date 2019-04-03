USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'LoadUserHealthDWData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

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
-- =============================================
ALTER PROCEDURE  [eddsdbo].[LoadUserHealthDWData] 
	@ProcessExecDate DateTime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SummaryDayHour DATETIME = DATEADD(hh,DATEDIFF(hh, 0, @ProcessExecDate), 0);
	Declare @Frequency int
	Select @Frequency= COALESCE(Frequency,0) From eddsdbo.Measure Where MeasureID = 4
 
	IF @Frequency > 0 
	BEGIN
		Insert eddsdbo.UserCountDW (MeasureDate, CaseArtifactID, UserCount, CreatedOn)
		SELECT  @SummaryDayHour MeasureDate, C.ArtifactID AS CaseArtifactID, COALESCE(UC.UserCount,0) UserCount, GETUTCDATE() as [CreatedOn]
		FROM EDDS.eddsdbo.[Case] C
		Left Join (SELECT
			CaseArtifactID,
			COUNT(UserID) as UserCount	
		FROM
			EDDS.eddsdbo.UserStatus (NOLOCK)
		WHERE
			CaseArtifactID <> -1
			-- value of -1 indicates they're not currently in any workspace
		GROUP BY
			CaseArtifactID) UC
			On C.ArtifactID = UC.CaseArtifactID
			WHERE C.ArtifactID > 0
	END
END

