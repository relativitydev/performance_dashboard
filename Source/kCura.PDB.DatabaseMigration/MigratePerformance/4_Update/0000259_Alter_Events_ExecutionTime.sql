USE [EDDSPerformance]

IF COL_LENGTH ('eddsdbo.Events' ,'ExecutionTime') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Events]
    ADD [ExecutionTime] int null
END