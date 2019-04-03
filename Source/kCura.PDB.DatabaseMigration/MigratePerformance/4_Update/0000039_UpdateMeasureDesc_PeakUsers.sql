USE EDDSPerformance;
GO

UPDATE EDDSPerformance.eddsdbo.Measure
SET MeasureDesc = 'Peak Users'
WHERE MeasureID = 4;