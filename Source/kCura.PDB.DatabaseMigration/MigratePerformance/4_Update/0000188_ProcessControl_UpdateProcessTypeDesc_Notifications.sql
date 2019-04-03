USE EDDSPerformance

GO

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET ProcessTypeDesc = 'Quarterly Score Alerts'
WHERE ProcessControlID = 11

GO

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET ProcessTypeDesc = 'Quarterly Score Status'
WHERE ProcessControlID = 12

GO

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET ProcessTypeDesc = 'Configuration Change Alerts'
WHERE ProcessControlID = 13

GO

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET ProcessTypeDesc = 'Weekly Score Alerts'
WHERE ProcessControlID = 14

GO

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET ProcessTypeDesc = 'Trust Delivery Alerts'
WHERE ProcessControlID = 15

GO

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET ProcessTypeDesc = 'Recoverability/Integrity Alerts'
WHERE ProcessControlID = 16

GO

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET ProcessTypeDesc = 'Infrastructure Performance Forecast'
WHERE ProcessControlID = 18