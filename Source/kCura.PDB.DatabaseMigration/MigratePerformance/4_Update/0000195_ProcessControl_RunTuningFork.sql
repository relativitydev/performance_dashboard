USE EDDSPerformance
GO
  
IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 23)
BEGIN
	INSERT INTO eddsdbo.ProcessControl (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (23, 'Run Tuning Fork', DATEADD(hh, DATEDIFF(hh, 0, getutcdate()) - 1, 0), 1440)
END