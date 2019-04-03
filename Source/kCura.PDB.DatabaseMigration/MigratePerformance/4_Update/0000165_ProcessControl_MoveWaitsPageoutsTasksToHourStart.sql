USE EDDSPerformance
GO

UPDATE [EDDSPerformance].[eddsdbo].[ProcessControl]
SET LastProcessExecDateTime = DATEADD(hh, DATEDIFF(hh, 0, LastProcessExecDateTime), 0)
WHERE ProcessControlID IN ( 9, 20 )