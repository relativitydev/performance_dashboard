USE [EDDSPerformance]
GO

/****** Object:  UserDefinedFunction [eddsdbo].[GetLRQHealthQueries]    Script Date: 12/17/2012 15:23:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


PRINT N'Create function [eddsdbo].[GetLRQHealthQueries]'
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[GetLRQHealthQueries]') AND type in (N'TF'))
DROP FUNCTION [eddsdbo].[GetLRQHealthQueries]
GO


-- =============================================
-- Author:		Murali Shesham
-- Create date: 2011-09-12
-- Description:	Returns Query list for LRQs
-- 2012-05-22 : Ron@Milyli - Modified return table to pass back dblocation in separate field
--			    This is so we can first connect to that remote database, then execute the query.
-- 2012-12-21 : Ron@Mulyli - Modified so that the queries look back to the previous hours, not future time.
-- 2013-02-18 : Ron@Milyli - Added new TotalNRQry
-- 2013-02-26 : Ron@Milyli - Modified WHERE so that [Timestamp] > DATEADD (was >= ) based on kCura feedback.
-- 2013-02-26 : Ron@Milyli - Modified WHERE to use CONVERT instead of CAST based on kCura feedback.
-- =============================================
CREATE FUNCTION [eddsdbo].[GetLRQHealthQueries] 
(	
	@ProcessExecDate DateTime
)
RETURNS 
@LRQHealthQry TABLE 
(
	dbLocation varchar(50),
	TotalQry varchar(2000),
	TotalNRQry varchar(2000),
	LRQQry varchar(2000),
	NRLRQQry varchar(2000)
)
AS
BEGIN
	-- Fill the table variable with the rows for your result set

	Declare @MeasureDate Date
	Declare @MeasureHour Int
	Declare @Frequency varchar(10)
	Select @MeasureDate = Cast(@ProcessExecDate as Date) , @MeasureHour = DatePart(HH,@ProcessExecDate) 
	Select @Frequency= CAST((0-COALESCE(Frequency,0)) as Varchar(10)) From eddsdbo.Measure Where MeasureID = 1

	IF @Frequency != '0' 
	BEGIN
		Insert @LRQHealthQry (dbLocation, TotalQry, TotalNRQry, LRQQry, NRLRQQry)
		SELECT 	 CAST(dblocation as varchar(50)),

			'SELECT ''' + CAST(@MeasureDate as Varchar(50)) + ''' MeasureDate, ' + CAST(@MeasureHour as Varchar(10)) +'  MeasureHour,  ' + CAST(ArtifactID AS Varchar(50))+ ' CaseArtifactID, 
				COUNT(ID) LRQCount, 
				GetUTCDate() as [CreatedOn]
			FROM EDDS' + CAST(ArtifactID AS Varchar(50))+ '.EDDSDBO.AuditRecord WITH (NOLOCK)
			WHERE
				[Action] = 28 AND
				[TimeStamp] <= ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''' AND 
				[TimeStamp] > DATEADD(MINUTE, ' + @Frequency + ', ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''');',

			'SELECT ''' + CAST(@MeasureDate as Varchar(50)) + ''' MeasureDate, ' + CAST(@MeasureHour as Varchar(10)) +'  MeasureHour,  ' + CAST(ArtifactID AS Varchar(50))+ ' CaseArtifactID, 
				COUNT(ID) LRQCount, 
				GetUTCDate() as [CreatedOn]
			FROM EDDS' + CAST(ArtifactID AS Varchar(50))+ '.EDDSDBO.AuditRecord WITH (NOLOCK)
			WHERE
				[Action] = 28 AND
				[RequestOrigination] NOT LIKE ''%RelationalPanel%'' AND
				[TimeStamp] <= ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''' AND 
				[TimeStamp] > DATEADD(MINUTE, ' + @Frequency + ', ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''');',

			'SELECT ''' + CAST(@MeasureDate as Varchar(50)) + ''' MeasureDate, ' + CAST(@MeasureHour as Varchar(10)) +'  MeasureHour,  ' + CAST(ArtifactID AS Varchar(50))+ ' CaseArtifactID, 
				COUNT(ID) LRQCount, 
				GetUTCDate() as [CreatedOn]
			FROM EDDS' + CAST(ArtifactID AS Varchar(50))+ '.EDDSDBO.AuditRecord WITH (NOLOCK)
			WHERE
				[Action] = 28 AND
				[ExecutionTime] >= 2000 AND
				[TimeStamp] <= ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''' AND 
				[TimeStamp] > DATEADD(MINUTE, ' + @Frequency + ', ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''');',
				
			'SELECT ''' + CAST(@MeasureDate as Varchar(50)) + ''' MeasureDate, ' + CAST(@MeasureHour as Varchar(10)) +'  MeasureHour,  ' + CAST(ArtifactID AS Varchar(50))+ ' CaseArtifactID, 
				COUNT(ID) LRQCount, 
				GetUTCDate() as [CreatedOn]
			FROM EDDS' + CAST(ArtifactID AS Varchar(50))+ '.EDDSDBO.AuditRecord WITH (NOLOCK)
			WHERE
				[Action] = 28 AND
				[ExecutionTime] >= 2000 AND
				[RequestOrigination] NOT LIKE ''%RelationalPanel%'' AND
				([UserId] >= 1000000 OR [UserId] = 9) AND
				[TimeStamp] <= ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''' AND 
				[TimeStamp] > DATEADD(MINUTE, ' + @Frequency + ', ''' + CONVERT(varchar, @ProcessExecDate, 21) + ''');'

		FROM	EDDS.eddsdbo.ExtendedCase WITH (NOLOCK)	
	END
	RETURN 
END


GO

