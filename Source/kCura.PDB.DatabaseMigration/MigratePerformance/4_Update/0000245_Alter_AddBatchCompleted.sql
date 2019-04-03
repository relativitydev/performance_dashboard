USE [EDDSPerformance]


IF COL_LENGTH ('eddsdbo.SearchAuditBatch' ,'Completed') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[SearchAuditBatch]
    ADD [Completed] bit not null DEFAULT 0
END