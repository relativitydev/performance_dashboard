USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.EnvironmentCheckServerDetails' ,'ServerIPAddress') IS NULL
BEGIN
    --added new server ip column
	ALTER TABLE eddsdbo.EnvironmentCheckServerDetails
    ADD ServerIPAddress varchar(100) null
	
	--delete all previous records
	delete from eddsdbo.EnvironmentCheckServerDetails
	
	--update PC to kick off the task to recalculate the server details
	update eddsdbo.ProcessControl
	set LastProcessExecDateTime = DATEADD(year,  DATEDIFF(year, GETUTCDATE(), '1900-1-1'), GETUTCDATE())
	where ProcessControlID = 24
END