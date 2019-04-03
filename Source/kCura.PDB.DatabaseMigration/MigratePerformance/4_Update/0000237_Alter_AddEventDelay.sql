USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.Events' ,'Delay') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Events]
    ADD Delay int null
END