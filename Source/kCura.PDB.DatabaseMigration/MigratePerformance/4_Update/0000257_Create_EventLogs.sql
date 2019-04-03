use [EddsPerformance] 
 
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'EventLogs' AND TABLE_SCHEMA = N'EDDSDBO')  
BEGIN 
  CREATE TABLE [eddsdbo].[EventLogs]( 
  [EventId] [bigint] NOT NULL, 
  [LogId] [int] NOT NULL, 
   CONSTRAINT [PK_EventLogs] PRIMARY KEY CLUSTERED  
  ( 
    [EventId] ASC, 
    [LogId] ASC 
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] 
  ) ON [PRIMARY] 
 
  ALTER TABLE [eddsdbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_GlassRunLogs] FOREIGN KEY([LogId]) 
  REFERENCES [eddsdbo].[QoS_GlassRunLog] ([GRLogID]) 
END