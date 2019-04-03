USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 19)
BEGIN
	INSERT INTO [EDDSPerformance].[eddsdbo].[ProcessControl] (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (19, 'User Experience Forecast', DATEADD(MINUTE, 10, DATEADD(hh, DATEDIFF(hh, 0, getUTCdate()), 0)), 60)
END