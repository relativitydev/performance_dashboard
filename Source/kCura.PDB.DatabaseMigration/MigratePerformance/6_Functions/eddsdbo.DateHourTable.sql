USE [EDDSPerformance]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'eddsdbo.DateHourTable')) DROP FUNCTION eddsdbo.DateHourTable
GO

CREATE FUNCTION [eddsdbo].[DateHourTable]
(
	@FirstDate	datetime,
	@LastDate	datetime
)
RETURNS @datetable TABLE (
	[date]		datetime
)
AS
BEGIN

  SELECT @FirstDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @FirstDate));   SELECT @LastDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @LastDate)); 
  WITH CTE_DatesTable
  AS 
  (
    SELECT @FirstDate AS [date]
    UNION ALL
    SELECT DATEADD(HH, 1, [date])
    FROM CTE_DatesTable
    WHERE DATEADD(HH, 1, [date]) <= @LastDate
  )
  INSERT INTO @datetable ([date])
  SELECT [date] FROM CTE_DatesTable
  OPTION (MAXRECURSION 0)

  RETURN
END
 