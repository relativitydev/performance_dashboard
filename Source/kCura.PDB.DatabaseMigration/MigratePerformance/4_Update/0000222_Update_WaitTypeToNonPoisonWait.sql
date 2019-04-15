USE EDDSPerformance
GO

update EDDSPerformance.eddsdbo.QoS_Waits
set IsPoisonWait = 0
where WaitType = 'LCK_M_RS_U'