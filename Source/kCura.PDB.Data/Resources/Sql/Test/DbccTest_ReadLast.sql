-- EDDSPerformance
SELECT TOP (1) [LastCleanDbccDate]
  FROM [eddsdbo].[MockDbccServerResults]
  WHERE [Server] = @server 
  AND [Database] = @database