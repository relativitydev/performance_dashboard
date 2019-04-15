USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_SampleHistory') 
BEGIN
	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND name = N'NCI_SummaryDayHour_RowHash')
	BEGIN
		DROP INDEX NCI_SummaryDayHour_RowHash ON eddsdbo.QoS_SampleHistory
	END
END

GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_SampleHistory') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND name = N'NCI_SummaryDayHour_RowHash')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [NCI_SummaryDayHour_RowHash] ON [eddsdbo].[QoS_SampleHistory]
			(
				[SummaryDayHour] ASC
			)
			INCLUDE ([RowHash],
				[QoSHourID],
				[IsActiveWeeklySample])'
		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END