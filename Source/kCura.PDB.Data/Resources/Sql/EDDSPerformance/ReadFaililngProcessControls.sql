USE EDDSPerformance

select * from eddsdbo.ProcessControl as pc
where pc.LastExecSucceeded = 0 and pc.Frequency > 0