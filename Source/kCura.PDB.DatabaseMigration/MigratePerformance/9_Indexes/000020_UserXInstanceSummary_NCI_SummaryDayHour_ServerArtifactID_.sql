USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserXInstanceSummary') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UserXInstanceSummary]') AND name = 'NCI_SummaryDayHour_ServerArtifactID')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [NCI_SummaryDayHour_ServerArtifactID] ON [eddsdbo].[QoS_UserXInstanceSummary] 
		(
			[SummaryDayHour] ASC,
			[ServerArtifactID] ASC,
			[QoSHourID] ASC,
			[ArrivalRate] ASC,
			[AVGConcurrency] ASC
		)
		INCLUDE ( [AvgSQScorePerUser])'
		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END