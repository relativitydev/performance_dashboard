USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 13)
BEGIN
	INSERT INTO [EDDSPerformance].[eddsdbo].[ProcessControl] (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (13, 'Reconfiguration Notification', GETUTCDATE(), -1)
END