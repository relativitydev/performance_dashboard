USE EDDSPerformance

UPDATE [eddsdbo].[ProcessControl]
SET Frequency = @frequency
WHERE ProcessControlID = @processControlId