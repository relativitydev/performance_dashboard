USE [EDDSPerformance]


IF COL_LENGTH ('eddsdbo.Events' ,'PreviousEventID') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Events]
    ADD [PreviousEventID] bigint null
END