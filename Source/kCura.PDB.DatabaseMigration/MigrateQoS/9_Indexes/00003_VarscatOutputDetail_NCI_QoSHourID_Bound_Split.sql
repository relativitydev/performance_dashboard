USE [EDDSQoS]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetail' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetail]') AND name = N'NCI_QoSHourID_Bound_Split')
	BEGIN
		CREATE NONCLUSTERED INDEX [NCI_QoSHourID_Bound_Split] ON [eddsdbo].[QoS_VarscatOutputDetail]
        (
			[QoSHourID] ASC,
			[ExecutionTime] ASC
        )
        INCLUDE ([UserID], [QoSAction])
	END
END