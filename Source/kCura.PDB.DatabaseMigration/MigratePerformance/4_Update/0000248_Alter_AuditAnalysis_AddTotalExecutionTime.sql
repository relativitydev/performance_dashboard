USE [EDDSPerformance]


IF COL_LENGTH ('eddsdbo.MetricData_AuditAnalysis' ,'TotalExecutionTime') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[MetricData_AuditAnalysis]
    ADD [TotalExecutionTime] bigint
END