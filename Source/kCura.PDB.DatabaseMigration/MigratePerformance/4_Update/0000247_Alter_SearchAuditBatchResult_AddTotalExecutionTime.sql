USE [EDDSPerformance]


IF COL_LENGTH ('eddsdbo.SearchAuditBatchResult' ,'TotalExecutionTime') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[SearchAuditBatchResult]
    ADD [TotalExecutionTime] bigint
END