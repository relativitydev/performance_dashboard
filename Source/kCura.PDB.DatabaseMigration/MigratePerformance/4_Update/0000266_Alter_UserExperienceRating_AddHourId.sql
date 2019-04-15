USE [EDDSPerformance]

-- Add new column
IF COL_LENGTH ('eddsdbo.QoS_UserExperienceRatings' ,'HourId') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[QoS_UserExperienceRatings]
    ADD [HourId] int not null DEFAULT 0
END