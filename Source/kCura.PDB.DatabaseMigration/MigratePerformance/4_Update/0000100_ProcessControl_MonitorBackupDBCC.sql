USE EDDSPerformance
GO
  
IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 10)
BEGIN
	INSERT INTO eddsdbo.ProcessControl (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (10, 'Monitor Backup/DBCC', DATEADD(dd, -1, getutcdate()), 1440)
END