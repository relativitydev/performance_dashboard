USE [EDDSQoS]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_ConcurrencyItems' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ConcurrencyItems]') AND name = N'NCIQoS_hr_VODID')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		--This index is used for the user simple query score calculation in QoS_ConcurrencyServer
		SET @SQL = 'CREATE NONCLUSTERED INDEX [NCIQoS_hr_VODID] ON [EDDSDBO].[QoS_ConcurrencyItems]
		(
			[executiontime] ASC
		)
		INCLUDE ( [QoS_VODID], UserID )'
		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END