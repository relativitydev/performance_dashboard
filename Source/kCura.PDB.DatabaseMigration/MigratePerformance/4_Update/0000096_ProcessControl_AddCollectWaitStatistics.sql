USE EDDSPerformance
GO
  
IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 9)
BEGIN
	INSERT INTO eddsdbo.ProcessControl (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (9, 'Collect Wait Statistics', DATEADD(hh, -1, getutcdate()), 60)
END