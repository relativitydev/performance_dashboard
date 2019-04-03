  USE EDDSPerformance;
  GO
  
  UPDATE [EDDSPerformance].[eddsdbo].[ProcessControl]
  SET Frequency = -1 WHERE ProcessControlID = 4