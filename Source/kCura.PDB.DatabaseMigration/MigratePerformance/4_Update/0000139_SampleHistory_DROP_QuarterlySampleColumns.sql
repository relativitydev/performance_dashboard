USE EDDSPerformance
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_SampleHistory' AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND name = N'NCI_IsActiveSample')
	BEGIN
		DROP INDEX [NCI_IsActiveSample] ON [eddsdbo].[QoS_SampleHistory] WITH ( ONLINE = OFF )
	END

	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SampleHistory]') AND name = N'NCI_IsActive4Sample')
	BEGIN
		DROP INDEX [NCI_IsActive4Sample] ON [eddsdbo].[QoS_SampleHistory] WITH ( ONLINE = OFF )
	END

	IF COL_LENGTH('eddsdbo.QoS_SampleHistory', 'IsActiveSample') IS NOT NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_SampleHistory
		DROP COLUMN IsActiveSample
	END

	IF COL_LENGTH('eddsdbo.QoS_SampleHistory', 'IsActive4Sample') IS NOT NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_SampleHistory
		DROP COLUMN IsActive4Sample
	END
END