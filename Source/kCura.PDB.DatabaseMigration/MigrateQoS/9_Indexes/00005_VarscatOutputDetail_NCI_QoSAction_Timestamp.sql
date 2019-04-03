USE [EDDSQoS]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);
IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetail' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetail]') AND name = N'NCI_QoSAction_Timestamp')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [NCI_QoSAction_Timestamp] ON [eddsdbo].[QoS_VarscatOutputDetail]
(
	[QoSAction] ASC,
	[Timestamp] ASC
)'
		
		IF (@enterprise = 1)
			SET @SQL += ' WITH(ONLINE = ON)'
			
		EXEC sp_executesql @SQL
	END
END