/*
	PDB QA Scripts:
	Generate Ratings from Sample Data

	Author: Joseph Low
	Date: 7/14/2014

	Comments: This script assumes you have run the SampleDataGenerator script previously as well as the desired Scenario from 7DayQoSDataScenarios. Automated Looking Glass calls should be disabled at this time.
		The procedure call will pull data into QoS_SampleHistory and redetermine whether all SampleHistory hours fall into the quarterly and weekly
		sample sets. It will also generate scores for you in QoS_Ratings and QoS_UptimeRatings.
*/

SET NOCOUNT ON

DECLARE @workingHour datetime = (SELECT TOP 1 SummaryDayHour FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary ORDER BY SummaryDayHour DESC)
DECLARE @qosHourID bigint = (SELECT TOP 1 QoSHourID FROM EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary WHERE SummaryDayHour = @workingHour)

IF (@qosHourID IS NULL)
BEGIN
	PRINT 'Halting due to null QoSHourID - did you run the sample data generator?'
	RETURN;
END

EXEC EDDSPerformance.eddsdbo.QoS_BuildandRateSample
	@QoShourID = @qosHourID,
	@summaryDayHour = @workingHour,
	@glassRunID = 1 --This is really just a dummy value