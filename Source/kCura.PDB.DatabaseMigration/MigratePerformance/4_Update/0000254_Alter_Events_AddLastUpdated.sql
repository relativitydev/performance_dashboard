USE [EDDSPerformance]

IF COL_LENGTH ('eddsdbo.Events' ,'LastUpdated') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Events]
    ADD [LastUpdated] datetime not null DEFAULT getutcdate()
END