USE [EDDSPerformance];
GO

IF NOT EXISTS (SELECT TOP 1 * FROM [EDDSPerformance].[eddsdbo].[Configuration] WHERE Section = 'kCura.PDB' AND Name = 'EnableLookingGlassLogging')
  INSERT INTO [EDDSPerformance].[eddsdbo].[Configuration] (Section, Name, Value, MachineName, [Description])
  VALUES ('kCura.PDB', 'EnableLookingGlassLogging', 'True', '', 'When this value is set to False, Looking Glass will be executed without logging. By default, the value is set to True, and logging will occur.')
  