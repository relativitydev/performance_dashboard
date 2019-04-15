USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 23)
BEGIN
	INSERT INTO eddsdbo.ProcessControl (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (23, 'Environment Check Relativity Config', DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0), 1440)
END
else
BEGIN
	update eddsdbo.ProcessControl
	set ProcessTypeDesc = 'Environment Check Relativity Config'
	where ProcessControlID = 23
end

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 24)
BEGIN
	INSERT INTO eddsdbo.ProcessControl (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (24, 'Environment Check Server Info', DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0), 1440)
END