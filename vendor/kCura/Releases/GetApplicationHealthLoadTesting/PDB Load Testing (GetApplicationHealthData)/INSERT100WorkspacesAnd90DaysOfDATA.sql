/********************************************************************************************************
*
* INSERT entries into the Case statistics table
* Pre-configured to insert one entry for the past 90 days
* Will skip days that already have an entry
* 
* Before running the script you must update the caseArtifactIDs and values in the insert statement
*
* Written by: Ryan Flint
* Modified by: Justin Jarczyk (day automation, and mass loading, for performance testing)
* Looks in the Artifact table for all workspaces... and iterates through each workspace generating random metrics
* for all tables
* 2013
********************************************************************************************************/
/********************************************************************************************************
* DIRECTIONS::
* Run this script the way it is... It will add 1,000 workspace to your relativity instance....
* It should take about 20 min. to load... once the script is executed, you will need to uncomment the code
* below, and drop the view [ DROP VIEW [eddsdbo].[EDDSWorkspace] ] in EDDSPerformance... and set it to 
* look at all the workspaces from the EDDS DATABASE.. the query below shows how to create this view...
*
* The limitations of this script will not test for VALID DATA, it will only test performance and load times.

*******************************************************************************************************
******************************************TEMPLATE ON VIEW TO CREATE***********************************
*******************************************************************************************************
*******************************************************************************************************

********************************************************************************************************/




USE [EDDSPerformance]
DROP VIEW [eddsdbo].[EDDSWorkspace]
USE [EDDSPerformance]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [eddsdbo].[EDDSWorkspace]
AS
SELECT  
	[a].ArtifactID		AS CaseArtifactID
	, [a].[TextIdentifier] AS WorkspaceName
	, '' AS [DatabaseLocation]
FROM EDDS.eddsdbo.Artifact a
WHERE a.ArtifactTypeID = 8
GO




---INSERT 1000 WORKSPACES
DECLARE @ITOR INT

SET @ITOR = 0

WHILE (@ITOR <= 1000)
BEGIN
	-- generate random workspaceID
	INSERT INTO [EDDS].[eddsdbo].[Artifact] (
		[ArtifactTypeID],
		[ParentArtifactID],
		[AccessControlListID],
		[AccessControlListIsInherited],
		[CreatedOn],
		[LastModifiedOn],
		[LastModifiedBy],
		[CreatedBy],
		[TextIdentifier],
		[ContainerID],
		[Keywords],
		[Notes],
		[DeleteFlag]
		)
	VALUES (
		8,
		62,
		1000353,
		0,
		'2007-01-01 00:00:00.000',
		'2012-10-15 17:14:03.197',
		9,
		9,
		'GENERATED WORKSPACE ' + CAST(@ITOR AS VARCHAR),
		62,
		'',
		'',
		0
		)

	SET @ITOR = @ITOR + 1

	PRINT 'CREATED [ GENERATED WORKSPACE ' + CAST(@ITOR AS VARCHAR) + ' ]'
END


USE MASTER







/***********/

DECLARE @Upper INT;
DECLARE @Lower INT
DECLARE @NumberOfDaysToGenerate INT --APPROX. 3 MO

SET @Lower = 2000 ---- The lowest random number
SET @Upper = 200000 ---- The highest random number
SET @NumberOfDaysToGenerate = 90

DECLARE @WorkspaceID INT

DECLARE IDs CURSOR LOCAL
FOR
SELECT a.ArtifactID
FROM EDDS.eddsdbo.Artifact a
WHERE a.ArtifactTypeID = 8

OPEN IDs

FETCH NEXT
FROM IDs
INTO @WorkspaceID

WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @startTime DATETIME
	DECLARE @endTime DATETIME

	SET @startTime = DATEADD(DD, (- 1 * @NumberOfDaysToGenerate), DATEADD(DD, 0, DATEDIFF(DD, 0, GETDATE())))
	SET @endTime = DATEADD(HH, 23, DATEADD(DD, 0, DATEDIFF(DD, 0, GETDATE())))

	WHILE @startTime <= @endTime
	BEGIN
		PRINT @WorkspaceID

		-- RANDOM NUMBER GENERATOR
		DECLARE @Random INT;
		DECLARE @Random1 INT;
		DECLARE @Random2 INT;
		DECLARE @Random3 INT;
		DECLARE @Random4 INT;
		DECLARE @Random5 INT;
		DECLARE @Random6 INT;
		DECLARE @Random7 INT;
		DECLARE @Random8 INT;
		DECLARE @Random9 INT;
		DECLARE @Random10 INT;
		DECLARE @Random11 INT;
		DECLARE @Random12 INT;
		DECLARE @Random13 INT;
		DECLARE @Random14 INT;

		SELECT @Random = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random1 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random2 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random3 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random4 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random5 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random6 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random7 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random8 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random9 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random10 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random11 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random12 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random13 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		SELECT @Random14 = ROUND(((@Upper - @Lower - 1) * RAND() + @Lower), 0)

		--PRINT @Random 
		--PRINT @Random1 
		--PRINT @Random2 
		--PRINT @Random3 
		--PRINT @Random4 
		--PRINT @Random5 
		--PRINT @Random6 
		--PRINT @Random7 
		--PRINT @Random8 
		--PRINT @Random9 
		--PRINT @Random10
		--PRINT @Random11
		--PRINT @Random12
		--PRINT @Random13
		--PRINT @Random14
		--PRINT @startTime
		--PRINT @endTime
		IF NOT EXISTS (
				SELECT 1
				FROM edds.eddsdbo.CaseStatistics
				WHERE CaseArtifactID = @WorkspaceID AND TIMESTAMP >= @startTime AND TIMESTAMP < DATEADD(DAY, 1, @startTime)
				)
		BEGIN
			INSERT INTO EDDS.eddsdbo.CaseStatistics (
				[CaseArtifactID],
				[timestamp],
				[FileCount],
				[TotalFileSize],
				[MDFFileSize],
				[FullTextDataFileSize],
				[LDFFileSize],
				[EditAuditCount],
				[ViewAuditCount],
				[CreateAuditCount],
				[DeleteAuditCount],
				[ActiveUserCount],
				[ActiveUserNameList],
				[CABuildNativeFileSize],
				[CABuildNativeFileCount],
				[CABuildTotalFileCount],
				[DocumentCount],
				[DateKey],
				[CaseName]
				)
			VALUES (
				@WorkspaceID,
				@startTime,
				@Random,
				@Random1,
				@Random2,
				@Random3,
				@Random4,
				@Random5,
				@Random6,
				@Random7,
				@Random8,
				@Random9,
				'relativity.admin@kcura.com; relativity.serviceaccount@kcura.com; rflint@kcura.com; smoke@kcura.com',
				@Random10,
				@Random11,
				@Random12,
				@Random13,
				@Random14,
				'Salt vs. Pepper (Large)'
				) 
				--need TO generate metric information BY the hour
			--FOR each individual workspace
			--FOR 24 hours

			DECLARE @startHour DATETIME

			SET @startHour = DATEADD(DAY, DATEDIFF(DAY, '19000101', @startTime), '19000101')

			DECLARE @endHour DATETIME

			SET @endHour = DATEADD(SECOND, - 1, DATEADD(DAY, DATEDIFF(DAY, '18991231', @endTime), '19000101'))

			DECLARE @CNT INT

			SET @CNT = 0

			WHILE (@startHour <= @endHour)
			BEGIN
				INSERT INTO [EDDSPerformance].[eddsdbo].[PerformanceSummary] (
					[CreatedOn],
					[CaseArtifactID],
					[MeasureDate],
					[UserCount],
					[ErrorCount],
					[LRQCount],
					[AverageLatency],
					[NRLRQCount],
					[TotalQCount],
					[TotalNRQCount]
					)
				VALUES (
					GETDATE(),
					@WorkspaceID,
					(Convert(CHAR(10), @startHour, 126)),
					@Random7,
					@Random6,
					@Random5,
					@Random1,
					@Random2,
					@Random3,
					@Random4
					)

				INSERT INTO [EDDSPerformance].[eddsdbo].[LRQCountDW] (
					[MeasureDate],
					[MeasureHour],
					[CaseArtifactID],
					[LRQCount],
					[CreatedOn],
					[NRLRQCount],
					[TotalQCount],
					[TotalNRQCount],
					[totalQtime]
					)
				VALUES (
					(Convert(CHAR(10), @startHour, 126)),
					@CNT,
					@WorkspaceID,
					@Random5,
					GETDATE(),
					@Random1,
					@Random2,
					@Random3,
					@Random4
					)

				PRINT @CNT
				PRINT @startHour

				SET @CNT = @CNT + 1
				SET @startHour = DATEADD(Hour, 1, @startHour)
			END

			-- 1 record per day ....
			INSERT INTO [EDDSPerformance].[eddsdbo].[BISSummary] (
				[CreatedOn],
				[CaseArtifactID],
				[MeasureDate],
				[TQCount],
				[TotalNRQCount],
				[NRLRQCount],
				[StatusDay],
				[Status90Days],
				[StatusPercentageNRLRQDay],
				[StatusPercentageNRLRQ90Days],
				[DocumentCount]
				)
			VALUES (
				getdate(),
				@WorkspaceID,
				(Convert(CHAR(10), @startTime, 126)),
				@Random1,
				@Random2,
				@Random3,
				0x0100000066543B9084B275F319CA76DB7DA83B31F0DB66C8D4E6AB0B15535E44B9672777AA1DC81CBCC64562,
				0x01000000D7D4429691D9453135E140BC2ED6B33BEEEEE732CB270460C725C94A7ED6EA8C890277B512279A86EFCF89272BF6B72D,
				@Random1,
				- 1,
				@Random10
				)

			PRINT @startTime
		END

		SET @startTime = DATEADD(DAY, 1, @startTime)

		FETCH NEXT
		FROM IDs
		INTO @WorkspaceID
	END
END

CLOSE IDs

DEALLOCATE IDs;
