USE [EDDSQoS]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_ConcurrencyItems' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ConcurrencyItems]') AND name = N'NCI_QoSHourID')
	BEGIN
		DROP INDEX NCI_QoSHourID ON eddsdbo.QoS_ConcurrencyItems
	END
END

GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_ConcurrencyItems' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ConcurrencyItems]') AND name = N'NCI_QoSHourID')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		--This index is used for the user simple query score calculation in QoS_ConcurrencyServer
		SET @SQL = 'CREATE NONCLUSTERED INDEX [NCI_QoSHourID] ON [eddsdbo].[QoS_ConcurrencyItems]
			(
				[QoSHourID] DESC
			)
			INCLUDE ([QoS_VODID],
				[QoSAction],
				[IsComplex],
				[ExecutionTime],
				[UserID])'
		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END