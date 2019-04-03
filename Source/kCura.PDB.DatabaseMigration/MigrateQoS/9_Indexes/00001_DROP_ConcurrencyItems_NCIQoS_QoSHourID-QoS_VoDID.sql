USE EDDSQoS
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ConcurrencyItems]') AND name = N'[NCIQoS_QoSHourID-QoS_VoDID]')
BEGIN
	DROP INDEX [NCIQoS_QoSHourID-QoS_VoDID] ON eddsdbo.QoS_ConcurrencyItems
END