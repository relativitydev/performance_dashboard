  IF NOT EXISTS (SELECT * FROM [EDDSPerformance].[eddsdbo].[Configuration] WHERE Name = 'UpgradeWillPurgeQoSTables')
  INSERT INTO [EDDSPerformance].[eddsdbo].[Configuration]
  (Section, Name, Value, MachineName, [Description]) VALUES
  ('kCura.PDB', 'UpgradeWillPurgeQoSTables', 'False', '',
  'When this value is set to True, upgrades to Performance Dashboard will destroy QoS tables in EDDSPerformance, EDDSResource, and EDDSQoS.')