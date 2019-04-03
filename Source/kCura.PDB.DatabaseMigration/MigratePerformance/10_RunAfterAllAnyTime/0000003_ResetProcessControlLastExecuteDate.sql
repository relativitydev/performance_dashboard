USE [EDDSPerformance]

UPDATE eddsdbo.ProcessControl
set LastProcessExecDateTime = DATEADD(DAY, DATEDIFF(DAY, 7, GETUTCDATE()), 0)
where (LastExecSucceeded = 0 or LastExecSucceeded is null) and Frequency > -1