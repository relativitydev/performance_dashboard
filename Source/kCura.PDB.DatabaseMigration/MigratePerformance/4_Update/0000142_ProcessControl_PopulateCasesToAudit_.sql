USE EDDSPerformance
GO

IF NOT EXISTS (SELECT TOP 1 ProcessControlID FROM eddsdbo.ProcessControl WHERE ProcessControlID = 17)
BEGIN
	INSERT INTO [EDDSPerformance].[eddsdbo].[ProcessControl] (ProcessControlID, ProcessTypeDesc, LastProcessExecDateTime, Frequency)
	VALUES (17, 'Populate Cases to Audit', DATEADD(HH, DATEDIFF(hh, 0, getUTCdate()) - 1, 0), 60)
END