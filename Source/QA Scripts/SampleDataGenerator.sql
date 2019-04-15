/*
	PDB QA Scripts:
	Sample Data Generator

	Author: Joseph Low
	Date: 7/17/2014

	Comments: This script wipes out existing data in QoS_SystemLoadSummary, QoS_UserXInstanceSummary, QoS_SampleHistory, QoS_Ratings, and QoS_UptimeRatings. It also
		destroys data in ServerSummary that would interfere with new generated data when building the sample. You have been warned! It will generate base data for the
		hours specified below. After generating base data with this script, you should run other queries to get the data into the desired state for a particular test
		scenario. Once the data is in the desired state, run QoS_BuildandRateSample against a particular hour to view the sample sets and scores. This script will
		disable automated Looking Glass calls to prevent interference. Remember to enable them again after you're done looking at the data!
		
		After running this, a test scenario in 7DayQoSDataScenarios, and GenerateRatingsFromSampleData, you can use this script to append more data to your test bed.
		To do so, change @clearData to 0 below (so tables will not be wiped) and adjust @baseDate to control the date range of the new data. For example:
		
			* SampleDataGenerator executes with @baseDate = '2014-07-03 00:00:00.000', @numberOfDays = 11, @clearData = 1, and @arrivalRateSeed = 0
			* Scenario 5 in 7DayQoSDataScenarios is run against the data
			* GenerateRatingsFromSampleData is run
			* SampleDataGenerator executes again with @baseDate = '2014-07-14 00:00:00.000', @numberOfDays = 3, @clearData = 0, and @arrivalRateSeed = 500
			* GenerateRatingsFromSampleData is run again
			
		The result of the above is that three days of new entries are added onto the first sample, and sample bits are updated accordingly.
*/

SET NOCOUNT ON

--Parameters you can modify
DECLARE @baseDate datetime = '2014-07-03 00:00:00.000';
DECLARE @numberOfDays int = 11;
DECLARE @clearData bit = 1; --Set this to 1 to clear existing data, 0 to keep it (for adding to the test sample)
DECLARE @arrivalRateSeed int = 0; --If you want to start the arrival rate at something higher than 0, you can tweak this.

--Parameters you should leave alone
DECLARE @endDate datetime = DATEADD(dd, @numberOfDays, @baseDate);
DECLARE @workingDate datetime = @baseDate;

--Only proceed if Looking Glass is not currently running
IF EXISTS (SELECT TOP 1 * FROM [EDDSPerformance].[eddsdbo].[QoS_GlassRunHistory] WHERE isActive = 1)
BEGIN
	PRINT 'Halting due to active Looking Glass job - see QoS_GlassRunHistory'
	RETURN;
END

--Disable automated Looking Glass calls (to enable, set Frequency to 60)
UPDATE [EDDSPerformance].[eddsdbo].[ProcessControl]
SET Frequency = -1
WHERE ProcessControlID = 7;

PRINT 'Disabled automated Looking Glass calls'

--Temporary tables
CREATE TABLE #PDB_Servers
(
	ID int IDENTITY(1,1) NOT NULL,
	ServerArtifactID int NOT NULL,
	ServerTypeID int NOT NULL
);

--Destroy existing data
IF @clearData = 1
BEGIN
	TRUNCATE TABLE EDDSPerformance.eddsdbo.QoS_SystemLoadSummary;
	TRUNCATE TABLE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary;
	TRUNCATE TABLE EDDSPerformance.eddsdbo.QoS_SampleHistory;
	TRUNCATE TABLE EDDSPerformance.eddsdbo.QoS_Ratings;
	TRUNCATE TABLE EDDSPerformance.eddsdbo.QoS_UptimeRatings;

	PRINT 'Destroyed existing Ratings, UptimeRatings, SystemLoadSummary, UserXInstanceSummary, and SampleHistory rows'
END

DELETE FROM EDDSPerformance.eddsdbo.ServerSummary
WHERE MeasureDate >= @baseDate AND MeasureDate < @endDate;

PRINT 'Destroyed ServerSummary data that would interfere with generated data at sample time'

--Get server list
INSERT INTO #PDB_Servers (ServerArtifactID, ServerTypeID)
SELECT ArtifactID, ServerTypeID FROM EDDSPerformance.eddsdbo.Server
WHERE ServerTypeID IN (1, 3);

PRINT 'Gathered working server list'

--SELECT * FROM #PDB_Servers;

WHILE (@workingDate < @endDate)
BEGIN
	--Create a new row for each SQL server in SystemLoadSummary
	INSERT INTO EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
		(ServerArtifactID, ServerTypeID, AvgRAMPagesePerSec, AvgCPUpct, AvgRAMpct, RAMPagesScore, RAMPctScore, CPUScore, QoSHourID, SummaryDayHour)
	SELECT
		ServerArtifactID,
		ServerTypeID,
		0, 0, 0, --Metrics
		RAND()*100, RAND()*100, RAND()*100, --Scores
		EDDSPerformance.eddsdbo.QoS_GetServerHourID(ServerArtifactID, @workingDate), --QoSHourID
		@workingDate --SummaryDayHour
	FROM #PDB_Servers
	WHERE ServerTypeID = 3;

	INSERT INTO EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
		(ServerArtifactID, QoSHourID, GlassRunID, SummaryDayHour, AVGConcurrency, ArrivalRate, TotalSimpleQuery, TotalComplexQuery, TotalViews, TotalEdits, TotalMassOps,
		TotalOtherActions, TotalSLRQ, TotalCLRQ, AvgSQScorePerUser)
	SELECT
		ServerArtifactID,
		QoSHourID,
		1, --GlassRunID
		@workingDate, --SummaryDayHour
		RAND()*20, --AVGConcurrency
		(RAND()*10) + @arrivalRateSeed * 0.01, --Arrival rate
		2, --TotalSimpleQuery
		2, --TotalComplexQuery
		2, --TotalViews
		2, --TotalEdits
		2, --TotalMassOps
		2, --TotalOtherActions
		0, --TotalSLRQ
		0, --TotalCLRQ
		RAND()*100 --AvgSQScorePerUser
	FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
	WHERE SummaryDayHour = @workingDate;

	--We shouldn't need to do this because BuildandRateSample does it for us
	/*INSERT INTO EDDSPerformance.eddsdbo.QoS_SampleHistory
		(QoSHourID, SummaryDayHour, ServerArtifactID, ArrivalRate, AVGConcurrency, IsActiveSample, IsActive4Sample, IsActiveWeeklySample, IsActiveWeekly4Sample, SampleDate)
	SELECT
		QoSHourID,
		@workingDate, --SummaryDayHour
		ServerArtifactID,
		ArrivalRate, --Arrival rate
		AVGConcurrency, --Concurrency
		0, 0, 0, 0, --Sample bits
		@workingDate --SampleDate
	FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
	WHERE SummaryDayHour = @workingDate;*/

	PRINT 'Done generating data for ' + CAST(@workingDate as varchar)
	SET @workingDate = DATEADD(hh, 1, @workingDate);

	--SET @arrivalRateSeed = @arrivalRateSeed + 1;
END

/*
SELECT * FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary;
SELECT * FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary;
SELECT * FROM EDDSPerformance.eddsdbo.QoS_SampleHistory;
*/

DROP TABLE #PDB_Servers;

PRINT 'Sample data generation completed';