USE EDDSPerformance
GO

UPDATE eddsdbo.ProcessControl
SET Frequency = 60
WHERE ProcessControlID = 3;