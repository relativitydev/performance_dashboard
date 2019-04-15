update eddsdbo.Events
set StatusId = @timeoutStatus
where (StatusID = @inprogressStatus and LastUpdated <= @timeout) 
OR (StatusID = @pendingHangfireStatus and DATEADD(ss, ISNULL(Delay, 0), LastUpdated) <= @timeout)