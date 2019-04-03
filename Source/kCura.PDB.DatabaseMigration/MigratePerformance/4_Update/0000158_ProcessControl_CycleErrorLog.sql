USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 21)
BEGIN
	INSERT INTO [EDDSPerformance].[eddsdbo].[ProcessControl] (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (21, 'Cycle Error Log', DATEADD(hh, DATEDIFF(hh, 0, getUTCdate()), 0), 10080)
END