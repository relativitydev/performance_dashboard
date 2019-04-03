USE EDDSPerformance
GO

DELETE FROM eddsdbo.Configuration
WHERE Section = 'kCura.PDB' AND Name = 'NotificationsUseSSL'