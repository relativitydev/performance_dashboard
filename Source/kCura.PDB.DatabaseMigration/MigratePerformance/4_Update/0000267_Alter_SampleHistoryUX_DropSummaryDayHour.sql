USE [EDDSPerformance]

IF COL_LENGTH ('eddsdbo.QoS_SampleHistoryUX' ,'SummaryDayHour') IS NOT NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_SampleHistoryUX]
    DROP COLUMN [SummaryDayHour]
END