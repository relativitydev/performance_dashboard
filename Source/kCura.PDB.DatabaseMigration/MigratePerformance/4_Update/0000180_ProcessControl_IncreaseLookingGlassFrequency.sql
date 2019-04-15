USE EDDSPerformance
GO

UPDATE eddsdbo.ProcessControl
SET Frequency = 5
WHERE ProcessControlID = 7