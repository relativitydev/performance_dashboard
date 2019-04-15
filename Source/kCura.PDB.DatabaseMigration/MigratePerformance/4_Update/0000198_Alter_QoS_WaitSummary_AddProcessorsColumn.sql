USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.QoS_WaitSummary' ,'PercentOfCPUThreshold') IS NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_WaitSummary
    ADD PercentOfCPUThreshold decimal(10,2) null
END