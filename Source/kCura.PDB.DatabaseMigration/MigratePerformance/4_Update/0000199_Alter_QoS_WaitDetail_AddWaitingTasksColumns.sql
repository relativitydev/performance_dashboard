USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.QoS_WaitDetail' ,'CumulativeWaitingTasksCount') IS NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_WaitDetail
    ADD CumulativeWaitingTasksCount bigint null
END

IF COL_LENGTH ('eddsdbo.QoS_WaitDetail' ,'DifferentialWaitingTasksCount') IS NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_WaitDetail
    ADD DifferentialWaitingTasksCount bigint null
END
