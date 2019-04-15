/********************************************************************************************************
* Back Tests Performance of the new/old HealthData Stored Proceedures
* Run this test script to test performance on the new sproc vs. the old sproc
* 
* THE OLD SPROC MUST BE NAMED :: eddsdbo.GetApplicationHealthData
* THE NEW SPROC MUST BE NAMED :: eddsdbo.GetApplicationHealthDataUpdated 
* OR else the names will have to be changed to reflect the new and old sprocs
* 
* Written by: Justin Jarczyk
* 2013
********************************************************************************************************/


SET NOCOUNT ON;

USE EDDSPerformance

DECLARE @offset INT
SET @offset = 90

DECLARE @NumberOfDaysToGenerate INT
SET @NumberOfDaysToGenerate = 90

DECLARE @StartDate DATETIME
SET @StartDate = DATEADD(DD, (- 1 * @NumberOfDaysToGenerate), DATEADD(DD, 0, DATEDIFF(DD, 0, GETDATE())))

DECLARE @EndDate DATETIME
SET @EndDate = GETDATE()

DECLARE @Timer DATETIME

DECLARE @rcount INT



PRINT '| -------------------------------- |'
PRINT '| TEST SUITE COMPARING PERFORMANCE |'
PRINT '| -------------------------------- |'
PRINT '' 
PRINT '[eddsdbo.GetApplicationHealthData] vs [eddsdbo.GetApplicationHealthDataUpdated]'
PRINT ''
PRINT ''
PRINT '| -------------------------------- |'
-- start date ascending into end date
WHILE(@StartDate <= @EndDate)
	BEGIN
		PRINT CAST(@StartDate AS VARCHAR(20)) + ' - ' + CAST(@EndDate AS VARCHAR(20))
				PRINT 'OLD QUERY'
		SET @Timer = GETDATE()
		EXEC eddsdbo.GetApplicationHealthData	
		@StartDate,
		@EndDate,
			300
		SET @rcount = @@RowCount
		PRINT '# OF ROWS :: ' +CAST(@rcount AS VARCHAR(20) )
		PRINT 'TIME (S) :: ' + CAST((DATEDIFF(SS,@Timer,GETDATE())) AS VARCHAR(20) )
		PRINT 'NEW QUERY'
		SET @Timer = GETDATE()
		EXEC eddsdbo.GetApplicationHealthDataUpdated 	
		@StartDate,
		@EndDate,
			300
		SET @rcount = @@RowCount
		PRINT '# OF ROWS :: ' +CAST(@rcount AS VARCHAR(20) )
		PRINT 'TIME (S) :: ' + CAST((DATEDIFF(SS,@Timer,GETDATE())) AS VARCHAR(20) )
		PRINT '| -------------------------------- |'
		SET @StartDate = DATEADD(DAY, 1, @StartDate)
	END
	
	
	
SET @StartDate = DATEADD(DD, (- 1 * @NumberOfDaysToGenerate), DATEADD(DD, 0, DATEDIFF(DD, 0, GETDATE())))
-- end date descending into start date
WHILE(@StartDate <= @EndDate)
	BEGIN
		PRINT CAST(@StartDate AS VARCHAR(20)) + ' - ' + CAST(@EndDate AS VARCHAR(20))
				PRINT 'OLD QUERY'
		SET @Timer = GETDATE()
		EXEC eddsdbo.GetApplicationHealthData	
		@StartDate,
		@EndDate,
			300
		SET @rcount = @@RowCount
		PRINT '# OF ROWS :: ' +CAST(@rcount AS VARCHAR(20) )
		PRINT 'TIME (S) :: ' + CAST((DATEDIFF(SS,@Timer,GETDATE())) AS VARCHAR(20) )
		PRINT 'NEW QUERY'
		SET @Timer = GETDATE()
		EXEC eddsdbo.GetApplicationHealthDataUpdated 	
		@StartDate,
		@EndDate,
			300
		SET @rcount = @@RowCount
		PRINT '# OF ROWS :: ' +CAST(@rcount AS VARCHAR(20) )
		PRINT 'TIME (S) :: ' + CAST((DATEDIFF(SS,@Timer,GETDATE())) AS VARCHAR(20) )
		PRINT '| -------------------------------- |'
		SET @StartDate = DATEADD(DAY, 1, @StartDate)
	END

