USE [EDDSQoS]
GO

--Instant file initialization 
update [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
set Recommendation = 'No change needed.'
where ID = 'bd3b8f3a-a27a-46e3-a452-190132db91a3'







