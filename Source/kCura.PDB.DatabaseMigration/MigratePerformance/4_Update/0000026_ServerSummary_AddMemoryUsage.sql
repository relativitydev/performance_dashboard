USE [EDDSPerformance]
GO

ALTER TABLE eddsdbo.ServerSummary
ADD TotalPhysicalMemory decimal(10,0) null;
GO
  
ALTER TABLE eddsdbo.ServerSummary
ADD AvailableMemory decimal(10,0) null;
GO

ALTER TABLE eddsdbo.ServerSummary
ADD RAMPct decimal(10,2) null;