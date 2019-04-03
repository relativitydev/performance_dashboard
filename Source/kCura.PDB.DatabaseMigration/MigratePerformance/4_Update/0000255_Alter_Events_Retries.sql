USE [EDDSPerformance]

IF COL_LENGTH ('eddsdbo.Events' ,'Retries') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Events]
    ADD [Retries] int null
END