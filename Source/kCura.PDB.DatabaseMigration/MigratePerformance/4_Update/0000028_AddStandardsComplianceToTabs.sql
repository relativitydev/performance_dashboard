USE [EDDS]
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ApplicationPerformance.aspx?StandardsCompliance=true'
  WHERE Name = 'Application Performance'
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ServerHealth.aspx?StandardsCompliance=true'
  WHERE Name = 'Server Health'
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ServiceQuality.aspx?StandardsCompliance=true'
  WHERE Name = 'Quality of Service'
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/UserExperience.aspx?StandardsCompliance=true'
  WHERE Name = 'User Experience'
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/SystemLoad.aspx?StandardsCompliance=true'
  WHERE Name = 'System Load'
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/Backup.aspx?StandardsCompliance=true'
  WHERE Name = 'Backup and DBCC Checks'
UPDATE [eddsdbo].[Tab]
  SET ExternalLink = '%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/Uptime.aspx?StandardsCompliance=true'
  WHERE Name = 'Uptime'