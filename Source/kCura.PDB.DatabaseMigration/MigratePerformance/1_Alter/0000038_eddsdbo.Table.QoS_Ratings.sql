USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_Ratings' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_Ratings
	(
		QRatingID int IDENTITY ( 1 , 1 ),PRIMARY KEY (QRatingID)
		,ServerArtifactID int
		,UserExperience4SLRQScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_UserExperience4SLRQScore DEFAULT 100  
		,UserExperienceSLRQScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_UserExperienceSLRQScore DEFAULT 100
		,SystemLoadScoreWeb DECIMAL (5, 2) CONSTRAINT DF_Ratings_SystemLoadScoreWeb DEFAULT 100
		,SystemLoadScoreSQL DECIMAL (5, 2) CONSTRAINT DF_Ratings_SystemLoadScoreSQL DEFAULT 100
		,SystemLoadScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_SystemLoadScore DEFAULT 100 --The lesser of the afore two scores.
		,BackupScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_BackupScore DEFAULT 100
		,DBCCScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_DBCCScore DEFAULT 100
		,WeekUserExperience4SLRQScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_WeekUserExperience4SLRQScore DEFAULT 100
		,WeekUserExperienceSLRQScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_WeekUserExperienceSLRQScore DEFAULT 100
		,WeekSystemLoadScoreWeb DECIMAL (5, 2) CONSTRAINT DF_Ratings_WeekSystemLoadScoreWeb DEFAULT 100
		,WeekSystemLoadScoreSQL DECIMAL (5, 2) CONSTRAINT DF_Ratings_WeekSystemLoadScoreSQL DEFAULT 100
		,WeekSystemLoadScore DECIMAL (5, 2) CONSTRAINT DF_Ratings_WeekSystemLoadScore DEFAULT 100 --The lesser of the afore two scores.
		,SummaryDayHour datetime
		,QoSHourID bigint
		,RowHash binary(20)
	)
END