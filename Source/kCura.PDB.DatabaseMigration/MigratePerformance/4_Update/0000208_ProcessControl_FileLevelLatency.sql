USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 25)
BEGIN
	INSERT INTO eddsdbo.ProcessControl (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (25, 'Database File Level Latency', DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0), 60)
END