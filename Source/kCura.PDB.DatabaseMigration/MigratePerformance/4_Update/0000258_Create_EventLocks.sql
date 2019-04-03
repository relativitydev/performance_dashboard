USE [EDDSPerformance]

CREATE TABLE [eddsdbo].[EventLocks](
	[Id] [bigint] CONSTRAINT [PK_EventLocks] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[EventTypeId] [int] NOT NULL,
	[SourceId] [bigint] NULL,
	[EventId] [bigint] NOT NULL,
	CONSTRAINT UQ_EventTypeId_SourceId UNIQUE (EventTypeId, SourceId)
 )