USE [EDDSPerformance]
GO

ALTER TABLE eddsdbo.ServerDW
ADD TotalPhysicalMemory decimal(10,0) null;
GO
  
ALTER TABLE eddsdbo.ServerDW
ADD AvailableMemory decimal(10,0) null;
GO

ALTER TABLE eddsdbo.ServerDW
ADD RAMPct decimal(10,2) null;