USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_VarscatOutputCumulative') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputCumulative]') AND name = N'NCI_SummaryDayHour_QoSHourID')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [NCI_SummaryDayHour_QoSHourID] ON [eddsdbo].[QoS_VarscatOutputCumulative] 
	(
		[SummaryDayHour] ASC
	)
	INCLUDE ([QoSHourID])'		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END