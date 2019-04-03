/*
	PDB QA Scripts:
	7DayQoSDataScenarios

	Author: Joseph Low
	Date: 7/14/2014

	Comments: This script assumes you have run the SampleDataGenerator script previously. Run portions of the script by highlighting and executing SQL.
		It will manipulate data to match various scenarios.
*/

/*
	Scenario 1:
		QoS_SystemLoadSummary - 11 inactive (no data)
		QoS_UserXInstanceSummary - 7 active (11 total, alternating with single and triple gaps)
*/	

DECLARE @s1min datetime = (SELECT MIN(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

TRUNCATE TABLE EDDSPerformance.eddsdbo.QoS_SystemLoadSummary;

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET ArrivalRate = 0
WHERE SummaryDayHour >= DATEADD(dd, 1, @s1min) AND SummaryDayHour < DATEADD(dd, 2, @s1min);

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET ArrivalRate = 0
WHERE SummaryDayHour >= DATEADD(dd, 3, @s1min) AND SummaryDayHour < DATEADD(dd, 6, @s1min);

/*
	Scenario 2:
		QoS_UserXInstanceSummary - 11 inactive (no data)
		QoS_SystemLoadSummary - 7 active (11 total, alternating with single and triple gaps)
*/	

DECLARE @s2min datetime = (SELECT MIN(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET ArrivalRate = RAND()/10;

DELETE FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
WHERE SummaryDayHour >= DATEADD(dd, 1, @s2min) AND SummaryDayHour < DATEADD(dd, 2, @s2min);

DELETE FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
WHERE SummaryDayHour >= DATEADD(dd, 3, @s2min) AND SummaryDayHour < DATEADD(dd, 6, @s2min);

/*
	Scenario 3:
		QoS_SystemLoadSummary - 11 active (highest values)
		QoS_UserXInstanceSummary - 7 active (11 total, 2 inactive on either side)
*/

DECLARE @s3min datetime = (SELECT MIN(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);
DECLARE @s3max datetime = (SELECT MAX(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

UPDATE EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
SET RAMPagesScore = 100, RAMPctScore = 100, CPUScore = 100;

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET ArrivalRate = 0
WHERE SummaryDayHour < DATEADD(dd, 2, @s3min) OR SummaryDayHour > DATEADD(dd, -2, @s3max);

/*
	Scenario 4:
		QoS_UserXInstanceSummary - 11 active (highest values)
		QoS_SystemLoadSummary - 7 active (11 total, 2 inactive on either side)
*/

DECLARE @s4min datetime = (SELECT MIN(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);
DECLARE @s4max datetime = (SELECT MAX(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET AvgSQScorePerUser = 100;

DELETE FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
WHERE SummaryDayHour < DATEADD(dd, 2, @s4min) OR SummaryDayHour > DATEADD(dd, -2, @s4max);

/*
	Scenario 5:
		QoS_SystemLoadSummary - 11 active (lowest values)
		QoS_UserXInstanceSummary - 7 active (11 total, first 4 days inactive)
*/

DECLARE @s5min datetime = (SELECT MIN(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

UPDATE EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
SET RAMPagesScore = 0, RAMPctScore = 0, CPUScore = 0;

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET ArrivalRate = 0
WHERE SummaryDayHour < DATEADD(dd, 4, @s5min);

/*
	Scenario 6:
		QoS_UserXInstanceSummary - 11 active (lowest values)
		QoS_SystemLoadSummary - 7 active (11 total, first 4 days inactive)
*/

DECLARE @s6min datetime = (SELECT MIN(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET AvgSQScorePerUser = 0;

DELETE FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
WHERE SummaryDayHour < DATEADD(dd, 4, @s6min);

/*
	Scenario 7:
		QoS_UserXInstanceSummary - 7 active (11 total, most recent 4 inactive, mixed values)
		QoS_SystemLoadSummary - 7 active (11 total, most recent 4 inactive, mixed values)
*/

DECLARE @s7max datetime = (SELECT MAX(SummaryDayHour) FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary);

UPDATE EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary
SET ArrivalRate = 0
WHERE SummaryDayHour > DATEADD(dd, -4, @s7max);

DELETE FROM EDDSPerformance.eddsdbo.QoS_SystemLoadSummary
WHERE SummaryDayHour > DATEADD(dd, -4, @s7max);