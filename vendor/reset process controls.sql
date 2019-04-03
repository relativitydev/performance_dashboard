use EDDSPerformance

update eddsdbo.ProcessControl
set LastProcessExecDateTime = '1900-01-01 00:00:00.001'
where Frequency > 0 

declare @remaining int = (select count(*) from eddsdbo.ProcessControl where Frequency > 0 and LastExecSucceeded is not null and LastExecSucceeded = 1 and LastProcessExecDateTime = '1900-01-01 00:00:00.001')

while (0 < @remaining)
begin
	set @remaining = (select count(*) from eddsdbo.ProcessControl where Frequency > 0 and LastExecSucceeded is not null and LastExecSucceeded = 1 and LastProcessExecDateTime = '1900-01-01 00:00:00.001')
	WAITFOR DELAY '00:00:10'
end

print 'done'

select count(*) as Failed, GETUTCDATE() from eddsdbo.ProcessControl where LastExecSucceeded = 0

--select * from eddsdbo.ProcessControl