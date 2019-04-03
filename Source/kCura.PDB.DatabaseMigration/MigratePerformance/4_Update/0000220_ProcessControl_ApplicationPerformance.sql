USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 27)
BEGIN
	INSERT INTO eddsdbo.ProcessControl (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (27, 'Application Performance', DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0), 1)
END