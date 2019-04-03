-- EDDSPerformance

DELETE FROM eddsdbo.QoS_BackResults WHERE LoggedDate in @summaryDayHour
DELETE FROM eddsdbo.QoS_DBCCResults WHERE LoggedDate in @summaryDayHour
DELETE FROM eddsdbo.QoS_RecoverabilityIntegritySummary WHERE SummaryDayHour in @summaryDayHour