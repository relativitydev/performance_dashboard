USE EDDSPerformance;
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_SampleHistory' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_SampleHistory', 'IsActiveWeeklySample') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_SampleHistory ADD IsActiveWeeklySample bit
	END
	
	IF COL_LENGTH('eddsdbo.QoS_SampleHistory', 'IsActiveWeekly4Sample') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_SampleHistory ADD IsActiveWeekly4Sample bit
	END
END

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_Ratings' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_Ratings', 'WeekUserExperience4SLRQScore') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings ADD WeekUserExperience4SLRQScore DECIMAL (5, 2)
		CONSTRAINT DF_Ratings_WeekUserExperience4SLRQScore DEFAULT 100
	END
	
	IF COL_LENGTH('eddsdbo.QoS_Ratings', 'WeekUserExperienceSLRQScore') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings ADD WeekUserExperienceSLRQScore DECIMAL (5, 2)
		CONSTRAINT DF_Ratings_WeekUserExperienceSLRQScore DEFAULT 100
	END
	
	IF COL_LENGTH('eddsdbo.QoS_Ratings', 'WeekSystemLoadScoreWeb') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings ADD WeekSystemLoadScoreWeb DECIMAL (5, 2)
		CONSTRAINT DF_Ratings_WeekSystemLoadScoreWeb DEFAULT 100
	END
	
	IF COL_LENGTH('eddsdbo.QoS_Ratings', 'WeekSystemLoadScoreSQL') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings ADD WeekSystemLoadScoreSQL DECIMAL (5, 2)
		CONSTRAINT DF_Ratings_WeekSystemLoadScoreSQL DEFAULT 100
	END
	
	IF COL_LENGTH('eddsdbo.QoS_Ratings', 'WeekSystemLoadScore') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings ADD WeekSystemLoadScore DECIMAL (5, 2)
		CONSTRAINT DF_Ratings_WeekSystemLoadScore DEFAULT 100
	END
END