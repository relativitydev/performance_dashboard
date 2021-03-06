/****** Object:  Schema [eddsdbo]    Script Date: 8/7/2018 8:34:20 PM ******/
CREATE SCHEMA [eddsdbo]
GO
/****** Object:  Schema [HangFire]    Script Date: 8/7/2018 8:34:20 PM ******/
CREATE SCHEMA [HangFire]
GO
/****** Object:  Table [eddsdbo].[AgentHistory]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[AgentHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AgentArtifactId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Successful] [bit] NOT NULL,
 CONSTRAINT [PK_AgentHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Categories]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Categories](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryTypeID] [int] NOT NULL,
	[HourID] [int] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[CategoryScores]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[CategoryScores](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[ServerID] [int] NULL,
	[Score] [decimal](9, 0) NULL,
 CONSTRAINT [PK_CategoryScores] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[CategoryTypes]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[CategoryTypes](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_CategoryTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Configuration]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Configuration](
	[Section] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[MachineName] [varchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[Section] ASC,
	[Name] ASC,
	[MachineName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ConfigurationAudit]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ConfigurationAudit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FieldName] [nvarchar](200) NOT NULL,
	[ServerName] [nvarchar](150) NULL,
	[OldValue] [nvarchar](max) NOT NULL,
	[NewValue] [nvarchar](max) NOT NULL,
	[UserID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_ConfigurationHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[DatabaseGaps]    Script Date: 11/10/2018 3:49:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[DatabaseGaps](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DatabaseId] [int] NOT NULL,
	[GapStart] [datetime] NOT NULL,
	[GapEnd] [datetime] NULL,
	[Duration] [int] NULL,
	[ActivityType] [int] NOT NULL,
 CONSTRAINT [PK_DatabaseGaps] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[DbccInfoResults]    Script Date: 11/21/2018 3:18:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[DbccInfoResults](
	[ParentObject] [nvarchar](255) NULL,
	[Object] [nvarchar](255) NULL,
	[Field] [nvarchar](255) NULL,
	[Value] [varchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[DBCCTarget]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[DBCCTarget](
	[DbccTargetId] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](150) NOT NULL,
	[DatabaseName] [nvarchar](150) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_DbccTargetId] PRIMARY KEY CLUSTERED 
(
	[DbccTargetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EnvironmentCheckDatabaseDetails]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EnvironmentCheckDatabaseDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [varchar](150) NULL,
	[SQLVersion] [varchar](150) NULL,
	[AdHocWorkLoad] [int] NULL,
	[MaxServerMemory] [bigint] NULL,
	[MaxDegreeOfParallelism] [int] NULL,
	[TempDBDataFiles] [int] NULL,
	[LastSQLRestart] [datetime] NULL,
 CONSTRAINT [PK_EnvironmentCheckDatabaseDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EnvironmentCheckRecommendations]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EnvironmentCheckRecommendations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Scope] [varchar](200) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Description] [varchar](max) NULL,
	[Status] [varchar](100) NOT NULL,
	[Recommendation] [varchar](max) NULL,
	[Value] [varchar](max) NULL,
	[Section] [varchar](200) NOT NULL,
	[Severity] [int] NULL,
 CONSTRAINT [PK_EnvironmentCheckRecommendations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EnvironmentCheckServerDetails]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EnvironmentCheckServerDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [varchar](150) NULL,
	[OSVersion] [varchar](150) NULL,
	[OSName] [varchar](150) NULL,
	[LogicalProcessors] [int] NULL,
	[Hyperthreaded] [bit] NULL,
	[ServerIPAddress] [varchar](100) NULL,
 CONSTRAINT [PK_EnvironmentCheckServerDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ErrorCountDW]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ErrorCountDW](
	[ErrorCountDWID] [int] IDENTITY(1,1) NOT NULL,
	[MeasureDate] [datetime] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[ErrorCount] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_ErrorCountDW] PRIMARY KEY CLUSTERED 
(
	[ErrorCountDWID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EventLocks]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EventLocks](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EventTypeId] [int] NOT NULL,
	[SourceId] [bigint] NULL,
	[EventId] [bigint] NOT NULL,
	[WorkerId] [int] NOT NULL,
 CONSTRAINT [PK_EventLocks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_EventTypeId_SourceId] UNIQUE NONCLUSTERED 
(
	[EventTypeId] ASC,
	[SourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EventLogs]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EventLogs](
	[EventId] [bigint] NOT NULL,
	[LogId] [int] NOT NULL,
 CONSTRAINT [PK_EventLogs] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC,
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Events]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Events](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SourceTypeID] [int] NOT NULL,
	[SourceID] [int] NULL,
	[StatusID] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[EventHash] [varchar](max) NULL,
	[Delay] [int] NULL,
	[PreviousEventID] [bigint] NULL,
	[LastUpdated] [datetime] NOT NULL,
	[Retries] [int] NULL,
	[ExecutionTime] [int] NULL,
	[HourId] [int] NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EventSourceSystemControl]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EventSourceSystemControl](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[State] [int] NOT NULL,
 CONSTRAINT [PK_EventSourceSystemControl] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EventTypes]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EventTypes](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_EventTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[EventWorkers]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[EventWorkers](
	[Id] [int] NOT NULL,
	[Name] [varchar](100) NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_EventWorkers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[FileLevelLatencyDetails]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[FileLevelLatencyDetails](
	[ServerName] [varchar](200) NOT NULL,
	[DatabaseName] [nvarchar](255) NOT NULL,
	[Score] [decimal](5, 2) NULL,
	[DataReadLatency] [bigint] NULL,
	[DataWriteLatency] [bigint] NULL,
	[LogReadLatency] [bigint] NULL,
	[LogWriteLatency] [bigint] NULL,
 CONSTRAINT [PK_FileLevelLatencyDetails] PRIMARY KEY CLUSTERED 
(
	[ServerName] ASC,
	[DatabaseName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Hours]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Hours](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HourTimeStamp] [datetime] NOT NULL,
	[Score] [decimal](9, 0) NULL,
	[InSample] [bit] NOT NULL,
	[StartedOn] [datetime] NULL,
	[CompletedOn] [datetime] NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Hours] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[HourSearchAuditBatches]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[HourSearchAuditBatches](
	[HourId] [int] NOT NULL,
	[ServerId] [int] NOT NULL,
	[BatchesCreated] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_HourSearchAuditBatches] PRIMARY KEY CLUSTERED 
(
	[HourId] ASC,
	[ServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[LogCategories]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[LogCategories](
	[Name] [varchar](50) NOT NULL,
	[LogLevel] [int] NOT NULL,
 CONSTRAINT [PK_LogCategories] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[MaintenanceSchedules]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[MaintenanceSchedules](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[Reason] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_MaintenanceSchedules] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Measure]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Measure](
	[MeasureID] [int] IDENTITY(1,1) NOT NULL,
	[MeasureCd] [varchar](100) NOT NULL,
	[MeasureDesc] [varchar](500) NOT NULL,
	[MeasureTypeId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[Frequency] [int] NOT NULL,
	[LastDataLoadDateTime] [datetime] NULL,
 CONSTRAINT [PK_Measure] PRIMARY KEY CLUSTERED 
(
	[MeasureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[MeasureType]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[MeasureType](
	[MeasureTypeId] [int] NOT NULL,
	[MeasureTypeCd] [varchar](50) NOT NULL,
	[MeasureTypeDesc] [varchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NULL,
 CONSTRAINT [PK_MeasureType] PRIMARY KEY CLUSTERED 
(
	[MeasureTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[MetricData]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[MetricData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MetricID] [int] NOT NULL,
	[ServerID] [int] NULL,
	[Data] [varchar](max) NULL,
	[Score] [decimal](9, 0) NULL,
 CONSTRAINT [PK_MetricData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[MetricData_AuditAnalysis]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[MetricData_AuditAnalysis](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MetricDataId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[TotalComplexQueries] [bigint] NOT NULL,
	[TotalLongRunningQueries] [bigint] NOT NULL,
	[TotalSimpleLongRunningQueries] [bigint] NOT NULL,
	[TotalQueries] [bigint] NOT NULL,
	[TotalExecutionTime] [bigint] NULL,
 CONSTRAINT [PK_MetricData_AuditAnalysis] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[MetricManagerExecutionStats]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[MetricManagerExecutionStats](
	[ExecutionId] [uniqueidentifier] NOT NULL,
	[Start] [datetime] NOT NULL,
	[End] [datetime] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[TotalTime] [decimal](18, 4) NOT NULL,
	[MaxTime] [decimal](18, 4) NOT NULL,
	[Count] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Metrics]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Metrics](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MetricTypeID] [int] NOT NULL,
	[HourID] [int] NOT NULL,
 CONSTRAINT [PK_Metrics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[MetricTypes]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[MetricTypes](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](max) NULL,
	[SampleType] [int] NOT NULL,
 CONSTRAINT [PK_MetricTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[MetricTypesToCategoryTypes]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[MetricTypesToCategoryTypes](
	[MetricTypeID] [int] NOT NULL,
	[CategoryTypeID] [int] NOT NULL,
 CONSTRAINT [PK_MetricTypesToCategoryTypes] PRIMARY KEY CLUSTERED 
(
	[MetricTypeID] ASC,
	[CategoryTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[PerformanceSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[PerformanceSummary](
	[PerformanceSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[MeasureDate] [datetime] NOT NULL,
	[UserCount] [int] NULL,
	[ErrorCount] [int] NULL,
	[LRQCount] [int] NULL,
 CONSTRAINT [PK_PerformanceSummary] PRIMARY KEY CLUSTERED 
(
	[PerformanceSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ProcessControl]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ProcessControl](
	[ProcessControlID] [int] NOT NULL,
	[ProcessTypeDesc] [nvarchar](200) NOT NULL,
	[LastProcessExecDateTime] [datetime] NOT NULL,
	[Frequency] [int] NULL,
	[LastExecSucceeded] [bit] NULL,
	[LastErrorMessage] [varchar](max) NULL,
 CONSTRAINT [PK_ProcessControl] PRIMARY KEY CLUSTERED 
(
	[ProcessControlID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_ActiveHours]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_ActiveHours](
	[ActiveHoursID] [int] IDENTITY(1,1) NOT NULL,
	[AuditStartDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ActiveHoursID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_AllDatabasesChecked]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_AllDatabasesChecked](
	[DatabaseID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](150) NULL,
	[DBName] [nvarchar](255) NULL,
	[CaseArtifactID] [int] NULL,
	[DateCreated] [datetime] NULL,
	[IsCompleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[DatabaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_BackResults]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_BackResults](
	[kdbbuID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](255) NULL,
	[DBName] [nvarchar](255) NULL,
	[CaseArtifactID] [int] NULL,
	[dateDBCreated] [datetime] NULL,
	[LastBackupDate] [datetime] NULL,
	[BackupStatus] [int] NULL,
	[DaysSinceLast] [int] NULL,
	[LoggedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[kdbbuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_BackSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_BackSummary](
	[kdbsID] [int] IDENTITY(1,1) NOT NULL,
	[DBName] [nvarchar](255) NULL,
	[CaseArtifactID] [int] NULL,
	[LastBackupDate] [datetime] NULL,
	[EntryDate] [datetime] NULL,
	[WindowExceededBy] [int] NULL,
	[GapResolvedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[kdbsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_BackupHistory]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_BackupHistory](
	[BackupHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[DBName] [nvarchar](255) NULL,
	[CompletedOn] [datetime] NULL,
	[Duration] [int] NULL,
	[BackupType] [char](1) NULL,
	[LoggedDate] [datetime] NULL,
 CONSTRAINT [PK_BackupHistory] PRIMARY KEY CLUSTERED 
(
	[BackupHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_CasesToAudit]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_CasesToAudit](
	[RowID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](150) NULL,
	[ServerID] [int] NULL,
	[CaseArtifactID] [int] NULL,
	[DatabaseName] [nvarchar](128) NULL,
	[WorkspaceName] [varchar](50) NULL,
	[AuditStartDate] [datetime] NULL,
	[Retry] [int] NULL,
	[IsCompleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[IsFailedThisRun] [bit] NULL,
	[RowHash] [binary](20) NULL,
	[AgentID] [varchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_DBCCBACKKEY]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_DBCCBACKKEY](
	[KID] [int] IDENTITY(1,1) NOT NULL,
	[run] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[KID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_DBCCHistory]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_DBCCHistory](
	[DbccHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[DBName] [nvarchar](255) NULL,
	[CompletedOn] [datetime] NULL,
	[IsCommandBased] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[DbccHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_DBCCResults]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_DBCCResults](
	[kdbccbID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](255) NULL,
	[DBName] [nvarchar](255) NULL,
	[CaseArtifactID] [int] NULL,
	[dateDBCreated] [datetime] NULL,
	[LastCleanDBCCDate] [datetime] NULL,
	[DBCCStatus] [int] NULL,
	[DaysSinceLast] [int] NULL,
	[LoggedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[kdbccbID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_DBCCSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_DBCCSummary](
	[kdbbcsID] [int] IDENTITY(1,1) NOT NULL,
	[DBName] [nvarchar](255) NULL,
	[CaseArtifactID] [int] NULL,
	[LastDBCCDate] [datetime] NULL,
	[EntryDate] [datetime] NULL,
	[WindowExceededBy] [int] NULL,
	[GapResolvedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[kdbbcsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_FailedDatabases]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_FailedDatabases](
	[DatabaseID] [int] IDENTITY(1,1) NOT NULL,
	[DBName] [nvarchar](255) NULL,
	[ServerName] [nvarchar](255) NULL,
	[Errors] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[DatabaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  Table [eddsdbo].[QoS_FileLatencySummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_FileLatencySummary](
	[FileLatencyID] [int] IDENTITY(1,1) NOT NULL,
	[ServerArtifactID] [int] NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[HighestLatencyDatabase] [nvarchar](255) NULL,
	[ReadLatencyMs] [bigint] NULL,
	[WriteLatencyMs] [bigint] NULL,
	[LatencyScore] [decimal](5, 2) NULL,
	[IOWaitsHigh] [bit] NULL,
	[IsDataFile] [bit] NULL,
 CONSTRAINT [PK_FileLatencyID] PRIMARY KEY CLUSTERED 
(
	[FileLatencyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_GapSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_GapSummary](
	[ServerArtifactID] [int] NULL,
	[ServerName] [nvarchar](255) NULL,
	[DatabaseName] [nvarchar](50) NOT NULL,
	[WorkspaceName] [varchar](50) NULL,
	[CaseArtifactID] [int] NOT NULL,
	[IsBackup] [bit] NOT NULL,
	[LastActivityDate] [datetime] NULL,
	[ResolutionDate] [datetime] NULL,
	[GapSize] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_GlassRunHistory]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_GlassRunHistory](
	[GlassRunID] [int] IDENTITY(1,1) NOT NULL,
	[RunDateTime] [datetime] NULL,
	[RunDuration] [int] NULL,
	[FailedCases] [int] NULL,
	[isActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[GlassRunID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_GlassRunLog]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_GlassRunLog](
	[GRLogID] [int] IDENTITY(1,1) NOT NULL,
	[LogTimestampUTC] [datetime] NULL,
	[Module] [varchar](100) NULL,
	[TaskCompleted] [varchar](500) NULL,
	[OtherVars] [varchar](max) NULL,
	[NextTask] [varchar](500) NULL,
	[AgentID] [int] NULL,
	[LogLevel] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[GRLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_MonitoringExclusions]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_MonitoringExclusions](
	[ExclusionId] [int] IDENTITY(1,1) NOT NULL,
	[ExclusionName] [nvarchar](255) NOT NULL,
	[SkipDBCC] [bit] NOT NULL,
	[SkipBackups] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ExclusionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_MonitoringExclusions_DBName] UNIQUE NONCLUSTERED 
(
	[ExclusionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_Ratings]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_Ratings](
	[QRatingID] [int] IDENTITY(1,1) NOT NULL,
	[ServerArtifactID] [int] NULL,
	[UserExperience4SLRQScore] [decimal](5, 2) NULL,
	[UserExperienceSLRQScore] [decimal](5, 2) NULL,
	[SystemLoadScoreWeb] [decimal](5, 2) NULL,
	[SystemLoadScoreSQL] [decimal](5, 2) NULL,
	[SystemLoadScore] [decimal](5, 2) NULL,
	[WeekUserExperience4SLRQScore] [decimal](5, 2) NULL,
	[WeekUserExperienceSLRQScore] [decimal](5, 2) NULL,
	[WeekSystemLoadScoreWeb] [decimal](5, 2) NULL,
	[WeekSystemLoadScoreSQL] [decimal](5, 2) NULL,
	[WeekSystemLoadScore] [decimal](5, 2) NULL,
	[SummaryDayHour] [datetime] NULL,
	[QoSHourID] [bigint] NULL,
	[RowHash] [binary](20) NULL,
	[IntegrityScore] [decimal](5, 2) NULL,
	[WeekIntegrityScore] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[QRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_RecoverabilityIntegritySummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_RecoverabilityIntegritySummary](
	[RISID] [int] IDENTITY(1,1) NOT NULL,
	[SummaryDayHour] [datetime] NULL,
	[RecoverabilityIntegrityScore] [decimal](5, 2) NULL,
	[BackupFrequencyScore] [decimal](5, 2) NULL,
	[BackupCoverageScore] [decimal](5, 2) NULL,
	[DbccFrequencyScore] [decimal](5, 2) NULL,
	[DbccCoverageScore] [decimal](5, 2) NULL,
	[RPOScore] [decimal](5, 2) NULL,
	[RTOScore] [decimal](5, 2) NULL,
	[WorstRPODatabase] [nvarchar](255) NULL,
	[WorstRTODatabase] [nvarchar](255) NULL,
	[PotentialDataLossMinutes] [int] NULL,
	[EstimatedTimeToRecoverHours] [decimal](6, 2) NULL,
	[RowHash] [binary](20) NULL,
 CONSTRAINT [PK_RecoverabilityIntegritySummary] PRIMARY KEY CLUSTERED 
(
	[RISID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_RecoveryObjectiveSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_RecoveryObjectiveSummary](
	[ROSID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](255) NULL,
	[DBName] [nvarchar](255) NULL,
	[PotentialDataLossMinutes] [int] NULL,
	[EstimatedTimeToRecoverHours] [decimal](6, 2) NULL,
	[RPOScore] [decimal](5, 2) NULL,
	[RTOScore] [decimal](5, 2) NULL,
 CONSTRAINT [PK_RecoveryObjectiveSummary] PRIMARY KEY CLUSTERED 
(
	[ROSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_SampleHistory]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_SampleHistory](
	[QSampleID] [int] IDENTITY(1,1) NOT NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[ServerArtifactID] [int] NULL,
	[ArrivalRate] [decimal](10, 3) NULL,
	[AVGConcurrency] [decimal](10, 3) NULL,
	[IsActiveWeeklySample] [bit] NULL,
	[IsActiveWeekly4Sample] [bit] NULL,
	[SampleDate] [datetime] NULL,
	[RowHash] [binary](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[QSampleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_SampleHistoryUX]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_SampleHistoryUX](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServerId] [int] NOT NULL,
	[HourId] [int] NOT NULL,
	[IsActiveArrivalRateSample] [bit] NOT NULL,
	[IsActiveConcurrencySample] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_SourceDatetime]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_SourceDatetime](
	[QSDID] [int] IDENTITY(1,1) NOT NULL,
	[quotidian] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_SystemLoadSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_SystemLoadSummary](
	[QSLSID] [int] IDENTITY(1,1) NOT NULL,
	[ServerArtifactID] [int] NULL,
	[ServerTypeID] [int] NULL,
	[AvgRAMPagesePerSec] [decimal](18, 0) NULL,
	[AvgCPUpct] [decimal](5, 2) NULL,
	[AvgRAMpct] [decimal](5, 2) NULL,
	[AvgRAMAvailKB] [decimal](10, 0) NULL,
	[RAMPagesScore] [decimal](5, 2) NULL,
	[RAMPctScore] [decimal](5, 2) NULL,
	[CPUScore] [decimal](5, 2) NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[RowHash] [binary](20) NULL,
	[MemorySignalStateRatio] [decimal](4, 3) NULL,
	[MemorySignalStateScore] [decimal](5, 2) NULL,
	[Pageouts] [int] NULL,
	[SignalWaitsRatio] [decimal](4, 3) NULL,
	[PoisonWaits] [int] NULL,
	[WaitsScore] [decimal](5, 2) NULL,
	[MaxVirtualLogFiles] [int] NULL,
	[VLFScore] [decimal](5, 2) NULL,
	[HighestLatencyDatabase] [nvarchar](255) NULL,
	[ReadLatencyMs] [int] NULL,
	[WriteLatencyMs] [int] NULL,
	[IOWaitsHigh] [bit] NULL,
	[IsDataFile] [bit] NULL,
	[LatencyScore] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[QSLSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_tempServers]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_tempServers](
	[ServerID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](255) NULL,
	[Failed] [bit] NULL,
	[Errors] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ServerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_UptimeRatings]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_UptimeRatings](
	[UpRatingID] [int] IDENTITY(1,1) NOT NULL,
	[HoursDown] [decimal](4, 3) NULL,
	[UptimeScore] [decimal](5, 2) NULL,
	[SummaryDayHour] [datetime] NULL,
	[RowHash] [binary](20) NULL,
	[WeekUptimeScore] [decimal](5, 2) NULL,
	[IsWebDowntime] [bit] NULL,
	[AffectedByMaintenanceWindow] [bit] NOT NULL DEFAULT (0)
PRIMARY KEY CLUSTERED 
(
	[UpRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [eddsdbo].[QoS_UserExperienceRatings]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_UserExperienceRatings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArrivalRateUXScore] [decimal](10, 3) NOT NULL,
	[ConcurrencyUXScore] [decimal](10, 3) NOT NULL,
	[ServerArtifactId] [int] NOT NULL,
	[HourId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_UserExperienceSearchSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_UserExperienceSearchSummary](
	[SearchSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[CaseArtifactID] [int] NULL,
	[SearchArtifactID] [int] NULL,
	[Search] [nvarchar](150) NULL,
	[LastAuditID] [bigint] NULL,
	[UserArtifactID] [int] NULL,
	[User] [nvarchar](150) NULL,
	[TotalRunTime] [bigint] NULL,
	[AverageRunTime] [int] NULL,
	[TotalRuns] [int] NULL,
	[PercentLongRunning] [int] NULL,
	[IsComplex] [bit] NULL,
	[SummaryDayHour] [datetime] NULL,
	[QoSHourID] [bigint] NULL,
	[ServerID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SearchSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_UserExperienceServerSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_UserExperienceServerSummary](
	[ServerSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[ServerArtifactID] [int] NULL,
	[Server] [nvarchar](150) NULL,
	[CaseArtifactID] [int] NULL,
	[Workspace] [nvarchar](150) NULL,
	[Score] [int] NULL,
	[TotalLongRunning] [int] NULL,
	[TotalUsers] [int] NULL,
	[TotalSearchAudits] [int] NULL,
	[TotalNonSearchAudits] [int] NULL,
	[TotalAudits] [int] NULL,
	[TotalExecutionTime] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[QoSHourID] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[ServerSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_UserXInstanceSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_UserXInstanceSummary](
	[UserXID] [int] IDENTITY(1,1) NOT NULL,
	[ServerArtifactID] [int] NULL,
	[QoSHourID] [bigint] NULL,
	[GlassRunID] [int] NULL,
	[SummaryDayHour] [datetime] NULL,
	[qtyActiveUsers] [int] NULL,
	[TotalExecutionTime] [int] NULL,
	[AVGConcurrency] [decimal](10, 3) NULL,
	[ArrivalRate] [decimal](10, 3) NULL,
	[TotalSimpleQuery] [int] NULL,
	[TotalSLRQ] [int] NULL,
	[AvgSQScorePerUser] [decimal](10, 3) NULL,
	[RowHash] [binary](20) NULL,
	[DocumentSearchScore] [decimal](10, 3) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserXID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_VarscatOutputCumulative]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_VarscatOutputCumulative](
	[kVOID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](150) NULL,
	[QoSHourID] [bigint] NULL,
	[AgentID] [int] NULL,
	[DatabaseName] [nvarchar](128) NULL,
	[SearchName] [nvarchar](max) NULL,
	[SearchArtifactID] [int] NULL,
	[isCLRQ] [bit] NULL,
	[totalSearchComplexityScore] [int] NULL,
	[LongestRunTime] [int] NULL,
	[ShortestRunTime] [int] NULL,
	[TotalLRQRunTime] [int] NULL,
	[QTYLikeOperators] [int] NULL,
	[QTYSubSearches] [int] NULL,
	[TotalQTYSubSearches] [int] NULL,
	[QTY Select folders] [int] NULL,
	[SearchTextLength] [int] NULL,
	[TotalRuns] [int] NULL,
	[ParsedSearchText] [nvarchar](max) NULL,
	[SearchType] [varchar](50) NULL,
	[total words all conditions] [int] NULL,
	[dtSearchTextLength] [int] NULL,
	[QTYOrderBy] [int] NULL,
	[SummaryDayHour] [datetime] NULL,
	[ServerID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[kVOID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_VarscatOutputDetailCumulative]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_VarscatOutputDetailCumulative](
	[VODCID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](150) NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[AgentID] [int] NULL,
	[CaseArtifactID] [int] NULL,
	[SearchArtifactID] [int] NULL,
	[SearchName] [nvarchar](max) NULL,
	[AuditID] [bigint] NULL,
	[UserID] [int] NULL,
	[ComplexityScore] [int] NULL,
	[ExecutionTime] [int] NULL,
	[Timestamp] [datetime] NULL,
	[QoSAction] [int] NULL,
	[IsLongRunning] [bit] NULL,
	[IsComplex] [bit] NULL,
	[QueryID] [uniqueidentifier] NULL,
	[ServerID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[VODCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_WaitDetail]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_WaitDetail](
	[WaitDetailID] [int] IDENTITY(1,1) NOT NULL,
	[WaitSummaryID] [int] NULL,
	[WaitTypeID] [int] NULL,
	[CumulativeWaitMs] [bigint] NULL,
	[CumulativeSignalWaitMs] [bigint] NULL,
	[DifferentialWaitMs] [bigint] NULL,
	[DifferentialSignalWaitMs] [bigint] NULL,
	[CumulativeWaitingTasksCount] [bigint] NULL,
	[DifferentialWaitingTasksCount] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[WaitDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_Waits]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_Waits](
	[WaitTypeID] [int] IDENTITY(1,1) NOT NULL,
	[WaitType] [nvarchar](60) NULL,
	[IsPoisonWait] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[WaitTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[QoS_WaitSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[QoS_WaitSummary](
	[WaitSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[ServerArtifactID] [int] NULL,
	[ServerName] [nvarchar](150) NULL,
	[LastSqlRestart] [datetime] NULL,
	[SignalWaitsRatio] [decimal](4, 3) NULL,
	[RunCondition] [int] NULL,
	[PercentOfCPUThreshold] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[WaitSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Reports_SearchAudits]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Reports_SearchAudits](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ServerId] [int] NOT NULL,
	[HourId] [int] NOT NULL,
	[SearchId] [int] NOT NULL,
	[MinAuditId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[WorkspaceId] [int] NOT NULL,
	[IsComplex] [bit] NOT NULL,
	[TotalSearchAudits] [bigint] NOT NULL,
	[PercentLongRunning] [decimal](9, 0) NOT NULL,
	[TotalExecutionTime] [bigint] NOT NULL,
 CONSTRAINT [PK_Reports_SearchAudits] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Reports_WorkspaceSearchAudits]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Reports_WorkspaceSearchAudits](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ServerId] [int] NOT NULL,
	[HourId] [int] NOT NULL,
	[SearchId] [int] NOT NULL,
	[SearchName] [varchar](75) NULL,
	[WorkspaceId] [int] NOT NULL,
	[TotalExecutionTime] [bigint] NOT NULL,
	[TotalSearchAudits] [bigint] NOT NULL,
	[IsComplex] [bit] NOT NULL,
 CONSTRAINT [PK_Reports_WorkspaceSearchAudits] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[RHScriptsRun]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[RHScriptsRun](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[version_id] [bigint] NULL,
	[script_name] [nvarchar](255) NULL,
	[text_of_script] [text] NULL,
	[text_hash] [nvarchar](512) NULL,
	[one_time_script] [bit] NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[RHScriptsRunErrors]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[RHScriptsRunErrors](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[repository_path] [nvarchar](255) NULL,
	[version] [nvarchar](50) NULL,
	[script_name] [nvarchar](255) NULL,
	[text_of_script] [ntext] NULL,
	[erroneous_part_of_script] [ntext] NULL,
	[error_message] [ntext] NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[RHVersion]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[RHVersion](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[repository_path] [nvarchar](255) NULL,
	[version] [nvarchar](50) NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[SearchAuditBatch]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[SearchAuditBatch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkspaceId] [int] NOT NULL,
	[BatchStart] [bigint] NOT NULL,
	[BatchSize] [int] NOT NULL,
	[Completed] [bit] NOT NULL,
	[HourSearchAuditBatchId] [int] NULL,
 CONSTRAINT [PK_SearchAuditBatch] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[SearchAuditBatchResult]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[SearchAuditBatchResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[TotalComplexQueries] [bigint] NOT NULL,
	[TotalLongRunningQueries] [bigint] NOT NULL,
	[TotalSimpleLongRunningQueries] [bigint] NOT NULL,
	[TotalQueries] [bigint] NOT NULL,
	[TotalExecutionTime] [bigint] NULL,
 CONSTRAINT [PK_SearchAuditBatchResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Server]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Server](
	[ServerID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [nvarchar](150) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[DeletedOn] [datetime] NULL,
	[ServerTypeID] [int] NOT NULL,
	[ServerIPAddress] [varchar](100) NULL,
	[IgnoreServer] [bit] NULL,
	[ResponsibleAgent] [nvarchar](max) NULL,
	[ArtifactID] [int] NULL,
	[LastChecked] [datetime] NULL,
	[UptimeMonitoringResourceUseHttps] [bit] NULL,
	[UptimeMonitoringResourceHost] [varchar](max) NULL,
	[LastServerBackup] [datetime] NULL,
	[AdminScriptsVersion] [varchar](20) NULL,
	[IsQoSDeployed] [bit] NOT NULL,
 CONSTRAINT [PK_Server] PRIMARY KEY CLUSTERED 
(
	[ServerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerCleanups]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerCleanups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HourId] [int] NOT NULL,
	[ServerId] [int] NOT NULL,
	[Success] [bit] NOT NULL,
 CONSTRAINT [PK_ServerCleanups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerDiskDW]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerDiskDW](
	[ServerDiskDWID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[DiskNumber] [int] NOT NULL,
	[DiskAvgReadsPerSec] [decimal](10, 2) NULL,
	[DiskAvgWritesPerSec] [decimal](10, 2) NULL,
	[DriveLetter] [nvarchar](300) NULL,
	[DiskFreeMegabytes] [int] NULL,
	[DiskSecPerRead] [bigint] NULL,
	[DiskSecPerWrite] [bigint] NULL,
	[FrequencyPerfTime] [bigint] NULL,
	[DiskSecPerReadBase] [bigint] NULL,
	[DiskSecPerWriteBase] [bigint] NULL,
 CONSTRAINT [PK_ServerDiskDW] PRIMARY KEY CLUSTERED 
(
	[ServerDiskDWID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerDiskSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerDiskSummary](
	[ServerDiskSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[MeasureDate] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[DiskNumber] [int] NOT NULL,
	[DiskAvgReadsPerSec] [decimal](10, 2) NULL,
	[DiskAvgWritesPerSec] [decimal](10, 2) NULL,
	[DriveLetter] [nvarchar](300) NULL,
	[DiskFreeMegabytes] [int] NULL,
	[DiskSecPerRead] [decimal](10, 3) NULL,
	[DiskSecPerWrite] [decimal](10, 3) NULL,
 CONSTRAINT [PK_ServerDiskSummary] PRIMARY KEY CLUSTERED 
(
	[ServerDiskSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerDW]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerDW](
	[ServerDWID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[RAMPagesPerSec] [decimal](10, 2) NULL,
	[RAMPageFaultsPerSec] [decimal](10, 2) NULL,
	[TotalPhysicalMemory] [decimal](10, 0) NULL,
	[AvailableMemory] [decimal](10, 0) NULL,
	[RAMPct] [decimal](10, 2) NULL,
 CONSTRAINT [PK_ServerDW] PRIMARY KEY CLUSTERED 
(
	[ServerDWID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerProcessorDW]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerProcessorDW](
	[ServerProcessorDWID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[CoreNumber] [int] NOT NULL,
	[CPUProcessorTimePct] [decimal](10, 0) NULL,
	[CPUName] [nvarchar](200) NULL,
 CONSTRAINT [PK_ServerProcessorDW] PRIMARY KEY CLUSTERED 
(
	[ServerProcessorDWID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerProcessorSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerProcessorSummary](
	[ServerProcessorSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[MeasureDate] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[CoreNumber] [int] NOT NULL,
	[CPUProcessorTimePct] [decimal](10, 0) NULL,
	[CPUName] [nvarchar](200) NULL,
 CONSTRAINT [PK_ServerProcessorSummary] PRIMARY KEY CLUSTERED 
(
	[ServerProcessorSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerSummary](
	[ServerSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[MeasureDate] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[RAMPagesPerSec] [decimal](10, 2) NULL,
	[RAMPageFaultsPerSec] [decimal](10, 2) NULL,
	[TotalPhysicalMemory] [decimal](10, 0) NULL,
	[AvailableMemory] [decimal](10, 0) NULL,
	[RAMPct] [decimal](10, 2) NULL,
 CONSTRAINT [PK_ServerSummary] PRIMARY KEY CLUSTERED 
(
	[ServerSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[ServerType]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[ServerType](
	[ServerTypeID] [int] IDENTITY(1,1) NOT NULL,
	[ServerTypeName] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ArtifactID] [int] NULL,
 CONSTRAINT [PK_ServerType] PRIMARY KEY CLUSTERED 
(
	[ServerTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[SQLServerDW]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[SQLServerDW](
	[SQLServerDWID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[SQLPageLifeExpectancy] [decimal](10, 2) NULL,
	[LowMemorySignalState] [bit] NULL,
 CONSTRAINT [PK_SQLServerDW] PRIMARY KEY CLUSTERED 
(
	[SQLServerDWID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[SQLServerPageouts]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[SQLServerPageouts](
	[SSPID] [int] IDENTITY(1,1) NOT NULL,
	[ServerID] [int] NULL,
	[SummaryDayHour] [datetime] NULL,
	[QoSHourID] [bigint] NULL,
	[Pageouts] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SSPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[SQLServerSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[SQLServerSummary](
	[SQLServerSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[MeasureDate] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[SQLPageLifeExpectancy] [decimal](10, 2) NULL,
	[LowMemorySignalStateRatio] [decimal](4, 3) NULL,
 CONSTRAINT [PK_SQLServerSummary] PRIMARY KEY CLUSTERED 
(
	[SQLServerSummaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[SystemVersionHistory]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[SystemVersionHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SummaryDayHour] [datetime] NOT NULL,
	[RowHash] [binary](20) NOT NULL,
	[RelativityVersion] [varchar](200) NOT NULL,
	[OSVersion] [nvarchar](256) NOT NULL,
	[OSServicePack] [nvarchar](256) NOT NULL,
	[SqlServerVersion] [nvarchar](128) NOT NULL,
	[SqlServerLevel] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_SystemVersionHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[UserCountDW]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[UserCountDW](
	[UserCountDWID] [int] IDENTITY(1,1) NOT NULL,
	[MeasureDate] [datetime] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[UserCount] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_UserCountDW] PRIMARY KEY CLUSTERED 
(
	[UserCountDWID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[UserExperience]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[UserExperience](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HourId] [int] NOT NULL,
	[ServerId] [int] NOT NULL,
	[ActiveUsers] [int] NOT NULL,
	[HasPoisonWaits] [bit] NOT NULL,
	[ArrivalRate] [decimal](12, 5) NOT NULL,
	[Concurrency] [decimal](12, 5) NOT NULL,
 CONSTRAINT [PK_UserExperience] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[Version]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[Version](
	[ApplicationVersion] [nchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [eddsdbo].[VirtualLogFileSummary]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [eddsdbo].[VirtualLogFileSummary](
	[VLFSID] [int] IDENTITY(1,1) NOT NULL,
	[QoSHourID] [bigint] NULL,
	[SummaryDayHour] [datetime] NULL,
	[ServerArtifactID] [int] NULL,
	[DatabaseName] [nvarchar](150) NULL,
	[VirtualLogFiles] [int] NULL,
 CONSTRAINT [PK_VirtualLogFileSummary] PRIMARY KEY CLUSTERED 
(
	[VLFSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[AggregatedCounter]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[AggregatedCounter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [bigint] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_CounterAggregated] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Counter]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Counter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [smallint] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Counter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Hash]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Hash](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Field] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime2](7) NULL,
 CONSTRAINT [PK_HangFire_Hash] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Job]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Job](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateId] [int] NULL,
	[StateName] [nvarchar](20) NULL,
	[InvocationData] [nvarchar](max) NOT NULL,
	[Arguments] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Job] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[JobParameter]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[JobParameter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Name] [nvarchar](40) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_JobParameter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[JobQueue]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[JobQueue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Queue] [nvarchar](50) NOT NULL,
	[FetchedAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_JobQueue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[List]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[List](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_List] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Schema]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Schema](
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_HangFire_Schema] PRIMARY KEY CLUSTERED 
(
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Server]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Server](
	[Id] [nvarchar](100) NOT NULL,
	[Data] [nvarchar](max) NULL,
	[LastHeartbeat] [datetime] NOT NULL,
 CONSTRAINT [PK_HangFire_Server] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Set]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Set](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Score] [float] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Set] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[State]    Script Date: 8/7/2018 8:34:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[State](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Reason] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_State] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [eddsdbo].[DataGridCache](
	[WorkspaceId] [int] NOT NULL,
	[StartHourToReadAuditsFromDataGrid] [int] NULL,
 CONSTRAINT [PK_DataGridCache] PRIMARY KEY CLUSTERED 
(
	[WorkspaceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [eddsdbo].[Databases](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[ServerId] [int] NOT NULL,
	[WorkspaceId] [int] NULL,
	[Type] [int] NOT NULL,
	[DeletedOn] [datetime] NULL,
	[Ignore] [bit] NOT NULL,
	[LastDbccDate] [datetime] NULL,
	[LastBackupLogDate] [datetime] NULL,
	[LastBackupDiffDate] [datetime] NULL,
	[LastBackupFullDate] [datetime] NULL,
	[LastBackupFullDuration] [int] NULL,
	[LastBackupDiffDuration] [int] NULL,
	[LogBackupsDuration] [int] NULL,
 CONSTRAINT [PK_Databases] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [eddsdbo].[Databases] ADD  CONSTRAINT [DF_Databases_Ignore]  DEFAULT ((0)) FOR [Ignore]

ALTER TABLE [eddsdbo].[Databases]  WITH CHECK ADD  CONSTRAINT [FK_Databases_Server] FOREIGN KEY([ServerId])
REFERENCES [eddsdbo].[Server] ([ServerID])

ALTER TABLE [eddsdbo].[Databases] CHECK CONSTRAINT [FK_Databases_Server]

ALTER TABLE [eddsdbo].[DatabaseGaps]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseGaps_Databases] FOREIGN KEY([DatabaseId])
REFERENCES [eddsdbo].[Databases] ([ID])
GO

ALTER TABLE [eddsdbo].[DatabaseGaps] CHECK CONSTRAINT [FK_DatabaseGaps_Databases]
GO

CREATE TABLE eddsdbo.Reports_RecoverabilityIntegritySummary (
	Id INT IDENTITY(1,1),
	CONSTRAINT PK_Reports_RecoverabilityIntegritySummary PRIMARY KEY (Id),
	HourId INT,
	OverallScore DECIMAL (5,2),
	RpoScore DECIMAL (5,2),
	RtoScore DECIMAL (5,2),
	BackupFrequencyScore DECIMAL (5,2),
	BackupCoverageScore DECIMAL (5,2),
	DbccFrequencyScore DECIMAL (5,2),
	DbccCoverageScore DECIMAL (5,2)
)
GO

CREATE TABLE eddsdbo.Reports_RecoverabilityIntegrityRpoSummary (
	Id INT IDENTITY(1,1),
	CONSTRAINT PK_Reports_RecoverabilityIntegrityRpoSummary PRIMARY KEY (Id),
	HourId INT,
	WorstRpoDatabase NVARCHAR(255),
	RpoMaxDataLoss INT
)

CREATE TABLE eddsdbo.Reports_RecoverabilityIntegrityRtoSummary (
	Id INT IDENTITY(1,1),
	CONSTRAINT PK_Reports_RecoverabilityIntegrityRtoSummary PRIMARY KEY (Id),
	HourId INT,
	WorstRtoDatabase NVARCHAR(255),
	RtoTimeToRecover DECIMAL (6,2)
)

CREATE TABLE eddsdbo.Reports_RecoveryObjectives (
	Id INT IDENTITY (1,1),
	CONSTRAINT PK_Reports_RecoveryObjectives PRIMARY KEY (Id),
	DatabaseId INT,
	RpoScore DECIMAL (5, 2),
	RpoMaxDataLoss INT,
	RtoScore DECIMAL (5, 2),		
	RtoTimeToRecover DECIMAL (6,2)
)
GO

CREATE TABLE eddsdbo.Reports_RecoveryGaps
(
	Id INT IDENTITY (1,1),
	CONSTRAINT PK_Reports_RecoveryGaps PRIMARY KEY (Id),
	DatabaseId INT,
	ActivityType INT,
	LastActivity DATETIME,
	GapResolutionDate DATETIME,
	GapSize INT
)
GO

ALTER TABLE [eddsdbo].[DataGridCache]  WITH CHECK ADD  CONSTRAINT [FK_DataGridCache_Hours] FOREIGN KEY([StartHourToReadAuditsFromDataGrid])
REFERENCES [eddsdbo].[Hours] ([ID])
GO
ALTER TABLE [eddsdbo].[DataGridCache] CHECK CONSTRAINT [FK_DataGridCache_Hours]
GO
ALTER TABLE [eddsdbo].[Events] ADD  DEFAULT (getutcdate()) FOR [LastUpdated]
GO
ALTER TABLE [eddsdbo].[Hours] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_Frequency]  DEFAULT ((60)) FOR [Frequency]
GO
ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[PerformanceSummary] ADD  CONSTRAINT [DF_PerformanceSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_UserExperience4SLRQScore]  DEFAULT ((100)) FOR [UserExperience4SLRQScore]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_UserExperienceSLRQScore]  DEFAULT ((100)) FOR [UserExperienceSLRQScore]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_SystemLoadScoreWeb]  DEFAULT ((100)) FOR [SystemLoadScoreWeb]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_SystemLoadScoreSQL]  DEFAULT ((100)) FOR [SystemLoadScoreSQL]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_SystemLoadScore]  DEFAULT ((100)) FOR [SystemLoadScore]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_WeekUserExperience4SLRQScore]  DEFAULT ((100)) FOR [WeekUserExperience4SLRQScore]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_WeekUserExperienceSLRQScore]  DEFAULT ((100)) FOR [WeekUserExperienceSLRQScore]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_WeekSystemLoadScoreWeb]  DEFAULT ((100)) FOR [WeekSystemLoadScoreWeb]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_WeekSystemLoadScoreSQL]  DEFAULT ((100)) FOR [WeekSystemLoadScoreSQL]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_WeekSystemLoadScore]  DEFAULT ((100)) FOR [WeekSystemLoadScore]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_IntegrityScore]  DEFAULT ((100)) FOR [IntegrityScore]
GO
ALTER TABLE [eddsdbo].[QoS_Ratings] ADD  CONSTRAINT [DF_Ratings_WeekIntegrityScore]  DEFAULT ((100)) FOR [WeekIntegrityScore]
GO
ALTER TABLE [eddsdbo].[QoS_UptimeRatings] ADD  CONSTRAINT [DF_UptimeRatings_UptimeScore]  DEFAULT ((100)) FOR [UptimeScore]
GO
ALTER TABLE [eddsdbo].[QoS_UptimeRatings] ADD  CONSTRAINT [DF_UptimeRatings_WeekUptimeScore]  DEFAULT ((100)) FOR [WeekUptimeScore]
GO
ALTER TABLE [eddsdbo].[QoS_UserExperienceRatings] ADD  DEFAULT ((0)) FOR [HourId]
GO
ALTER TABLE [eddsdbo].[SearchAuditBatch] ADD  DEFAULT ((0)) FOR [Completed]
GO
ALTER TABLE [eddsdbo].[Server] ADD  CONSTRAINT [DF_Server_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[Server] ADD  DEFAULT ((0)) FOR [IsQoSDeployed]
GO
ALTER TABLE [eddsdbo].[ServerDiskDW] ADD  CONSTRAINT [DF_ServerDiskDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[ServerDiskSummary] ADD  CONSTRAINT [DF_ServerDiskSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[ServerDW] ADD  CONSTRAINT [DF_ServerDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[ServerProcessorDW] ADD  CONSTRAINT [DF_ServerProcessorDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[ServerProcessorSummary] ADD  CONSTRAINT [DF_ServerProcessorSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[ServerSummary] ADD  CONSTRAINT [DF_ServerSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[ServerType] ADD  CONSTRAINT [DF_ServerType_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[SQLServerDW] ADD  CONSTRAINT [DF_SQLServerDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[SQLServerSummary] ADD  CONSTRAINT [DF_SQLServerSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [eddsdbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_CategoryTypes] FOREIGN KEY([CategoryTypeID])
REFERENCES [eddsdbo].[CategoryTypes] ([ID])
GO
ALTER TABLE [eddsdbo].[Categories] CHECK CONSTRAINT [FK_Categories_CategoryTypes]
GO
ALTER TABLE [eddsdbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Hours] FOREIGN KEY([HourID])
REFERENCES [eddsdbo].[Hours] ([ID])
GO
ALTER TABLE [eddsdbo].[Categories] CHECK CONSTRAINT [FK_Categories_Hours]
GO
ALTER TABLE [eddsdbo].[CategoryScores]  WITH CHECK ADD  CONSTRAINT [FK_CategoryScores_Categories] FOREIGN KEY([CategoryID])
REFERENCES [eddsdbo].[Categories] ([ID])
GO
ALTER TABLE [eddsdbo].[CategoryScores] CHECK CONSTRAINT [FK_CategoryScores_Categories]
GO
ALTER TABLE [eddsdbo].[CategoryScores]  WITH CHECK ADD  CONSTRAINT [FK_CategoryScores_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[CategoryScores] CHECK CONSTRAINT [FK_CategoryScores_Server]
GO
ALTER TABLE [eddsdbo].[EventLocks]  WITH CHECK ADD  CONSTRAINT [FK_EventLocks_EventWorkers] FOREIGN KEY([WorkerId])
REFERENCES [eddsdbo].[EventWorkers] ([Id])
GO
ALTER TABLE [eddsdbo].[EventLocks] CHECK CONSTRAINT [FK_EventLocks_EventWorkers]
GO
ALTER TABLE [eddsdbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_GlassRunLogs] FOREIGN KEY([LogId])
REFERENCES [eddsdbo].[QoS_GlassRunLog] ([GRLogID])
GO
ALTER TABLE [eddsdbo].[EventLogs] CHECK CONSTRAINT [FK_EventLogs_GlassRunLogs]
GO
ALTER TABLE [eddsdbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Hours] FOREIGN KEY([HourId])
REFERENCES [eddsdbo].[Hours] ([ID])
GO
ALTER TABLE [eddsdbo].[Events] CHECK CONSTRAINT [FK_Events_Hours]
GO
ALTER TABLE [eddsdbo].[HourSearchAuditBatches]  WITH CHECK ADD  CONSTRAINT [FK_HourSearchAuditBatches_Hours] FOREIGN KEY([HourId])
REFERENCES [eddsdbo].[Hours] ([ID])
GO
ALTER TABLE [eddsdbo].[HourSearchAuditBatches] CHECK CONSTRAINT [FK_HourSearchAuditBatches_Hours]
GO
ALTER TABLE [eddsdbo].[HourSearchAuditBatches]  WITH CHECK ADD  CONSTRAINT [FK_HourSearchAuditBatches_Server] FOREIGN KEY([ServerId])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[HourSearchAuditBatches] CHECK CONSTRAINT [FK_HourSearchAuditBatches_Server]
GO
ALTER TABLE [eddsdbo].[Measure]  WITH CHECK ADD  CONSTRAINT [FK_Measure_MeasureType] FOREIGN KEY([MeasureTypeId])
REFERENCES [eddsdbo].[MeasureType] ([MeasureTypeId])
GO
ALTER TABLE [eddsdbo].[Measure] CHECK CONSTRAINT [FK_Measure_MeasureType]
GO
ALTER TABLE [eddsdbo].[MetricData]  WITH CHECK ADD  CONSTRAINT [FK_MetricData_Metrics] FOREIGN KEY([MetricID])
REFERENCES [eddsdbo].[Metrics] ([ID])
GO
ALTER TABLE [eddsdbo].[MetricData] CHECK CONSTRAINT [FK_MetricData_Metrics]
GO
ALTER TABLE [eddsdbo].[MetricData]  WITH CHECK ADD  CONSTRAINT [FK_MetricData_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[MetricData] CHECK CONSTRAINT [FK_MetricData_Server]
GO
ALTER TABLE [eddsdbo].[MetricData_AuditAnalysis]  WITH CHECK ADD  CONSTRAINT [FK_MetricData_AuditAnalysis_MetricData] FOREIGN KEY([MetricDataId])
REFERENCES [eddsdbo].[MetricData] ([ID])
GO
ALTER TABLE [eddsdbo].[MetricData_AuditAnalysis] CHECK CONSTRAINT [FK_MetricData_AuditAnalysis_MetricData]
GO
ALTER TABLE [eddsdbo].[Metrics]  WITH CHECK ADD  CONSTRAINT [FK_Metrics_Hours] FOREIGN KEY([HourID])
REFERENCES [eddsdbo].[Hours] ([ID])
GO
ALTER TABLE [eddsdbo].[Metrics] CHECK CONSTRAINT [FK_Metrics_Hours]
GO
ALTER TABLE [eddsdbo].[Metrics]  WITH CHECK ADD  CONSTRAINT [FK_Metrics_MetricTypes] FOREIGN KEY([MetricTypeID])
REFERENCES [eddsdbo].[MetricTypes] ([ID])
GO
ALTER TABLE [eddsdbo].[Metrics] CHECK CONSTRAINT [FK_Metrics_MetricTypes]
GO
ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes]  WITH CHECK ADD  CONSTRAINT [FK_MetricTypesToCategoryTypes_CategoryTypes] FOREIGN KEY([CategoryTypeID])
REFERENCES [eddsdbo].[CategoryTypes] ([ID])
GO
ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes] CHECK CONSTRAINT [FK_MetricTypesToCategoryTypes_CategoryTypes]
GO
ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes]  WITH CHECK ADD  CONSTRAINT [FK_MetricTypesToCategoryTypes_MetricTypes] FOREIGN KEY([MetricTypeID])
REFERENCES [eddsdbo].[MetricTypes] ([ID])
GO
ALTER TABLE [eddsdbo].[MetricTypesToCategoryTypes] CHECK CONSTRAINT [FK_MetricTypesToCategoryTypes_MetricTypes]
GO
ALTER TABLE [eddsdbo].[SearchAuditBatchResult]  WITH CHECK ADD  CONSTRAINT [FK_SearchAuditBatchResult_SearchAuditBatch] FOREIGN KEY([BatchId])
REFERENCES [eddsdbo].[SearchAuditBatch] ([Id])
GO
ALTER TABLE [eddsdbo].[SearchAuditBatchResult] CHECK CONSTRAINT [FK_SearchAuditBatchResult_SearchAuditBatch]
GO
ALTER TABLE [eddsdbo].[ServerDiskDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerDiskDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerDiskDW] CHECK CONSTRAINT [FK_ServerDiskDW_Server]
GO
ALTER TABLE [eddsdbo].[ServerDiskSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerDiskSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerDiskSummary] CHECK CONSTRAINT [FK_ServerDiskSummary_Server]
GO
ALTER TABLE [eddsdbo].[ServerDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerDW] CHECK CONSTRAINT [FK_ServerDW_Server]
GO
ALTER TABLE [eddsdbo].[ServerProcessorDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerProcessorDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerProcessorDW] CHECK CONSTRAINT [FK_ServerProcessorDW_Server]
GO
ALTER TABLE [eddsdbo].[ServerProcessorSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerProcessorSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerProcessorSummary] CHECK CONSTRAINT [FK_ServerProcessorSummary_Server]
GO
ALTER TABLE [eddsdbo].[ServerSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerSummary] CHECK CONSTRAINT [FK_ServerSummary_Server]
GO
ALTER TABLE [eddsdbo].[SQLServerDW]  WITH CHECK ADD  CONSTRAINT [FK_SQLServerDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[SQLServerDW] CHECK CONSTRAINT [FK_SQLServerDW_Server]
GO
ALTER TABLE [eddsdbo].[SQLServerSummary]  WITH CHECK ADD  CONSTRAINT [FK_SQLServerSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[SQLServerSummary] CHECK CONSTRAINT [FK_SQLServerSummary_Server]
GO
ALTER TABLE [HangFire].[JobParameter]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_JobParameter_Job] FOREIGN KEY([JobId])
REFERENCES [HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [HangFire].[JobParameter] CHECK CONSTRAINT [FK_HangFire_JobParameter_Job]
GO
ALTER TABLE [HangFire].[State]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_State_Job] FOREIGN KEY([JobId])
REFERENCES [HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [HangFire].[State] CHECK CONSTRAINT [FK_HangFire_State_Job]
GO
