/********************************************************************************************************
 *
 * INSERT all queries (RQ, NRQ, RLRQ, NRLRQ)
 * Pre-configured to insert one non relational long running query per hour for the past 90 days
 * 
 * Before running the script you must update line 10 with the workspace artifact ID
 * Update lines 89 - 97 with the values for the saved search you will insert in the auditRecord Table
 *
 * Written by: Ryan Flint
 * 2013
 ********************************************************************************************************/
 
SET NOCOUNT ON

USE EDDS#######

DECLARE @NRQi integer						-- Number of Queries (Used to set a static number of queries per hour)
DECLARE @RLRQi integer						-- Number of Queries (Used to set a static number of queries per hour)
DECLARE @RQi integer						-- Number of Queries (Used to set a static number of queries per hour)
DECLARE @NRLRQi integer						-- Number of Queries (Used to set a static number of queries per hour)

DECLARE @MinI integer						-- Lower end of number of queries if a random number of queries per hour is needed
DECLARE @MaxI integer						-- Upper end of number of queries if a random number of queries per hour is needed
DECLARE @MinNRQI integer
DECLARE @MaxNRQI integer
DECLARE @MinNRLRQI integer
DECLARE @MaxNRLRQI integer
DECLARE @MinRQI integer
DECLARE @MaxRQI integer
DECLARE @MinRLRQI integer
DECLARE @MaxRLRQI integer
DECLARE @UseRanLRQVal bit
DECLARE @MinValue integer					-- Lower end of timestamp min and sec random value
DECLARE @MaxValue integer					-- Upper end of timestamp min and sec random value

DECLARE @MinRanVal integer					-- Used to determine if a day will be "Poor," determined by # of LRQ
DECLARE @MaxRanVal integer
DECLARE @DayStatus float
DECLARE @DayStatusThreshold float
DECLARE @dayPercent float
DECLARE @forcePoor bit

DECLARE @Minute integer
DECLARE @Second integer
DECLARE @Value integer

DECLARE @NewTimeStamp datetime
DECLARE @StartingTimeStamp datetime
DECLARE @TempHour varchar(2)
DECLARE @NumberOfDays integer						-- Used to set how many days to insert queries for
DECLARE @HoursPerDay integer
DECLARE @TempHoursPerDay integer
DECLARE @StartingHour integer

DECLARE @TempMinute varchar(2)
DECLARE @TempSecond varchar(2)

DECLARE @MaxExecutionTime integer			-- Upper end of query execution time random value
DECLARE @MinExecutionTime integer			-- Lower end of query execution time random value
DECLARE @MinLRQExecutionTime integer
DECLARE @MaxLRQExecutionTime integer
DECLARE @NewExecutionTime integer

-- Query varibles
DECLARE @QueryArtifactID integer			-- Query artifactID for audit record
DECLARE @Action integer						-- Audit record action
DECLARE @QueryDetailsOne varchar(max)		-- Query Details through '<milliseconds>'
DECLARE @QueryDetailsTwo varchar(max)		-- Query Details starting from '</milliseconds>'
DECLARE @RequestOrigination varchar(max)	-- Used to build audit record entry
DECLARE @RQRequestOrigination varchar(max)	-- Used to build audit record entry for a relational query
DECLARE @RecordOrigination varchar(max)		-- Used to build audit record entry
DECLARE @UserID integer						-- User ID for audit record
DECLARE @SessionIdentifier integer			-- Session ID for audit record

-- SET number of days to populate heare --
SET @NumberOfDays = 90						-- Used to set how many days to insert queries for
SET @UseRanLRQVal = 0						-- Decide if you want to inject a preset number of queries (0) or a random amount of queries (1)
SET @startingHour = 0						-- Set the starting hour for the day
SET @HoursPerDay = 24						-- How many hours to insert per day

-- Ensure the day does not go past midnight
-- If it does then stop the day at midnight
if(@startingHour + @hoursPerDay > 24)
	SET @hoursPerDay = 24 - @startingHour

/*************************************************************************************************
 *
 *	Enter query details for the audit record table entry
 *
 *************************************************************************************************/
 
SET @QueryArtifactID = 1040782
SET @Action = 28
SET @QueryDetailsOne = '<auditElement><QueryText>SET NOCOUNT ON   SELECT TOP 1000   [Document].[ArtifactID],   CAST([SearchTable].[Rank] AS FLOAT) AS [Rank]    FROM   [Document] (NOLOCK)  LEFT JOIN CONTAINSTABLE([Document],([DocIDBeg],[ExtractedText],[GroupIdentifier]),''"Energy"'') AS [SearchTable]ON    [SearchTable].[Key] = [Document].[ArtifactID]  WHERE   [SearchTable].[RANK] &gt;= 1   AND  [Document].[AccessControlListID_D] IN (1)  ORDER BY    [Document].[ArtifactID]       -------------------  -- records returned: 383  -------------------  </QueryText><Milliseconds>'
SET @QueryDetailsTwo = '</Milliseconds><searchTableReplacements><R1>CONTAINSTABLE([Document],([DocIDBeg],[ExtractedText],[GroupIdentifier]),''"Energy"'') = CONTAINSTABLE([Document],([DocIDBeg],[ExtractedText],[GroupIdentifier]),''"Energy"'')</R1></searchTableReplacements></auditElement>'
SET @RequestOrigination = '<auditElement><RequestOrigination><IP>::1</IP><Page>http://localhost:8080/Relativity/Controls/ListTemplateFrames/SearchListTemplateFrame.aspx?ArtifactTypeID=10&amp;ArtifactID=1040782&amp;AppID=1015376&amp;ResetItemList=True</Page></RequestOrigination></auditElement>'
SET @RQRequestOrigination = '<auditElement><RequestOrigination><IP>::1</IP><Page>RelationalPanel</Page></RequestOrigination></auditElement>'
SET @RecordOrigination = '<auditElement><RecordOrigination><MAC>00:50:56:A1:12:79</MAC><IP>192.168.18.158</IP><Server>KIE-PDBTEST</Server></RecordOrigination></auditElement>'
SET @UserID = 1015375
SET @SessionIdentifier = 148

/******************  End Enter Query Details ****************************************************/

/************************************************************************************************
 ** 
 ** Build timestamp
 **
 ************************************************************************************************/
 
--Clean Today's Date and set to midnight--
SET @startingTimeStamp = GETDATE()
SET @startingTimeStamp = DATEADD(MI, -DATEPART(MI, @startingTimeStamp), @startingTimeStamp)
SET @startingTimeStamp = DATEADD(S, -DATEPART(S, @startingTimeStamp), @startingTimeStamp)
SET @startingTimeStamp = DATEADD(MILLISECOND, -DATEPART(MILLISECOND, @startingTimeStamp), @startingTimeStamp)
SET @startingTimeStamp = DATEADD(HOUR, -DATEPART(HOUR, @startingTimeStamp), @startingTimeStamp)
SET @startingTimeStamp = DATEADD(HOUR, @startingHour, @startingTimeStamp)

PRINT @startingTimeStamp

-- Go back x number of days
SET @StartingTimeStamp = DATEADD(DAY, -(@NumberOfDays - 1), @StartingTimeStamp)
PRINT 'Inserting in workspace ' + DB_NAME() + ' with a timestamp of ' 
			+ convert(varchar, @StartingTimeStamp, 121) + '.'

-- seed timestamp			
SET @MinValue = 1		-- Used for timestamp
SET @MaxValue = 59		-- Used for timestamp
SET @Minute = 0			-- Used for timestamp
SET @Second = 0			-- Used for timestamp


-- SET @TempHoursPerDay = @HoursPerDay - 1		-- 24 hour day goes from 0 - 23 so need to reduce number of hours by 1
SET @TempHoursPerDay = 0

/******* End build timestamp  ********************************************************************/

											
/*************************************************************************************************
 *
 *	Determine if a day will be poor
 *	Seed logic to determine poor days
 *  i.e. 25% of the days will be poor
 *
 *************************************************************************************************/
SET @forcePoor = 1		-- Use (set to 1) if you want to force all days to be poor 					
SET @DayStatusThreshold = 0.25	
SET @MinRanVal = 1
SET @MaxRanVal = 1000

-- Seed for random execution times
SET @MinExecutionTime = 1			-- Used for query execution time
SET @MaxExecutionTime = 1950		-- Used for query execution time
SET @MinLRQExecutionTime = 2000
SET @MaxLRQExecutionTime = 10000
SET @NewExecutionTime = 0


	
/***********************************************************************************************
 ***********************************************************************************************
 **
 **		Start While Loops
 **
 ***********************************************************************************************
 ***********************************************************************************************/

WHILE @NumberOfDays > 0
BEGIN	
	
	PRINT CAST(@NumberOfDays as varchar(4)) + ' days left to process.'
	--SELECT @NewTimeStamp as 'Days time stamp'
	
	--PRINT 'In days loop.  Days left = ' + cast(@NumberOfdays as varchar(10))
	WHILE @TempHoursPerDay < 24
	BEGIN
	
		--SELECT @HoursPerDay as 'Hours per day left'
		
		
		--PRINT 'In hours loop.  Hours = ' + cast(@hour as varchar(10))
		
		/********************************************************************************************
		 *
		 *	Set number of queries per hour here
		 *	Use @i if you want to set a static number of queries per hour
		 *	Use @MaxI and @MinI if you want a random number of queries per hour
		 *
		 *	If using @i then comment out lines 68, 69, & 70
		 *	If using @MinI and @MaxI then comment out line 66
		 *
		 *********************************************************************************************/
		 
		IF @forcePoor = 0
		BEGIN
		    -- Get random number to determine if a day is poor
			SET @DayStatus = CAST((@MaxRanVal - @MinRanVal + 1) * RAND() AS integer) + @MinRanVal
		END 
		ELSE
		BEGIN
			--PRINT 'Force poor day'
			SET @DayStatus = 1
		END
		
		-- Get percentage for the day
		SET @dayPercent =  (@DayStatus / @MaxRanVal) * 100
		 
		-- Determine if the percentage for the day is under the threshold (Poor)
		IF (@DayStatus / @MaxRanVal <= @DayStatusThreshold)
		BEGIN
			-- Day will be poor
			--PRINT 'Day Status = ' + CAST(@DayStatus as varchar(100)) + ': ' + CAST(@dayPercent AS varchar(100))
			--+ '%. Day will be poor.'
			
			IF @UseRanLRQVal = 0
			BEGIN
				SET @NRQi = 0	
				SET @NRLRQi = 1	
				SET @RQi = 0
				SET @RLRQi = 0
			END
			ELSE
			BEGIN
				SET @MinNRQI = 1
				SET @MaxNRQI = 40
				SET @NRQi = CAST((@MaxNRQI - @MinNRQI + 1) * RAND() AS integer) + @MinNRQI
							
				SET @MinNRLRQI = @NRQi * 0.15
				SET @MaxNRLRQI = @NRQi * 0.70
				SET @NRLRQi = CAST((@MaxNRLRQI - @MinNRLRQI + 1) * RAND() AS integer) + @MinNRLRQI
							
				SET @MinRQI = 1
				SET @MaxRQI = 40
				SET @RQi = CAST((@MaxRQI - @MinRQI + 1) * RAND() AS integer) + @MinRQI
				
				SET @MinRLRQI = @RQi * 0.15
				SET @MaxRLRQI = @RQi * 0.70
				SET @RLRQi = CAST((@MaxRLRQI - @MinRLRQI + 1) * RAND() AS integer) + @MinRLRQI
			END

		END
		ELSE
		BEGIN
			-- Day will be good
			--PRINT 'Day Status = ' + CAST(@DayStatus as varchar(100)) + ': ' + CAST(@dayPercent AS varchar(100)) 
			--+ '%. Day will be good.'
			IF @UseRanLRQVal = 0
			BEGIN
				SET @NRQi = 1425	
				SET @NRLRQi = 75	
				SET	@RQi = 1000
				SET @RLRQi = 100
			END
			ELSE
			BEGIN
				SET @MinNRQI = 1
				SET @MaxNRQI = 40
				SET @NRQi = CAST((@MaxNRQI - @MinNRQI + 1) * RAND() AS integer) + @MinNRQI
							
				SET @MinNRLRQI = @NRQi * 0.01
				SET @MaxNRLRQI = @NRQi * 0.10
				SET @NRLRQi = CAST((@MaxNRLRQI - @MinNRLRQI + 1) * RAND() AS integer) + @MinNRLRQI
				
				SET @MinRQI = 1
				SET @MaxRQI = 40
				SET @RQi = CAST((@MaxRQI - @MinRQI + 1) * RAND() AS integer) + @MinRQI
				
				SET @MinRLRQI = @RQi * 0.01
				SET @MaxRLRQI = @RQi * 0.10
				SET @RLRQi = CAST((@MaxRLRQI - @MinRLRQI + 1) * RAND() AS integer) + @MinRLRQI
			END
		END
		 
		--PRINT 'Inserting ' + CAST(@NRLRQi AS VARCHAR(6)) + ' NRLRQ records'
		--PRINT 'Inserting ' + CAST(@NRQi AS VARCHAR(6)) + ' NRQ records'
		--PRINT 'Inserting ' + CAST(@RQi AS VARCHAR(6)) + ' RQ records'
		--PRINT 'Inserting ' + CAST(@RLRQi AS VARCHAR(6)) + ' RLRQ records'

		/********************  End Set Number of Queries per Hour  ************************************/

		WHILE @NRQi > 0
		BEGIN

			/******************************************************************************************************
			 *	
			 *	Build TimeStamp
			 *
			 ******************************************************************************************************/
			
			-- Get the random minute value
			-- Set temp minute to a random number between 1 and 59
			SET @Minute = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue
			
			-- Get the random second value
			-- Set temp second to a random number between 1 and 59
			SET @Second = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue

			SET @NewTimeStamp = DATEADD(MINUTE, @Minute, @StartingTimeStamp)
			SET @NewTimeStamp = DATEADD(SECOND, @Minute, @NewTimeStamp)

			--PRINT 'NewTimeStamp = ' + convert(varchar, @NewTimeStamp, 121)

			/*****************  End Build TimeStamp  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Build ExecutionTime
			 *
			 ******************************************************************************************************/
			 
			-- Set new exection time to a random number between @MinExecutionTime and @MaxExecutionTime
			SET @NewExecutionTime = CAST((@MaxExecutionTime - @MinExecutionTime + 1) * RAND() AS integer) + @MinExecutionTime
			--PRINT 'Execution Time = ' +  CAST(@NewExecutionTime AS varchar(10))
				  
			/*****************  End Build ExecutionTime  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Insert into the AuditRecord_PrimaryPartition Table
			 *
			 ******************************************************************************************************/
		 
			INSERT INTO EDDSDBO.AuditRecord_PrimaryPartition (
			ArtifactID, 
			Action, 
			Details, 
			UserID, 
			[TimeStamp], 
			RequestOrigination, 
			RecordOrigination, 
			ExecutionTime, 
			SessionIdentifier)

			VALUES(
			@QueryArtifactID, 
			@Action,
			@QueryDetailsOne + CAST(@NewExecutionTime as varchar(5)) + @QueryDetailsTwo,
			@UserID,
			@NewTimeStamp,
			@RequestOrigination,
			@RecordOrigination,
			@NewExecutionTime,
			@SessionIdentifier
			)
			
			SET @NRQi = @NRQi - 1
		END

		/*****************  End Insert into AuditRecord_PrimaryPartition Table  **************************************/

		 -- INSERT Non-Relational LRQ (NRLRQ)

		/********************  End Set Number of Queries per Hour  ************************************/

		WHILE @NRLRQi > 0
		BEGIN

			/******************************************************************************************************
			 *	
			 *	Build TimeStamp
			 *
			 ******************************************************************************************************/
			
			-- Get the random minute value
			-- Set temp minute to a random number between 1 and 59
			SET @Minute = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue
			
			-- Get the random second value
			-- Set temp second to a random number between 1 and 59
			SET @Second = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue

			SET @NewTimeStamp = DATEADD(MINUTE, @Minute, @StartingTimeStamp)
			SET @NewTimeStamp = DATEADD(SECOND, @Minute, @NewTimeStamp)

			--PRINT 'NewTimeStamp = ' + convert(varchar, @NewTimeStamp, 121)

			/*****************  End Build TimeStamp  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Build ExecutionTime
			 *
			 ******************************************************************************************************/
			 
			-- Set new exection time to a random number between @MinExecutionTime and @MaxExecutionTime
			SET @NewExecutionTime = CAST((@MaxLRQExecutionTime - @MinLRQExecutionTime + 1) * RAND() AS integer) + @MinLRQExecutionTime
			--PRINT 'Execution Time = ' +  CAST(@NewExecutionTime AS varchar(10))
				  
			/*****************  End Build ExecutionTime  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Insert into the AuditRecord_PrimaryPartition Table
			 *
			 ******************************************************************************************************/
		 
			INSERT INTO EDDSDBO.AuditRecord_PrimaryPartition (
			ArtifactID, 
			Action, 
			Details, 
			UserID, 
			[TimeStamp], 
			RequestOrigination, 
			RecordOrigination, 
			ExecutionTime, 
			SessionIdentifier)

			VALUES(
			@QueryArtifactID, 
			@Action,
			@QueryDetailsOne + CAST(@NewExecutionTime as varchar(5)) + @QueryDetailsTwo,
			@UserID,
			@NewTimeStamp,
			@RequestOrigination,
			@RecordOrigination,
			@NewExecutionTime,
			@SessionIdentifier
			)
			
			SET @NRLRQi = @NRLRQi - 1
		END

		/*****************  End Insert into AuditRecord_PrimaryPartition Table  **************************************/

		 -- INSERT Relational Query - NOT LRQ (RQ)

		WHILE @RQi > 0
		BEGIN

			/******************************************************************************************************
			 *	
			 *	Build TimeStamp
			 *
			 ******************************************************************************************************/
			
			-- Get the random minute value
			-- Set temp minute to a random number between 1 and 59
			SET @Minute = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue
			
			-- Get the random second value
			-- Set temp second to a random number between 1 and 59
			SET @Second = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue

			SET @NewTimeStamp = DATEADD(MINUTE, @Minute, @StartingTimeStamp)
			SET @NewTimeStamp = DATEADD(SECOND, @Minute, @NewTimeStamp)

			--PRINT 'NewTimeStamp = ' + convert(varchar, @NewTimeStamp, 121)

			/*****************  End Build TimeStamp  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Build ExecutionTime
			 *
			 ******************************************************************************************************/
			 
			-- Set new exection time to a random number between @MinExecutionTime and @MaxExecutionTime
			SET @NewExecutionTime = CAST((@MaxExecutionTime - @MinExecutionTime + 1) * RAND() AS integer) + @MinExecutionTime
			--PRINT 'Execution Time = ' +  CAST(@NewExecutionTime AS varchar(10))
				  
			/*****************  End Build ExecutionTime  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Insert into the AuditRecord_PrimaryPartition Table
			 *
			 ******************************************************************************************************/
		 
			INSERT INTO EDDSDBO.AuditRecord_PrimaryPartition (
			ArtifactID, 
			Action, 
			Details, 
			UserID, 
			[TimeStamp], 
			RequestOrigination, 
			RecordOrigination, 
			ExecutionTime, 
			SessionIdentifier)

			VALUES(
			@QueryArtifactID, 
			@Action,
			@QueryDetailsOne + CAST(@NewExecutionTime as varchar(5)) + @QueryDetailsTwo,
			@UserID,
			@NewTimeStamp,
			@RQRequestOrigination,
			@RecordOrigination,
			@NewExecutionTime,
			@SessionIdentifier
			)
			
			SET @RQi = @RQi - 1
		END

		/*****************  End Insert into AuditRecord_PrimaryPartition Table  **************************************/

		 -- INSERT Relational LRQ (RLRQ)
		 
		/********************  End Set Number of Queries per Hour  ************************************/

		WHILE @RLRQi > 0
		BEGIN

			/******************************************************************************************************
			 *	
			 *	Build TimeStamp
			 *
			 ******************************************************************************************************/
			
			-- Get the random minute value
			-- Set temp minute to a random number between 1 and 59
			SET @Minute = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue
			
			-- Get the random second value
			-- Set temp second to a random number between 1 and 59
			SET @Second = CAST((@MaxValue - @MinValue + 1) * RAND() AS integer) + @MinValue

			SET @NewTimeStamp = DATEADD(MINUTE, @Minute, @StartingTimeStamp)
			SET @NewTimeStamp = DATEADD(SECOND, @Minute, @NewTimeStamp)

			--PRINT 'NewTimeStamp = ' + convert(varchar, @NewTimeStamp, 121)

			/*****************  End Build TimeStamp  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Build ExecutionTime
			 *
			 ******************************************************************************************************/
			 
			-- Set new exection time to a random number between @MinExecutionTime and @MaxExecutionTime
			SET @NewExecutionTime = CAST((@MaxLRQExecutionTime - @MinLRQExecutionTime + 1) * RAND() AS integer) + @MinLRQExecutionTime
			--PRINT 'Execution Time = ' +  CAST(@NewExecutionTime AS varchar(10))
				  
			/*****************  End Build ExecutionTime  **************************************************************/

			/******************************************************************************************************
			 *
			 *	Insert into the AuditRecord_PrimaryPartition Table
			 *
			 ******************************************************************************************************/
		 
			INSERT INTO EDDSDBO.AuditRecord_PrimaryPartition (
			ArtifactID, 
			Action, 
			Details, 
			UserID, 
			[TimeStamp], 
			RequestOrigination, 
			RecordOrigination, 
			ExecutionTime, 
			SessionIdentifier)

			VALUES(
			@QueryArtifactID, 
			@Action,
			@QueryDetailsOne + CAST(@NewExecutionTime as varchar(5)) + @QueryDetailsTwo,
			@UserID,
			@NewTimeStamp,
			@RQRequestOrigination,
			@RecordOrigination,
			@NewExecutionTime,
			@SessionIdentifier
			)
			
			SET @RLRQi = @RLRQi - 1
		END

		/*****************  End Insert into AuditRecord_PrimaryPartition Table  **************************************/

		-- increment one hour
		if @tempHoursPerDay < 23
		SET @StartingTimeStamp = DATEADD(HOUR, 1, @StartingTimeStamp)
		
		-- Reduce number of hours left by 1
		SET @temphoursPerDay = @temphoursPerDay + 1
		
		--SELECT @NewTimeStamp as 'Hours time stamp'
		
		
	END

	SET @tempHoursPerDay = 0

	-- zero out hours and set to @startingHour
	SET @startingTimeStamp = DATEADD(HOUR, -DATEPART(HOUR, @startingTimeStamp), @startingTimeStamp)
	SET @startingTimeStamp = DATEADD(HOUR, @startingHour, @startingTimeStamp)
		
	-- Completed one day, reduce number by one
	SET @NumberOfDays = @NumberOfDays - 1	
	-- increment one day
	SET @StartingTimeStamp = DATEADD(DAY, 1, @StartingTimeStamp)

END -- Number of days while loop

SET NOCOUNT OFF