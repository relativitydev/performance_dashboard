USE [EDDSPerformance]

UPDATE eddsdbo.ProcessControl
set Frequency = -1
where ProcessControlId in (4, 5, 6, 7, 17)