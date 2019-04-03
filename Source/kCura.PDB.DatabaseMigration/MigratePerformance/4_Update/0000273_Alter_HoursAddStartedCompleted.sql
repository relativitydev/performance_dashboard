USE [EDDSPerformance];

-- Add new column
IF COL_LENGTH ('eddsdbo.Hours' ,'StartedOn') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Hours]
    ADD [StartedOn] datetime null
END

IF COL_LENGTH ('eddsdbo.Hours' ,'CompletedOn') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Hours]
    ADD [CompletedOn] datetime null
END