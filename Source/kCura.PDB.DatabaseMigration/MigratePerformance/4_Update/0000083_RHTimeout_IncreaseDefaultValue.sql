USE EDDSPerformance
GO

UPDATE eddsdbo.Configuration
SET Value = '3600'
WHERE Section = 'kCura.PDB' AND Name = 'RHTimeoutSeconds'