USE [EDDSPerformance]
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_VarscatOutputCumulative' AND TABLE_SCHEMA = N'EDDSDBO') AND COL_LENGTH('eddsdbo.QoS_VarscatOutputCumulative', 'ServerID') IS NULL
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	ADD ServerID int
END

GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = N'EDDSDBO') AND COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'ServerID') IS NULL
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
    ADD ServerID int
END
