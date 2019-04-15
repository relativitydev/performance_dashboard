USE [EDDSPerformance]

/****** Object:  Table [eddsdbo].[RHVersion]    Script Date: 03/25/2014 15:31:03 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET IDENTITY_INSERT [eddsdbo].[RHVersion] ON
INSERT [eddsdbo].[RHVersion] ([id], [repository_path], [version], [entry_date], [modified_date], [entered_by]) VALUES (1, N'kCura Corporation - Performance Dashboard v1', N'0.8.6.0', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
SET IDENTITY_INSERT [eddsdbo].[RHVersion] OFF
/****** Object:  Table [eddsdbo].[RHScriptsRunErrors]    Script Date: 03/25/2014 15:31:03 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
/****** Object:  Table [eddsdbo].[RHScriptsRun]    Script Date: 03/25/2014 15:31:03 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET IDENTITY_INSERT [eddsdbo].[RHScriptsRun] ON
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (1, 1, N'0000001_eddsdbo.Table.BISSummary.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[BISSummary]    Script Date: 03/14/2014 10:47:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[BISSummary](
	[BISSummaryID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[MeasureDate] [date] NOT NULL,
	[TQCount] [int] NULL,
	[TotalNRQCount] [int] NULL,
	[NRLRQCount] [int] NULL,
	[StatusDay] [int] NULL,
	[StatusPercentageNRLRQDay] [int] NULL,
	[DocumentCount] [bigint] NULL,
 CONSTRAINT [PK_BISSummary] PRIMARY KEY CLUSTERED 
(
	[BISSummaryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


', N'NS55ieG0k1I+AvXYpjxu4Q==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (2, 1, N'0000002_eddsdbo.Table.Configuration.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Configuration]    Script Date: 03/14/2014 10:48:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


', N'IJ09LOwJ54lwgHjEvjSf3A==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (3, 1, N'0000003_eddsdbo.Table.ErrorCountDW.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ErrorCountDW]    Script Date: 03/14/2014 10:49:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[ErrorCountDW](
	[ErrorCountDWID] [int] IDENTITY(1,1) NOT NULL,
	[MeasureDate] [date] NOT NULL,
	[MeasureHour] [int] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[ErrorCount] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_ErrorCountDW] PRIMARY KEY CLUSTERED 
(
	[ErrorCountDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



', N'5hCx8FT+/nfUIfVQLSTPvQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (4, 1, N'0000004_eddsdbo.Table.LatencyDW.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[LatencyDW]    Script Date: 03/14/2014 10:49:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[LatencyDW](
	[LatencyDWID] [int] IDENTITY(1,1) NOT NULL,
	[MeasureDate] [date] NOT NULL,
	[MeasureHour] [int] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[AverageLatency] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_LatencyDW] PRIMARY KEY CLUSTERED 
(
	[LatencyDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


', N'glVlogpDy5kb450/JFawjQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (5, 1, N'0000005_eddsdbo.Table.LRQCountDW.sql', N'USE [EDDSPerformance]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[LRQCountDW](
	[LRQCountDWID] [int] IDENTITY(1,1) NOT NULL,
	[MeasureDate] [date] NOT NULL,
	[MeasureHour] [int] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[LRQCount] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
	[NRLRQCount] [int] NULL,
	[TotalQCount] [int] NULL,
	[TotalNRQCount] [int] NULL,
	[totalQtime] [int] NULL,
 CONSTRAINT [PK_LRQCountDW] PRIMARY KEY CLUSTERED 
(
	[LRQCountDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
', N'wAjff2FsNrq13hkXP4njvA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (6, 1, N'0000006_eddsdbo.Table.Measure.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Measure]    Script Date: 03/14/2014 10:50:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


', N'FHhau/OerQZox995/u7Xkw==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (7, 1, N'0000007_eddsdbo.Table.MeasureType.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[MeasureType]    Script Date: 03/14/2014 10:51:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

', N'NiVnKWkbFwkMo14yFlA+MQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (8, 1, N'0000008_eddsdbo.Table.PerformanceSummary.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[PerformanceSummary]    Script Date: 03/14/2014 10:51:36 ******/
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
	[AverageLatency] [int] NULL,
	[NRLRQCount] [int] NULL,
	[TotalQCount] [int] NULL,
	[TotalNRQCount] [int] NULL,
 CONSTRAINT [PK_PerformanceSummary] PRIMARY KEY CLUSTERED 
(
	[PerformanceSummaryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



', N'bn6BdIQ2AXun4+zcdi+yEQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (9, 1, N'0000009_eddsdbo.Table.ProcessControl.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ProcessControl]    Script Date: 03/14/2014 10:51:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[ProcessControl](
	[ProcessControlID] [int] NOT NULL,
	[ProcessTypeDesc] [nvarchar](200) NOT NULL,
	[LastProcessExecDateTime] [datetime] NOT NULL,
	[Frequency] [int] NULL,
 CONSTRAINT [PK_ProcessControl] PRIMARY KEY CLUSTERED 
(
	[ProcessControlID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


', N'FD5uBIEWQYuiuyeaoQMVdw==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (10, 1, N'0000010_eddsdbo.Table.Server.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Server]    Script Date: 03/14/2014 10:52:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [eddsdbo].[Server](
	[ServerID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [varchar](100) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[DeletedOn] [datetime] NULL,
	[ServerTypeID] [int] NOT NULL,
	[ServerIPAddress] [varchar](100) NULL,
	[IgnoreServer] [bit] NULL,
	[ResponsibleAgent] [nvarchar](max) NULL,
 CONSTRAINT [PK_Server] PRIMARY KEY CLUSTERED 
(
	[ServerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


', N'ffa7V4DGka0o28HepxGv3Q==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (11, 1, N'0000011_eddsdbo.Table.ServerDiskDW.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerDiskDW]    Script Date: 03/14/2014 10:53:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[ServerDiskDW](
	[ServerDiskDWID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[DiskNumber] [int] NOT NULL,
	[DiskAvgSecPerRead] [decimal](10, 2) NULL,
	[DiskAvgSecPerWrite] [decimal](10, 2) NULL,
	[DriveLetter] [nvarchar](300) NULL,
 CONSTRAINT [PK_ServerDiskDW] PRIMARY KEY CLUSTERED 
(
	[ServerDiskDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
', N'blUGE/dc0jQh27e9yS2Zaw==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (12, 1, N'0000012_eddsdbo.Table.ServerDiskSummary.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerDiskSummary]    Script Date: 03/14/2014 10:53:28 ******/
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
	[DiskAvgSecPerRead] [decimal](10, 2) NULL,
	[DiskAvgSecPerWrite] [decimal](10, 2) NULL,
	[DriveLetter] [nvarchar](300) NULL,
 CONSTRAINT [PK_ServerDiskSummary] PRIMARY KEY CLUSTERED 
(
	[ServerDiskSummaryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


', N'4ckj6IRYuMu49+iT1U0MVw==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (13, 1, N'0000013_eddsdbo.Table.ServerDW.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerDW]    Script Date: 03/14/2014 10:54:11 ******/
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
 CONSTRAINT [PK_ServerDW] PRIMARY KEY CLUSTERED 
(
	[ServerDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
', N'sbF0d8S628jVFb9n47eEeQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (14, 1, N'0000014_eddsdbo.Table.ServerProceessorDW.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerProcessorDW]    Script Date: 03/14/2014 11:04:50 ******/
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
 CONSTRAINT [PK_ServerProcessorDW] PRIMARY KEY CLUSTERED 
(
	[ServerProcessorDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

', N'GBAUkp/fea21NoHNW20XoQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (15, 1, N'0000015_eddsdbo.Table.ServerProcessorSummary.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerProcessorSummary]    Script Date: 03/14/2014 11:05:38 ******/
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
 CONSTRAINT [PK_ServerProcessorSummary] PRIMARY KEY CLUSTERED 
(
	[ServerProcessorSummaryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
', N'NMU28NzLxiZPiyW46V+EyQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (16, 1, N'0000016_eddsdbo.Table.ServerSummary.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerSummary]    Script Date: 03/14/2014 11:06:12 ******/
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
 CONSTRAINT [PK_ServerSummary] PRIMARY KEY CLUSTERED 
(
	[ServerSummaryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
', N'L+1mMzY3MB8tg6OMtDlmCA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (17, 1, N'0000017_eddsdbo.Table.ServerType.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ServerType]    Script Date: 03/14/2014 11:06:26 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

', N'APMquhyGKMrBmpk4PydTKg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (18, 1, N'0000018_eddsdbo.Table.SQLServerDW.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[SQLServerDW]    Script Date: 03/14/2014 11:06:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[SQLServerDW](
	[SQLServerDWID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ServerID] [int] NOT NULL,
	[SQLPageLifeExpectancy] [decimal](10, 2) NULL,
 CONSTRAINT [PK_SQLServerDW] PRIMARY KEY CLUSTERED 
(
	[SQLServerDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
', N'7U+mbWaJkD40cx8kDv4ZtA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (19, 1, N'0000019_eddsdbo.Table.SQLServerSummary.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[SQLServerSummary]    Script Date: 03/14/2014 11:07:16 ******/
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
 CONSTRAINT [PK_SQLServerSummary] PRIMARY KEY CLUSTERED 
(
	[SQLServerSummaryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
', N'yCpYiHCDi5uxXxyyDKoNJQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (20, 1, N'0000020_eddsdbo.Table.UserCountDW.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[UserCountDW]    Script Date: 03/14/2014 11:07:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[UserCountDW](
	[UserCountDWID] [int] IDENTITY(1,1) NOT NULL,
	[MeasureDate] [date] NOT NULL,
	[MeasureHour] [int] NOT NULL,
	[CaseArtifactID] [int] NOT NULL,
	[UserCount] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_UserCountDW] PRIMARY KEY CLUSTERED 
(
	[UserCountDWID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

', N'sIrPzVlN9SJcw0jnIimCZg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (21, 1, N'0000021_eddsdbo.Table.Version.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Version]    Script Date: 03/14/2014 11:08:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[Version](
	[ApplicationVersion] [nchar](10) NULL
) ON [PRIMARY]

GO


', N'E3HfOIbreIOhkHpW8XFVyQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (22, 1, N'0000022_eddsdbo.Table.JustinTable.sql', N'USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Version]    Script Date: 03/14/2014 11:08:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [eddsdbo].[JustinTable](
	[ApplicationVersion] [nchar](10) NULL
) ON [PRIMARY]

GO


', N'DuL9LoLcqYZ3GTwHvuWM9A==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (23, 1, N'0000022_eddsdbo.KEYS.CONSTRAINTS.sql', N'USE EDDSPerformance;
GO


/****** Object:  Default [DF_Server_CreatedOn]    Script Date: 10/11/2011 13:32:09 ******/
ALTER TABLE [eddsdbo].[Server] ADD  CONSTRAINT [DF_Server_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_LRQCountDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[LRQCountDW] ADD  CONSTRAINT [DF_LRQCountDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_MeasureType_IsActive]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_MeasureType_IsDeleted]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
/****** Object:  Default [DF_MeasureType_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[MeasureType] ADD  CONSTRAINT [DF_MeasureType_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_LatencyDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[LatencyDW] ADD  CONSTRAINT [DF_LatencyDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ErrorCountDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[ErrorCountDW] ADD  CONSTRAINT [DF_ErrorCountDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ServerType_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[ServerType] ADD  CONSTRAINT [DF_ServerType_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_PerformanceSummary_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[PerformanceSummary] ADD  CONSTRAINT [DF_PerformanceSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_UserCountDW_CreatedOn]    Script Date: 10/11/2011 13:32:10 ******/
ALTER TABLE [eddsdbo].[UserCountDW] ADD  CONSTRAINT [DF_UserCountDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_SQLServerSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[SQLServerSummary] ADD  CONSTRAINT [DF_SQLServerSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_SQLServerDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[SQLServerDW] ADD  CONSTRAINT [DF_SQLServerDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ServerSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerSummary] ADD  CONSTRAINT [DF_ServerSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ServerProcessorSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerProcessorSummary] ADD  CONSTRAINT [DF_ServerProcessorSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ServerProcessorDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerProcessorDW] ADD  CONSTRAINT [DF_ServerProcessorDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ServerDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerDW] ADD  CONSTRAINT [DF_ServerDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ServerDiskSummary_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerDiskSummary] ADD  CONSTRAINT [DF_ServerDiskSummary_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_ServerDiskDW_CreatedOn]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerDiskDW] ADD  CONSTRAINT [DF_ServerDiskDW_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_Measure_IsActive]    Script Date: 10/11/2011 13:32:15 ******/
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_Measure_IsDeleted]    Script Date: 10/11/2011 13:32:15 ******/
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
/****** Object:  Default [DF_Measure_CreatedOn]    Script Date: 10/11/2011 13:32:15 ******/
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
/****** Object:  Default [DF_Measure_Frequency]    Script Date: 10/11/2011 13:32:15 ******/
ALTER TABLE [eddsdbo].[Measure] ADD  CONSTRAINT [DF_Measure_Frequency]  DEFAULT ((60)) FOR [Frequency]
GO
/****** Object:  ForeignKey [FK_SQLServerSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[SQLServerSummary]  WITH CHECK ADD  CONSTRAINT [FK_SQLServerSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[SQLServerSummary] CHECK CONSTRAINT [FK_SQLServerSummary_Server]
GO
/****** Object:  ForeignKey [FK_SQLServerDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[SQLServerDW]  WITH CHECK ADD  CONSTRAINT [FK_SQLServerDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[SQLServerDW] CHECK CONSTRAINT [FK_SQLServerDW_Server]
GO
/****** Object:  ForeignKey [FK_ServerSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerSummary] CHECK CONSTRAINT [FK_ServerSummary_Server]
GO
/****** Object:  ForeignKey [FK_ServerProcessorSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerProcessorSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerProcessorSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerProcessorSummary] CHECK CONSTRAINT [FK_ServerProcessorSummary_Server]
GO
/****** Object:  ForeignKey [FK_ServerProcessorDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerProcessorDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerProcessorDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerProcessorDW] CHECK CONSTRAINT [FK_ServerProcessorDW_Server]
GO
/****** Object:  ForeignKey [FK_ServerDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerDW] CHECK CONSTRAINT [FK_ServerDW_Server]
GO
/****** Object:  ForeignKey [FK_ServerDiskSummary_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerDiskSummary]  WITH CHECK ADD  CONSTRAINT [FK_ServerDiskSummary_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerDiskSummary] CHECK CONSTRAINT [FK_ServerDiskSummary_Server]
GO
/****** Object:  ForeignKey [FK_ServerDiskDW_Server]    Script Date: 10/11/2011 13:32:13 ******/
ALTER TABLE [eddsdbo].[ServerDiskDW]  WITH CHECK ADD  CONSTRAINT [FK_ServerDiskDW_Server] FOREIGN KEY([ServerID])
REFERENCES [eddsdbo].[Server] ([ServerID])
GO
ALTER TABLE [eddsdbo].[ServerDiskDW] CHECK CONSTRAINT [FK_ServerDiskDW_Server]
GO
/****** Object:  ForeignKey [FK_Measure_MeasureType]    Script Date: 10/11/2011 13:32:15 ******/
ALTER TABLE [eddsdbo].[Measure]  WITH CHECK ADD  CONSTRAINT [FK_Measure_MeasureType] FOREIGN KEY([MeasureTypeId])
REFERENCES [eddsdbo].[MeasureType] ([MeasureTypeId])
GO
ALTER TABLE [eddsdbo].[Measure] CHECK CONSTRAINT [FK_Measure_MeasureType]
GO', N'cPeX9PaGgGEf6jczT6nPQg==', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (24, 1, N'0000030_EDDSPerformance_MetaData.sql', N'SET NUMERIC_ROUNDABORT OFF
GO
SET XACT_ABORT, ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO

USE [EDDSPerformance]

BEGIN TRANSACTION

ALTER TABLE [eddsdbo].[Measure] DROP CONSTRAINT [FK_Measure_MeasureType]
SET IDENTITY_INSERT [eddsdbo].[ServerType] ON
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (1, N''Web'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (2, N''Agent'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (3, N''Database'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (4, N''Document'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (5, N''Search'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (11, N''WebAPI'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (12, N''Services'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (20, N''WebBackground'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (21, N''Processing'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (22, N''Analytics'', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (99, N''Unrecognized'', NULL)
SET IDENTITY_INSERT [eddsdbo].[ServerType] OFF
INSERT INTO [eddsdbo].[MeasureType] ([MeasureTypeId], [MeasureTypeCd], [MeasureTypeDesc], [IsActive], [IsDeleted], [UpdatedOn]) VALUES (1, N''AppHealth'', N''Application Health Diagnostics'', 1, 0, NULL)
INSERT INTO [eddsdbo].[MeasureType] ([MeasureTypeId], [MeasureTypeCd], [MeasureTypeDesc], [IsActive], [IsDeleted], [UpdatedOn]) VALUES (2, N''ServerHealth'', N''Server Health Diagnostics'', 1, 0, NULL)
INSERT INTO [eddsdbo].[MeasureType] ([MeasureTypeId], [MeasureTypeCd], [MeasureTypeDesc], [IsActive], [IsDeleted], [UpdatedOn]) VALUES (3, N''ServerDiskHealth'', N''Server Disk Health Diagnostics'', 1, 0, NULL)
INSERT INTO [eddsdbo].[MeasureType] ([MeasureTypeId], [MeasureTypeCd], [MeasureTypeDesc], [IsActive], [IsDeleted], [UpdatedOn]) VALUES (4, N''ServerProcessorHealth '', N''Server Processor Health Diagnostics'', 1, 0, NULL)
INSERT INTO [eddsdbo].[MeasureType] ([MeasureTypeId], [MeasureTypeCd], [MeasureTypeDesc], [IsActive], [IsDeleted], [UpdatedOn]) VALUES (5, N''SQLServerHealth'', N''SQL Server Health Diagnostics'', 1, 0, NULL)
INSERT INTO [eddsdbo].[MeasureType] ([MeasureTypeId], [MeasureTypeCd], [MeasureTypeDesc], [IsActive], [IsDeleted], [UpdatedOn]) VALUES (6, N''BISHealth'', N''BIS Health Diagnostics'' ,1,0, NULL)
SET IDENTITY_INSERT [eddsdbo].[Measure] ON
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (1, N''LRQ'', N''Long Running Queries'', 1, 1, 0, NULL, 60)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (2, N''Errors'', N''Critical Errors'', 1, 1, 0, NULL, 60)

INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (3, N''Latency'', N''Average Latency'', 1, 0, 1, NULL, 60)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (4, N''Users'', N''Active Users'', 1, 1, 0, NULL, 5)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (5, N''RAMPagesPerSec'', N''RAM Pages/Sec'', 2, 1, 0, NULL, 5)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (6, N''RAMPageFaultsPerSec'', N''RAM Page Faults/Sec'', 2, 1, 0, NULL, 5)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (7, N''DiskAvgSecPerRead'', N''Disk Avg Sec/Read'', 3, 1, 0, NULL, 5)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (8, N''DiskAvgSecPerWrite'', N''Disk Avg Sec/Write'', 3, 1, 0, NULL, 5)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (10, N''CPUProcessorTimePct'', N''CPU Processor Time (%)'', 4, 1, 0, NULL, 5)
INSERT INTO [eddsdbo].[Measure] ([MeasureID], [MeasureCd], [MeasureDesc], [MeasureTypeId], [IsActive], [IsDeleted], [UpdatedOn], [Frequency]) VALUES (11, N''SQLPageLifeExpectancy'', N''Page Life Expectancy'', 5, 1, 0, NULL, 5)
SET IDENTITY_INSERT [eddsdbo].[Measure] OFF
INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (1, N''Application Metrics DW Load'', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)
INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (2, N''Server Health Summary'', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)
INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (3, N''Server Info Refresh'', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 1440)
INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (4, N''BISSummary Refresh'', DATEADD(d, -91, DATEADD(hour, DATEDIFF(hh, 0, GETUTCDATE()), 0)), 1440)
INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (5, N''Install Server Scripts'', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)
INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (6, N''Install Workspace Scripts'', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)
INSERT INTO [eddsdbo].[ProcessControl] ([ProcessControlID], [ProcessTypeDesc], [LastProcessExecDateTime], [Frequency]) VALUES (7, N''Run Looking Glass'', DATEADD(HOUR, DATEDIFF(HH, 0, GETUTCDATE()), 0), 60)
ALTER TABLE [eddsdbo].[Measure] ADD CONSTRAINT [FK_Measure_MeasureType] FOREIGN KEY ([MeasureTypeId]) REFERENCES [eddsdbo].[MeasureType] ([MeasureTypeId])

INSERT INTO [eddsdbo].[Version] ([ApplicationVersion]) VALUES (''7.5.0.1'')

INSERT INTO [eddsdbo].[Configuration] ([Section],[Name],[Value],[MachineName],[Description]) VALUES (N''kCura.PDB'',N''ShowVersion'',N''true'',N'''',N'''')
INSERT INTO [eddsdbo].[Configuration] ([Section],[Name],[Value],[MachineName],[Description]) VALUES (N''kCura.PDB'',N''ShowCustomErrorPage'',N''false'',N'''',N'''')
INSERT INTO [eddsdbo].[Configuration] ([Section],[Name],[Value],[MachineName],[Description]) VALUES (N''kCura.PDB'',N''AssemblyName'',N''kCura.PDD.Service.Task'',N'''',N'''')
INSERT INTO [eddsdbo].[Configuration] ([Section],[Name],[Value],[MachineName],[Description]) VALUES (N''kCura.PDB'',N''HealthTask'',N''kCura.PDD.Service.Task.Impl.HealthTask'',N'''',N'''')
INSERT INTO [eddsdbo].[Configuration] ([Section],[Name],[Value],[MachineName],[Description]) VALUES (N''kCura.PDB'',N''PerformanceTaskFactory'',N''kCura.PDD.WindowsService.Task.Implementation.PerformanceTaskFactory'',N'''',N'''')
INSERT INTO [eddsdbo].[Configuration] ([Section],[Name],[Value],[MachineName],[Description]) VALUES (N''kCura.PDB'',N''DiagnosticAnalysisTask'',N''kCura.DFG.Task.Implementation.DiagnosticAnalysisTask'',N'''',N'''')

COMMIT TRANSACTION
', N'5bH6h0Ew7Wq+KvObdxh1zQ==', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (25, 1, N'0000022_DROP_eddsdbo.Table.JustinTable.sql', N'USE EDDSPerformance;
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = ''eddsdbo'' 
                 AND  TABLE_NAME = ''JustinTable''))
BEGIN
    DROP TABLE eddsdbo.JustinTable
END
', N'kfUjmIW4H55ehZvNbP4ogA==', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (26, 1, N'eddsdbo.DateHourTable.sql', N'USE [EDDSPerformance]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N''eddsdbo.DateHourTable'')) DROP FUNCTION eddsdbo.DateHourTable
GO

CREATE FUNCTION [eddsdbo].[DateHourTable]
(
	@FirstDate	datetime,
	@LastDate	datetime
)
RETURNS @datetable TABLE (
	[date]		datetime
)
AS
BEGIN

  SELECT @FirstDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @FirstDate));   SELECT @LastDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @LastDate)); 
  WITH CTE_DatesTable
  AS 
  (
    SELECT @FirstDate AS [date]
    UNION ALL
    SELECT DATEADD(HH, 1, [date])
    FROM CTE_DatesTable
    WHERE DATEADD(HH, 1, [date]) <= @LastDate
  )
  INSERT INTO @datetable ([date])
  SELECT [date] FROM CTE_DatesTable
  OPTION (MAXRECURSION 0)

  RETURN
END
 ', N'isRB2thk8mOsIFwW9GVNNA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (27, 1, N'eddsdbo.DateTable.sql', N'USE [EDDSPerformance]
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N''eddsdbo.DateTable'')) DROP FUNCTION eddsdbo.DateTable
GO

CREATE FUNCTION [eddsdbo].[DateTable]
(
	@FirstDate	datetime,
	@LastDate	datetime
)
RETURNS @datetable TABLE (
	[date]		datetime
)
AS
BEGIN

  SELECT @FirstDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @FirstDate));   SELECT @LastDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @LastDate)); 
  WITH CTE_DatesTable
  AS 
  (
    SELECT @FirstDate AS [date]
    UNION ALL
    SELECT DATEADD(dd, 1, [date])
    FROM CTE_DatesTable
    WHERE DATEADD(dd, 1, [date]) <= @LastDate
  )
  INSERT INTO @datetable ([date])
  SELECT [date] FROM CTE_DatesTable
  OPTION (MAXRECURSION 0)

  RETURN
END
 ', N'/cfS5GE850KLWZ+jZcJV+A==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (28, 1, N'eddsdbo.GetDateRange.sql', N'USE [EDDSPerformance]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N''eddsdbo.GetDateRange'')) DROP FUNCTION eddsdbo.GetDateRange
GO

CREATE function [eddsdbo].[GetDateRange]
(@StartDate datetime,@EndDate datetime)				
RETURNS @DateRangeTable Table (DateRange datetime)			
as
Begin
	declare @blEqual bit =0	
	declare @HourDifference int = 0	
	if (@StartDate = @EndDate)
	begin				
		set  @EndDate = DateAdd(S,-1,@EndDate) +1 ;        
		set  @blEqual = 1;
	end
	      
	while (@StartDate <= @EndDate)
	begin                
		insert into @DateRangeTable select(@StartDate)	    
		if (@blEqual = 1)
		begin
			set @StartDate =  DateAdd(HH,1,@StartDate);
		end
		else
		begin
			set @StartDate = @StartDate + 1;
		end
	end	
	return 
End
 ', N'Wg3aBKYLmup58y3zlxUm1Q==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (30, 1, N'0000001_eddsdbo.EDDSWorkspace.sql', N'
DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''EDDSWorkspace'', @Type = ''VIEW'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

ALTER VIEW  [eddsdbo].[EDDSWorkspace]
AS
SELECT  
	[Case].ArtifactID		AS CaseArtifactID
	, [Case].[Name]			AS WorkspaceName
	, (
		CASE 
			WHEN CHARINDEX(''\'', [ResourceServer].Name) > 0 
				THEN SUBSTRING( [ResourceServer].Name, CHARINDEX(''\'', [ResourceServer].Name) + 1, LEN( [ResourceServer].Name ) )
			ELSE [ResourceServer].Name
		END		
	  ) AS [DatabaseLocation]
FROM [EDDS].[eddsdbo].[Case] AS [Case] WITH (NOLOCK)
	INNER JOIN [EDDS].[eddsdbo].[ResourceServer] [ResourceServer] WITH (NOLOCK)
		ON [ResourceServer].artifactId = [Case].ServerId

', N'SXPkn5EpL6nmR20Xr8udRw==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (31, 1, N'0000002_eddsdbo.CurrentDayPerformanceSummary.sql', N'USE EDDSPerformance;


DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''CurrentDayPerformanceSummary'', @Type = ''VIEW'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

ALTER VIEW  [eddsdbo].[CurrentDayPerformanceSummary]
AS
SELECT 
       PC.[CaseArtifactID]
      ,WS.[WorkspaceName]
      ,SUM([UserCount]) [UserCount]
      ,SUM([ErrorCount]) [ErrorCount]
      ,SUM([LRQCount]) [LRQCount]
      ,AVG([AverageLatency]) [AverageLatency]
  FROM [eddsdbo].[PerformanceSummary] PC
	Inner Join [eddsdbo].EDDSWorkspace WS
	On PC.CaseArtifactID = WS.CaseArtifactID
	Where [MeasureDate] between DATEADD(HH,-24,GETUTCDATE()) and GETUTCDATE()
Group By          PC.[CaseArtifactID]
      ,WS.[WorkspaceName]

', N'Vkamh/6U1PhfLHsFMOH1Cg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (32, 1, N'0000003_eddsdbo.CurrentDayPerformanceSummaryByHour.sql', N'
DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''CurrentDayPerformanceSummaryByHour'', @Type = ''VIEW'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

ALTER VIEW  [eddsdbo].[CurrentDayPerformanceSummaryByHour]
AS
SELECT 
       PC.[CaseArtifactID]
      ,WS.[WorkspaceName]
      ,PC.MeasureDate
      ,SUM([UserCount]) [UserCount]
      ,SUM([ErrorCount]) [ErrorCount]
      ,SUM([LRQCount]) [LRQCount]
      ,AVG([AverageLatency]) [AverageLatency]
  FROM [eddsdbo].[PerformanceSummary] PC
	Inner Join [eddsdbo].EDDSWorkspace WS
	On PC.CaseArtifactID = WS.CaseArtifactID
	Where [MeasureDate] between DATEADD(HH,-24,GETUTCDATE()) and GETUTCDATE()
Group By          PC.[CaseArtifactID]
      ,WS.[WorkspaceName],PC.MeasureDate

', N'4JCo+sniwEoTI89JiY2xIg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (33, 1, N'eddsdbo.GetApplicationHealthData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''GetApplicationHealthData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO


ALTER PROCEDURE  [eddsdbo].[GetApplicationHealthData]

	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset int

AS
BEGIN
	/*
		3/11/2014 - Joseph Low -
			Removed BISIndicator from the output
	*/

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	/* 
		10/31/2013 - Ryan Flint -  
			Fixed condition when @StartDate != @EndDate by changing Where clause 
			on the dates from a BETWEEN to explicit >= AND <
	*/
	
	
	SET NOCOUNT ON;

	DECLARE @pwd NVarchar(100)
	SET @pwd = ''kCuraPassword1!''

	DECLARE @SQLTimeZoneOffset INT
	Set @SQLTimeZoneOffset  = DATEDIFF(MINUTE, GETUTCDATE(), GETDATE()) 

	IF (@StartDate IS NULL AND @EndDate IS NULL OR @StartDate = CAST(DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE()) as DATE))
	BEGIN
		IF(@StartDate IS NULL AND @EndDate IS NULL)	-- Get the past 24 hours
		BEGIN
			SELECT @StartDate = DATEADD(HH,-24,DATEADD(HH, DATEDIFF(HH, 0, GETUTCDATE()) ,0))					
			SELECT @EndDate = DATEADD(HH, 0, DATEADD(HH, DATEDIFF(HH, 0, GETUTCDATE()) ,0))
		END
		ELSE	-- Pull data since midnight today
		BEGIN
			SET @EndDate = DATEADD(DD, 1, @StartDate)
		END
		
		SELECT
			ROW_NUMBER() OVER(ORDER BY ps.CaseArtifactID, ps.measureDate) AS Id				
			, ISNULL(ps.CaseArtifactID,0) as CaseArtifactID
			, ISNULL(w.WorkspaceName,'''') as WorkspaceName				
			, ISNULL(w.[DatabaseLocation],'''') as [DatabaseLocation]
			, DATEADD(MINUTE, @timezoneoffset, ps.MeasureDate) as MeasureDate
			, ISNULL(PS.UserCount, -1) AS UserCount
			, ISNULL(PS.ErrorCount, -1) AS ErrorCount
			, ISNULL(PS.LRQCount, -1) AS LRQCount
			, ISNULL(PS.AverageLatency,-1) AS AverageLatency
			,ISNULL(BIS.TotalQCount,0) AS TQCount
			,ISNULL(BIS.TotalNRQCount, 0) as TotalNRQCount
			,ISNULL(BIS.NRLRQCount, 0) as NRLRQCount
			,ISNULL(DocumentCount, 0) as DocumentCount
		
			FROM [EDDSPerformance].[eddsdbo].[PerformanceSummary] ps

			JOIN eddsperformance.eddsdbo.EDDSWorkspace w
			ON ps.CaseArtifactID = w.CaseArtifactID
			
			LEFT JOIN EDDS.eddsdbo.CaseStatistics CS
				--ON DATEADD(MI, @TimeZoneOffset, DATEADD(HOUR, DATEPART(HOUR,MeasureDate), 
				--CONVERT(varchar(10), cs.timestamp ,101))) = ps.MeasureDate	
				ON cs.timestamp = ps.MeasureDate			    			  
				AND ps.CaseArtifactID = CS.CaseArtifactID
		
			LEFT JOIN (
				SELECT
					LDW.CaseArtifactID AS CaseArtifactID,
					SUM(LDW.NRLRQCount) AS NRLRQCount,
					SUM(LDW.TotalQCount) AS TotalQCount,
					SUM(LDW.TotalNRQCount) AS TotalNRQCount,
					--DATEADD(HOUR,LDW.MeasureHour, cast(LDW.MeasureDate as datetime)) MeasureDate
					cast(LDW.MeasureDate as datetime) MeasureDate
					
				FROM
					eddsdbo.LRQCountDW LDW
				WHERE				
					cast(LDW.MeasureDate as datetime) < dateadd(MI,@SQLTimeZoneOffset, @EndDate + 1)
					and cast(LDW.MeasureDate as datetime) >= dateadd(MI,@SQLTimeZoneOffset, (@StartDate))
					
					--DATEADD(HOUR,LDW.MeasureHour, cast(LDW.MeasureDate as datetime)) < dateadd(MI,@SQLTimeZoneOffset, @EndDate + 1)
					--and DATEADD(HOUR,LDW.MeasureHour, cast(LDW.MeasureDate as datetime)) >= dateadd(MI,@SQLTimeZoneOffset, (@StartDate))
					
				GROUP BY
					--LDW.CaseArtifactID, DATEADD(HOUR,LDW.MeasureHour, cast(LDW.MeasureDate as datetime)) --order by offset
					LDW.CaseArtifactID, cast(LDW.MeasureDate as datetime) --order by offset
			) BIS 
			ON ps.CaseArtifactID = bis.CaseArtifactID and bis.MeasureDate = @StartDate	
			
			WHERE ps.MeasureDate between @StartDate and @EndDate
			
	END

	ELSE
	BEGIN
	IF (@StartDate = @EndDate AND @endDate != DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))		
	BEGIN
		SET @EndDate = DATEADD(DD, 1, @StartDate)		
		
		SELECT
			ROW_NUMBER() OVER(ORDER BY ps.CaseArtifactID, ps.measureDate) AS Id				
			, ISNULL(ps.CaseArtifactID,0) as CaseArtifactID
			, ISNULL(w.WorkspaceName,'''') as WorkspaceName				
			, ISNULL(w.[DatabaseLocation],'''') as [DatabaseLocation]
			, DATEADD(MINUTE, -@timezoneoffset, ps.[MeasureDate]) as MeasureDate	-- negative offset to push display date back to local time
			, ISNULL(PS.UserCount, -1) AS UserCount
			, ISNULL(PS.ErrorCount, -1) AS ErrorCount
			, ISNULL(PS.LRQCount, -1) AS LRQCount
			, ISNULL(PS.AverageLatency,-1) AS AverageLatency
			,ISNULL(BIS.TQCount,0) AS TQCount
			,ISNULL(BIS.TotalNRQCount, 0) as TotalNRQCount
			,ISNULL(BIS.NRLRQCount, 0) as NRLRQCount
			,ISNULL(BIS.DocumentCount, 0) as DocumentCount
		
			FROM [EDDSPerformance].[eddsdbo].[PerformanceSummary] ps

			JOIN eddsperformance.eddsdbo.EDDSWorkspace w
			ON ps.CaseArtifactID = w.CaseArtifactID
			
			INNER JOIN EDDSPerformance.eddsdbo.BISSummary BIS
			ON ps.CaseArtifactID = bis.CaseArtifactID and bis.MeasureDate = @StartDate	
			
			WHERE ps.MeasureDate >= DATEADD(MINUTE, @TimeZoneOffset, @StartDate)
			AND ps.MeasureDate < DATEADD(MINUTE, @TimeZoneOffset, @EndDate)
			
	END
	ELSE		
	BEGIN	
		SELECT
			ROW_NUMBER() OVER(ORDER BY ps.CaseArtifactID, ps.measureDate) AS Id				
			,ps.[CaseArtifactID]
			,w.WorkspaceName
			,w.DatabaseLocation
			,ps.MeasureDate
			,ps.[UserCount]
			,ps.[ErrorCount]
			,ps.[LRQCount]
			,ps.[AverageLatency]
			,ISNULL(BIS.TQCount,0) AS TQCount
			,ISNULL(BIS.TotalNRQCount, 0) as TotalNRQCount
			,ISNULL(BIS.NRLRQCount, 0) as NRLRQCount
			,ISNULL(BIS.DocumentCount, 0) as DocumentCount
		
			FROM 
			(
				SELECT 
					P.CaseArtifactID
					, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, -@TimeZoneOffset, p.MeasureDate))) [MeasureDate]
					, MAX(P.UserCount) AS UserCount
					, SUM(P.ErrorCount) AS ErrorCount
					, SUM(P.LRQCount) AS LRQCount
					, AVG(P.AverageLatency)  AS AverageLatency
				FROM eddsdbo.PerformanceSummary P
				WHERE 
					--p.MeasureDate between DATEADD(MI, @TimeZoneOffset, @StartDate) and DATEADD(MI, @TimeZoneOffset, @EndDate + 1)
					p.MeasureDate >= DATEADD(MI, @TimeZoneOffset, @StartDate) 
					AND p.MeasureDate < DATEADD(MI, @TimeZoneOffset, @EndDate + 1)
				GROUP BY P.CaseArtifactID
					, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, -@TimeZoneOffset, p.MeasureDate )))
						
			) as ps
			
			JOIN eddsperformance.eddsdbo.EDDSWorkspace w
			ON ps.CaseArtifactID = w.CaseArtifactID
			
			LEFT JOIN EDDSPerformance.eddsdbo.BISSummary BIS
			ON ps.CaseArtifactID = bis.CaseArtifactID and ps.MeasureDate = bis.MeasureDate
			
			--WHERE ps.MeasureDate BETWEEN DATEADD(MINUTE, @TimeZoneOffset, @StartDate) AND DATEADD(MINUTE, @TimeZoneOffset, @EndDate +1)
			--WHERE ps.MeasureDate >= DATEADD(MI, @TimeZoneOffset, @StartDate) AND ps.MeasureDate < DATEADD(MI, @TimeZoneOffset, @EndDate + 1)
			
				
	END	
	END
END


', N'2p4Ld70lKcGP/ZiozDGlRg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (36, 1, N'eddsdbo.GetBISHealthData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''GetBISHealthData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO


ALTER PROCEDURE  [eddsdbo].[GetBISHealthData]
(
	@WorkspaceId int,
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset int
)
  
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @pwd NVarchar(100)
	SET @pwd = ''kCuraPassword1!''
	
	Set @TimeZoneOffset  = DATEDIFF(MINUTE, GETUTCDATE(), GETDATE()) * -1
	
	IF (@StartDate IS NULL AND @EndDate IS NULL)
		BEGIN
			SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))				
			SET @EndDate = @StartDate			
		END
	ELSE
		BEGIN
			Set @StartDate = DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, @StartDate)) ,0)
			Set @EndDate = DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, @EndDate)) ,0)
		END
	
	SELECT
		ROW_NUMBER() OVER(ORDER BY WS.CaseArtifactID, DR.DateRange) AS Id								
		, WS.CaseArtifactID as CaseArtifactID
		, WS.WorkspaceName as WorkspaceName						
		, DR.DateRange as MeasureDate
		, ISNULL(PS.BISIndicator,0) AS BISIndicator
	FROM 
		[eddsdbo].GetDateRange(CAST(@StartDate as DATE), CAST(@EndDate as DATE)) DR
		CROSS JOIN 
		(
			SELECT DISTINCT 
				w.CaseArtifactID
				,w.WorkspaceName
			FROM 
				eddsdbo.EDDSWorkspace w 
			WHERE
				w.CaseArtifactID = @WorkspaceId
		) AS WS		
		LEFT JOIN 
		(
			SELECT 
				BS.CaseArtifactID
				, MeasureDate AS [MeasureDate]
				, BS.StatusDay AS [BISIndicator]
			FROM 
				eddsdbo.BISSummary BS
			WHERE 
				MeasureDate >=  CAST(@StartDate as DATE)
				AND MeasureDate < DATEADD(DAY, +1, CAST(@EndDate as DATE))
				AND BS.CaseArtifactID = @WorkspaceId
		) AS PS on PS.MeasureDate = dr.DateRange 
		AND WS.CaseArtifactID = PS.CaseArtifactID
END



/****** Object:  Default [DF_Server_CreatedOn]    Script Date: 10/11/2011 13:32:09 ******/
ALTER TABLE [eddsdbo].[Server] ADD  CONSTRAINT [DF_Server_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

', N'gtCkfTKGoxsBAIE0jXizlg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (37, 1, N'eddsdbo.GetRAMHealthData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''GetRAMHealthData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Jayesh Dhobi
-- Create date: 25th August 2011
-- Description:	Getting ServerSummary data 
-- =============================================
-- exec [eddsdbo].[GetRAMHealthData] null, null , -300
-- exec [eddsdbo].[GetRAMHealthData] ''2011-09-01'', ''2011-10-01'' , -300
ALTER PROCEDURE  [eddsdbo].[GetRAMHealthData]
(
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset INT	-- the difference between local time at the client and GMT (in minutes)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF (@StartDate IS NULL AND @EndDate IS NULL)
	BEGIN
		 SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))		
		 SET @EndDate = @StartDate
	END	
	 --select @StartDate
    -- Insert statements for procedure here
	 IF (@StartDate = @EndDate)		
		 BEGIN		 
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(ss.RAMPagesPerSec,-1) as RAMPagesPerSec 
				, ISNULL(ss.RAMPageFaultsPerSec,-1) as RAMPageFaultsPerSec	
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.ServerSummary ss on ss.ServerID = s.ServerID 								
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1				
			) as [ServerInfo]
			  left join eddsdbo.ServerSummary ss
			  on  DATEADD(MI, @TimeZoneOffset, DATEADD(HOUR, DATEPART(HOUR,MeasureDate), CONVERT(varchar(10), ss.MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))				    			  
			  and [ServerInfo].ServerID = ss.ServerID				  
			  
		END
	ELSE
		BEGIN	
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id								
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(SS.RAMPagesPerSec,-1) AS RAMPagesPerSec
				, ISNULL(SS.RAMPageFaultsPerSec,-1) AS RAMPageFaultsPerSec
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.ServerSummary SS on SS.ServerID = s.ServerID 
				AND DATEADD(MI, @TimeZoneOffset,MeasureDate) >=  @StartDate and DATEADD(MI, @TimeZoneOffset,MeasureDate)  < (@EndDate + 1)
			) as [ServerInfo]
			left join (
				select SS.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate],
				 AVG(SS.RAMPagesPerSec) AS RAMPagesPerSec , AVG(SS.RAMPageFaultsPerSec) AS RAMPageFaultsPerSec
				from eddsdbo.ServerSummary SS
				where DATEADD(MI, @TimeZoneOffset,MeasureDate) >=  @StartDate and DATEADD(MI, @TimeZoneOffset,MeasureDate)  < (@EndDate + 1)
				group by SS.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
			) as SS on SS.MeasureDate = dr.DateRange and [ServerInfo].ServerID = SS.ServerID 
		END	
END

', N'6hsychzN4M+iHvctoN8A0w==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (40, 1, N'eddsdbo.GetServerDiskSummaryData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''GetServerDiskSummaryData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Konstantin Kekhaev
-- Create date: 06 October 2011
-- Description:	Getting ServerDiskSummary data 
-- =============================================
-- exec [eddsdbo].[GetServerDiskSummaryData] ''2011-09-15'',''2011-10-04'' ,330
ALTER PROCEDURE  [eddsdbo].[GetServerDiskSummaryData]
(
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset int 
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	IF (@StartDate IS NULL AND @EndDate IS NULL)
	BEGIN
		-- hourly
		--SELECT @StartDate = DATEADD(HH, - 23, GETUTCDATE())
		SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))				
		SET @EndDate = @StartDate			
	END
	
    -- Insert statements for procedure here
     IF (@StartDate = @EndDate)		
		BEGIN
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID * 10 + [ServerInfo].DiskNumber as ServerDiskId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, [ServerInfo].DiskNumber
				, [ServerInfo].DriveLetter
				, ISNULL(sds.DiskAvgSecPerRead, -1) AS DiskAvgSecPerRead
				, ISNULL(sds.DiskAvgSecPerWrite, -1) AS DiskAvgSecPerWrite
				from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
				cross join (
					select distinct s.ServerID, sds.DiskNumber, sds.DriveLetter, s.ServerName, st.ServerTypeName
					from eddsdbo.Server s
					join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
					join eddsdbo.ServerDiskSummary sds on sds.ServerID = s.ServerID 
					AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1
				) as [ServerInfo]
				left join eddsdbo.ServerDiskSummary sds
				  on  DATEADD(MI, @TimeZoneOffset, DATEADD(HOUR, DATEPART(HOUR,MeasureDate), CONVERT(varchar(10), MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))
				  and [ServerInfo].ServerID = sds.ServerID
				  and [ServerInfo].DiskNumber = sds.DiskNumber
		END
	ELSE
		BEGIN
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange, [ServerInfo].DiskNumber) AS Id				
				, [ServerInfo].ServerID * 10 + [ServerInfo].DiskNumber as ServerDiskId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, [ServerInfo].DiskNumber
				, [ServerInfo].DriveLetter
				, ISNULL(sds.DiskAvgSecPerRead, -1) AS DiskAvgSecPerRead
				, ISNULL(sds.DiskAvgSecPerWrite, -1) AS DiskAvgSecPerWrite
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, sds.DiskNumber, sds.DriveLetter, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.ServerDiskSummary sds on sds.ServerID = s.ServerID 
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
			) as [ServerInfo]
			left join (
				select sds.ServerID, sds.DiskNumber, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate],
					   AVG(sds.DiskAvgSecPerRead) [DiskAvgSecPerRead], AVG(sds.DiskAvgSecPerWrite) [DiskAvgSecPerWrite]
				from eddsdbo.ServerDiskSummary sds
				where DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
				group by sds.ServerID, sds.DiskNumber, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
			) as sds on sds.MeasureDate = dr.DateRange and [ServerInfo].ServerID = sds.ServerID and [ServerInfo].DiskNumber = sds.DiskNumber

		END
END

', N'wafeRa9Mylo+2E9DyHTpkA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (43, 1, N'eddsdbo.GetServerProcessorSummaryData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''GetServerProcessorSummaryData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Jayesh Dhobi
-- Create date: 25th August 2011
-- Description:	Getting ServerProcessorSummary data 
-- =============================================
-- exec [eddsdbo].[GetServerProcessorSummaryData] ''9/22/2011 12:00:00 AM'',''9/22/2011 12:00:00 AM'',0
-- exec [eddsdbo].[GetServerProcessorSummaryData] null, null,0
ALTER PROCEDURE  [eddsdbo].[GetServerProcessorSummaryData]
(
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset INT	-- the difference between local time at the client and GMT (in minutes)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	IF (@StartDate IS NULL AND @EndDate IS NULL)
	BEGIN
		-- hourly
		--SELECT @StartDate = DATEADD(HH, - 23, GETUTCDATE())
		SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))
		SET @EndDate = @StartDate		
	END
	
    -- Insert statements for procedure here
     IF (@StartDate = @EndDate)		
		BEGIN
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID * 10 as ServerCoreId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sps.CPUProcessorTimePct, -1) AS CPUProcessorTimePct
			FROM [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
				CROSS JOIN 
				(
					SELECT DISTINCT 
						s.ServerID
						, sps.CoreNumber
						, s.ServerName
						, st.ServerTypeName
					FROM eddsdbo.Server s
						JOIN eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
						JOIN eddsdbo.ServerProcessorSummary sps on sps.ServerID = s.ServerID 
							AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate 
							AND DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1
					WHERE sps.CoreNumber = -1
				) as [ServerInfo]
				LEFT JOIN eddsdbo.ServerProcessorSummary sps
				  on  DATEADD(MI, @TimeZoneOffset,DATEADD(HOUR, DATEPART(HOUR, MeasureDate), CONVERT(varchar(10), MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))
				  AND [ServerInfo].ServerID = sps.ServerID
				  AND [ServerInfo].CoreNumber = sps.CoreNumber
		END
	ELSE
		BEGIN
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID * 10 as ServerCoreId
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sps.CPUProcessorTimePct, -1) AS CPUProcessorTimePct
			FROM [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
				CROSS JOIN 
				(
					SELECT DISTINCT 
						s.ServerID
						, sps.CoreNumber
						, s.ServerName
						, st.ServerTypeName
					FROM eddsdbo.Server s
						JOIN eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
						JOIN eddsdbo.ServerProcessorSummary sps on sps.ServerID = s.ServerID 
							AND DATEADD(MI, @TimeZoneOffset,MeasureDate) >= @StartDate 
							AND DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
					WHERE sps.CoreNumber = -1
				) as [ServerInfo]
				LEFT JOIN 
				(
					SELECT 
						sps.ServerID
						, sps.CoreNumber
						, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate]
						, AVG(sps.CPUProcessorTimePct) [CPUProcessorTimePct]
					FROM eddsdbo.ServerProcessorSummary sps
					WHERE DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate 
						AND DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
					GROUP BY sps.ServerID
						, sps.CoreNumber
						, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
				) as sps on sps.MeasureDate = dr.DateRange 
					AND [ServerInfo].ServerID = sps.ServerID 
					AND [ServerInfo].CoreNumber = sps.CoreNumber
		END
END

', N'Z5S2vlgBB9uVFygokhNfyA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (46, 1, N'eddsdbo.GetSQLServerSummaryData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''GetSQLServerSummaryData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Jayesh Dhobi
-- Create date: 25th August 2011
-- Description:	Getting SQLServerSummary data 
-- =============================================
ALTER PROCEDURE  [eddsdbo].[GetSQLServerSummaryData]
(
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimeZoneOffset INT	-- the difference between local time at the client and GMT (in minutes)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @DatabaseServerTypeId INT = 3	
	
	IF (@StartDate IS NULL AND @EndDate IS NULL)
	BEGIN
		--SELECT @StartDate = DATEADD(HH, - 23, GETUTCDATE())
		SELECT @StartDate = DATEADD(HH,-23,DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, GETUTCDATE())) ,0))
		SET @EndDate = @StartDate			
	END
	
	 -- Insert statements for procedure here
	 IF (@StartDate = @EndDate)		
		 BEGIN		 
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id				
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sss.SQLPageLifeExpectancy, -1) AS SQLPageLifeExpectancy
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.SQLServerSummary sss on sss.ServerID = s.ServerID 				
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < @EndDate + 1
			) as [ServerInfo]
			  left join eddsdbo.SQLServerSummary sss
			  on  DATEADD(MI, @TimeZoneOffset, DATEADD(HOUR, DATEPART(HOUR,MeasureDate), CONVERT(varchar(10), MeasureDate ,101))) = DATEADD(HOUR, DATEPART(HOUR,dr.DateRange), CONVERT(varchar(10), dr.DateRange,101))
			  and [ServerInfo].ServerID = sss.ServerID				  
		END
	ELSE
		BEGIN	
			SELECT
				ROW_NUMBER() OVER(ORDER BY [ServerInfo].ServerID, DateRange) AS Id								
				, [ServerInfo].ServerID
				, [ServerInfo].ServerName
				, [ServerInfo].ServerTypeName
				, dr.DateRange as MeasureDate
				, ISNULL(sss.SQLPageLifeExpectancy, -1) AS SQLPageLifeExpectancy								
			from [eddsdbo].GetDateRange(@StartDate, @EndDate) dr
			cross join (
				select distinct s.ServerID, s.ServerName, st.ServerTypeName
				from eddsdbo.Server s
				join eddsdbo.ServerType st on st.ServerTypeID = s.ServerTypeID
				join eddsdbo.SQLServerSummary sss on sss.ServerID = s.ServerID 
				AND DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
			) as [ServerInfo]
			left join (
				select sss.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate))) [MeasureDate],
				 AVG(sss.SQLPageLifeExpectancy) AS SQLPageLifeExpectancy
				from eddsdbo.SQLServerSummary sss
				where DATEADD(MI, @TimeZoneOffset, MeasureDate) >= @StartDate and DATEADD(MI, @TimeZoneOffset, MeasureDate) < (@EndDate + 1)
				group by sss.ServerID, DATEADD(DD, 0, DATEDIFF(DD, 0, DATEADD(MI, @TimeZoneOffset, MeasureDate)))
			) as sss on sss.MeasureDate = dr.DateRange and [ServerInfo].ServerID = sss.ServerID 
		END	
END

', N'hpM1VA6fY+TRAbHDpmGaJw==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (50, 1, N'eddsdbo.LoadApplicationHealthSummary.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''LoadApplicationHealthSummary'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Murali Shesham
-- Create date: 08/31/2011
-- Description:	
-- 2012-05-30 : Ron @ Milyli : Altered to add new fields
-- 2013-02-15 : Ron @ Milyli : Added new TotalNRQCount (total non-relational queries)
-- =============================================
ALTER PROCEDURE  [eddsdbo].[LoadApplicationHealthSummary]
	@ProcessExecDate DateTime 
AS
BEGIN
	SET NOCOUNT ON;


	Declare @SummaryMeasureDate DateTime
	Declare @MeasureDate Date
	Declare @MeasureHour Int
	Select @MeasureDate = Cast(@ProcessExecDate as Date) , @MeasureHour = DatePart(HH,@ProcessExecDate) 
	Select @SummaryMeasureDate = DATEADD(HH,@MeasureHour, CAST(@MeasureDate as Varchar(20)))

	IF EXISTS(Select 1 From eddsdbo.PerformanceSummary Where MeasureDate = @SummaryMeasureDate)
	BEGIN 
	  PRINT ''Cannot load data more than once in an hour''
			  UPDATE PS
			  SET LRQCount = LC.LRQCount,
				  NRLRQCount = LC.NRLRQCount,
				  TotalQCount = LC.TotalQCount,
				  TotalNRQCount = LC.TotalNRQCount
			  FROM [EDDSPerformance].[eddsdbo].[PerformanceSummary] AS PS  
				INNER JOIN [EDDSPerformance].[eddsdbo].LRQCountDW AS LC
				ON PS.CaseArtifactID = LC.CaseArtifactID 
				AND DATEPART(HH,PS.MeasureDate) =  LC.MeasureHour 
				AND CAST(PS.MeasureDate AS DATE) = LC.MeasureDate
				AND PS.CaseArtifactID = LC.CaseArtifactID
	END
	ELSE
	BEGIN
		BEGIN TRAN
			  UPDATE PS
			  SET LRQCount = LC.LRQCount,
				  NRLRQCount = LC.NRLRQCount,
				  TotalQCount = LC.TotalQCount,
				  TotalNRQCount = LC.TotalNRQCount
			  FROM [EDDSPerformance].[eddsdbo].[PerformanceSummary] AS PS  
				INNER JOIN [EDDSPerformance].[eddsdbo].LRQCountDW AS LC
				ON PS.CaseArtifactID = LC.CaseArtifactID 
				AND DATEPART(HH,PS.MeasureDate) =  LC.MeasureHour 
				AND CAST(PS.MeasureDate AS DATE) = LC.MeasureDate
				AND PS.CaseArtifactID = LC.CaseArtifactID
		
		
 			Insert PerformanceSummary
			Select 
			GetUTCDate() CreatedOn, 
			C.ArtifactID CaseArtifactID, 	
			@SummaryMeasureDate MeasureDate, 
			COALESCE(UC.UserCount,0) UserCount,
			COALESCE(EC.ErrorCount,0) ErrorCount,
			COALESCE(LC.TQCount,0) TotalQCount, 
			COALESCE(LC.LRQCount,0) LRQCount,
			COALESCE(LC.NRLRQCount,0) NRLRQCount, 
			COALESCE(LC.TotalNRQCount, 0) TotalNRQCount,
			COALESCE(AL.AverageLatency,0) AverageLatency
			From  EDDS.eddsdbo.[Case] C
			Left Join 
			(Select CaseArtifactID, AVG(UserCount) UserCount   
			From eddsdbo.UserCountDW 
			Where MeasureDate = @MeasureDate AND MeasureHour = @MeasureHour Group By CaseArtifactID) UC
			On C.ArtifactID = UC.CaseArtifactID
			Left Join (Select CaseArtifactID, SUM(ErrorCount) ErrorCount    
			From eddsdbo.ErrorCountDW 
			Where MeasureDate = @MeasureDate AND MeasureHour = @MeasureHour  Group By CaseArtifactID) EC
			On C.ArtifactID = EC.CaseArtifactID
			Left Join (Select CaseArtifactID, SUM(TotalQCount) TQCount, SUM(LRQCount) LRQCount, SUM(NRLRQCount) NRLRQCount, SUM(TotalNRQCount) TotalNRQCount   
			From eddsdbo.LRQCountDW 
			Where MeasureDate = @MeasureDate AND MeasureHour = @MeasureHour  Group By CaseArtifactID) LC
			On C.ArtifactID = LC.CaseArtifactID
			Left Join (Select CaseArtifactID, AVG(AverageLatency) AverageLatency
			From eddsdbo.LatencyDW 
			Where MeasureDate = @MeasureDate AND MeasureHour = @MeasureHour  Group By CaseArtifactID) AL
			On C.ArtifactID = AL.CaseArtifactID
		COMMIT TRAN
	END
END

', N'f7o/SHckROYObrFNKKryhA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (51, 1, N'eddsdbo.LoadBISSummary.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''LoadBISSummary'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Ron@Milyli
-- Create date: 2012-06-01
-- Description:	
-- 2013-02-18 : Ron@Milyli : Added new TotalNRQCount field
-- 2013-02-18 : Ron@Milyli : Added the field encryption code
-- 2013-06-05 : David@Milyli.com : added support for a timezone offset
-- 2013-08-01 : Ron@Milyli : Added DateAdd(DAY, DateDiff(DAY, 0, Cast(MeasureDate As datetime)), Cast(cast(MeasureHour as varchar) + '''':00:00.000'''' as datetime))
-- 2013-08-19 : Ron@Milyli : Added CAST( AS DATE) around the above calculation
-- 2013-09-10 : Ron@Milyli : Added in DocumentCount loader (this was missed in previous merge -- code orignally written 06/2013
-- =============================================
ALTER PROCEDURE  [eddsdbo].[LoadBISSummary]
	@ProcessExecDate DateTime, 
	@TimeZoneOffset int
 
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @pwd NVarchar(100)
	SET @pwd = ''kCuraPassword1!''

	set @TimeZoneOffset  = DATEDIFF(MINUTE, GETUTCDATE(), GETDATE()) * -1
	
	Declare @SummaryMeasureDate DateTime
	Declare @MeasureDate Date
	
	--Select @MeasureDate = Cast(@ProcessExecDate as Date)
	Select @MeasureDate = DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, @ProcessExecDate)) ,0)
	Select @SummaryMeasureDate = @MeasureDate

	--BISSummary data is already converted to local time
	IF EXISTS(Select 1 From eddsdbo.BISSummary Where MeasureDate = @SummaryMeasureDate)
	BEGIN 
	  --Existing data for the date, run an update
	  BEGIN TRAN
		UPDATE BISSummary
		SET TQCount = COALESCE(LC.TQCount,0),
			TotalNRQCount = COALESCE(LC.TotalNRQCount, 0),
			NRLRQCount = COALESCE(LC.NRLRQCount,0),
			DocumentCount = COALESCE(CS.DocumentCount,0)
		FROM
			eddsdbo.BISSummary BS
			
			LEFT JOIN
             (SELECT CaseArtifactID, DocumentCount
                FROM EDDS.eddsdbo.CaseStatistics
                WHERE (CAST(DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, timestamp)), 0) AS DATE) = @MeasureDate)) AS CS 
                ON BS.CaseArtifactID = CS.CaseArtifactID 
				
			Left Join (Select CaseArtifactID, SUM(TotalQCount) TQCount, SUM(TotalNRQCount) TotalNRQCount, SUM(LRQCount) LRQCount, SUM(NRLRQCount) NRLRQCount   
			From eddsdbo.LRQCountDW where cast(DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, DateAdd(DAY, DateDiff(DAY, 0, Cast(MeasureDate As datetime)), Cast(cast(MeasureHour as varchar) + '':00:00.000'' as datetime)))) ,0) as DATE) = @MeasureDate Group By CaseArtifactID) LC
			On BS.CaseArtifactID = LC.CaseArtifactID 
			AND BS.MeasureDate = @MeasureDate	
			
	  COMMIT TRAN 

	END

	ELSE

	BEGIN
	    --New day, so do an initial insert
		BEGIN TRAN
 			INSERT BISSummary (CreatedOn, CaseArtifactID, MeasureDate, TQCount, TotalNRQCount, NRLRQCount, StatusDay, StatusPercentageNRLRQDay, DocumentCount)
			SELECT 
				GetUTCDate() CreatedOn, 
				C.ArtifactID CaseArtifactID, 	
				@SummaryMeasureDate MeasureDate, 
				COALESCE(LC.TQCount,0) TQCount, 
			    COALESCE(LC.TotalNRQCount, 0) TotalNRQCount,
				COALESCE(LC.NRLRQCount,0) NRLRQCount, 
				N''-1'' AS StatusDay,
				-1  StatusPercentageNRLRQDay,
				COALESCE(CS.DocumentCount,0) DocumentCount
			From
			  EDDS.eddsdbo.[Case] C
			  
			  LEFT JOIN
                  (SELECT DISTINCT CaseArtifactID, DocumentCount
                    FROM EDDS.eddsdbo.CaseStatistics
                    WHERE cast(DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, timestamp)), 0) as date)  = @MeasureDate) AS CS 
                    ON C.ArtifactID = CS.CaseArtifactID 
					
			Left Join (Select CaseArtifactID, SUM(TotalQCount) TQCount, SUM(TotalNRQCount) TotalNRQCount, SUM(LRQCount) LRQCount, SUM(NRLRQCount) NRLRQCount   
			From eddsdbo.LRQCountDW 
			Where cast(DATEADD(HOUR, DATEDIFF(HH, 0, DATEADD(MINUTE, @TimeZoneOffset, DateAdd(DAY, DateDiff(DAY, 0, Cast(MeasureDate As datetime)), Cast(cast(MeasureHour as varchar) + '':00:00.000'' as datetime)))) ,0) as DATE) = @MeasureDate Group By CaseArtifactID) LC
			On C.ArtifactID = LC.CaseArtifactID

		COMMIT TRAN
	END
END


', N'Tvpy59zdY6QPFCXTa2RN5g==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (52, 1, N'eddsdbo.LoadErrorHealthDWData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''LoadErrorHealthDWData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

ALTER PROCEDURE  [eddsdbo].[LoadErrorHealthDWData] 
	@ProcessExecDate DateTime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @MeasureDate Date
	Declare @MeasureHour Int
	Declare @Frequency int
	Select @MeasureDate = Cast(@ProcessExecDate as Date) , @MeasureHour = DatePart(HH,@ProcessExecDate) 
	Select @Frequency= COALESCE(Frequency,0) From eddsdbo.Measure Where MeasureID = 2

	IF @Frequency > 0 
	BEGIN
		--this will give us the error count for the past five minutes for each workspace
		--we''ll later need to update this script to filter for only "kickout" errors
		Insert eddsdbo.ErrorCountDW (MeasureDate, MeasureHour, CaseArtifactID, ErrorCount, CreatedOn)
		SELECT    @MeasureDate MeasureDate, @MeasureHour MeasureHour,  C.ArtifactID AS CaseArtifactID, COALESCE(EC.ErrorCount,0) ErrorCount, GetUTCDate() as [CreatedOn]
		FROM         EDDS.eddsdbo.[Case] C
		Left Join (SELECT
			CaseArtifactID,
			COUNT(ArtifactID) as ErrorCount
		FROM
			EDDS.eddsdbo.ExtendedError (NOLOCK)
		WHERE
			CreatedOn >= @ProcessExecDate AND CreatedOn <= DateAdd(Minute, @Frequency, @ProcessExecDate)
		 AND ((FullError Like ''%Read Failed%''   
			OR FullError LIKE ''%Delete Failed%''
			OR FullError LIKE ''%Create Failed%''
			OR FullError LIKE ''%Update Failed%''
			OR FullError LIKE ''%object reference not set to an instance of an object%''
			OR FullError LIKE ''%SQL Statement Failed%''
			OR FullError LIKE ''%Unable to connect to the remote server%'')
			AND Source <> ''Native Document Viewer'')
		 AND
			CaseArtifactID IS NOT NULL
		GROUP BY
			CaseArtifactID) EC
			On C.ArtifactID = EC.CaseArtifactID		
	END		
END

', N'H6IcZfr4lec3ZFA4gHzMEQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (53, 1, N'eddsdbo.LoadLatencyHealthDWData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''LoadLatencyHealthDWData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

ALTER PROCEDURE  [eddsdbo].[LoadLatencyHealthDWData] 
	@ProcessExecDate DateTime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @MeasureDate Date
	Declare @MeasureHour Int
	Declare @Frequency int
	Select @MeasureDate = Cast(@ProcessExecDate as Date) , @MeasureHour = DatePart(HH,@ProcessExecDate) 
	Select @Frequency= COALESCE(Frequency,0) From eddsdbo.Measure Where MeasureID = 3

	IF @Frequency > 0 
	BEGIN
		--this will give us the avg latency for the past five minutes for each workspace
		--relativity adds a row to this table every 15min for each logged in user

		Insert eddsdbo.LatencyDW (MeasureDate, MeasureHour, CaseArtifactID, AverageLatency, CreatedOn)
		SELECT  @MeasureDate MeasureDate, @MeasureHour MeasureHour,  C.ArtifactID AS CaseArtifactID, COALESCE(AL.AvgLatency,0) AvgLatency, GetUTCDate() as [CreatedOn]
		FROM         EDDS.eddsdbo.[Case] C
		Left Join (SELECT
			CaseArtifactID,
			AVG(Latency) as AvgLatency
		FROM
			EDDS.eddsdbo.WebClientPerformance (NOLOCK)
		WHERE
			StartTime >= @ProcessExecDate AND StartTime <= DateAdd(Minute, @Frequency, @ProcessExecDate)
		GROUP BY
			CaseArtifactID) AL
			 On C.ArtifactID = AL.CaseArtifactID
	END				
END

', N'BLY5GKMN2cAdNNpDCvqixA==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (54, 1, N'eddsdbo.LoadUserHealthDWData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''LoadUserHealthDWData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Murali Shesham
-- Create date: 08/31/2011
-- Description:	
-- =============================================
ALTER PROCEDURE  [eddsdbo].[LoadUserHealthDWData] 
	@ProcessExecDate DateTime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @MeasureDate Date
	Declare @MeasureHour Int
	Declare @Frequency int
	Select @MeasureDate = Cast(@ProcessExecDate as Date) , @MeasureHour = DatePart(HH,@ProcessExecDate) 
	Select @Frequency= COALESCE(Frequency,0) From eddsdbo.Measure Where MeasureID = 4
 
	IF @Frequency > 0 
	BEGIN
		Insert eddsdbo.UserCountDW (MeasureDate, MeasureHour, CaseArtifactID, UserCount, CreatedOn)
		SELECT  @MeasureDate MeasureDate, @MeasureHour MeasureHour,   C.ArtifactID AS CaseArtifactID, COALESCE(UC.UserCount,0) UserCount, GETUTCDATE() as [CreatedOn]
		FROM         EDDS.eddsdbo.[Case] C
		Left Join (SELECT
			CaseArtifactID,
			COUNT(UserID) as UserCount	
		FROM
			EDDS.eddsdbo.UserStatus (NOLOCK)
		WHERE
			CaseArtifactID <> -1
			-- value of -1 indicates they''re not currently in any workspace
		GROUP BY
			CaseArtifactID) UC
			On C.ArtifactID = UC.CaseArtifactID
	END
END

', N'QJ8wPaKJyoytIGJ9zxvMNQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (55, 1, N'eddsdbo.MergeServerInformation.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''MergeServerInformation'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

ALTER PROCEDURE   [eddsdbo].[MergeServerInformation] 
(
	@XMLServerList XML = ''''
)
 
AS  
BEGIN

	DECLARE @Server TABLE( ServerName nvarchar(255), ServerIPAddress nvarchar(100), ServerTypeID INT)

	INSERT INTO @Server 
			--VALUES( ServerName, ServerIPAddress, ServerTypeID )
			SELECT DISTINCT
				item.value(''@Name'',''nvarchar(100)'') AS ServerName
				, item.value(''@IP'',''nvarchar(100)'') AS ServerIPAddress
				, item.value(''@TypeID'',''INT'') AS ServerTypeID
			FROM 
				@XMLServerList.nodes(''/ServerList/Server'') d(item)


	UPDATE [eddsdbo].[Server] SET DeletedOn = NULL 

	UPDATE [eddsdbo].[Server] set deletedon = GETUTCDATE()
	WHERE serverid not in(
	SELECT serverid FROM [eddsdbo].[Server]
	WHERE ServerName + ''_'' + ServerIPAddress + ''_'' + CAST(ServerTypeID as VARCHAR)
	IN (SELECT ServerName + ''_'' + ServerIPAddress + ''_'' + CAST(ServerTypeID as VARCHAR) from @server))

	INSERT INTO [eddsdbo].[Server] (ServerName, CreatedOn, DeletedOn, ServerTypeID, ServerIPAddress) 
	select ServerName , GETUTCDATE(), null, ServerTypeID , ServerIPAddress from @Server
	where ServerName + ''_'' + ServerIPAddress + ''_'' + CAST(ServerTypeID as VARCHAR)
	NOT IN (select ServerName + ''_'' + ServerIPAddress + ''_'' + CAST(ServerTypeID as VARCHAR) from [eddsdbo].[Server])
	
	--Update Unrecognized servers, in case they are now recognized.
	UPDATE S
	SET S.ServerTypeID = X.ServerTypeID
	FROM [eddsdbo].[Server] S INNER JOIN @Server X ON (S.ServerName = X.ServerName) AND (S.ServerIPAddress = X.ServerIPAddress)
	WHERE S.ServerTypeID = 99 

	
END

', N'PbppHidWkCSgPPMgBxAyfQ==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (56, 1, N'eddsdbo.PopulateFactTableData.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''PopulateFactTableData'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Jayesh Dhobi
-- Create date: 13th Sep 2011
-- Description:	Populate Fact Table data
-- Modified By: Justin Jarczyk 2/4/2014, to account
-- for the addition of drive letters into the 
-- ServerDiskSummary table.
-- =============================================  
ALTER PROCEDURE  [eddsdbo].[PopulateFactTableData]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN --- Declaration
		DECLARE @IsRunsuccessfully BIT = 0
		
		DECLARE @MeasureDate datetime = GETUTCDATE()
		DECLARE @MeasureHour datetime = DATEADD(hour, DATEDIFF(hour, 0, @MeasureDate), 0)	-- truncated to the last hour

	END
	PRINT '' Start ''
	BEGIN TRY
		BEGIN TRAN
		
		BEGIN --- Populate ServerSummary table
			MERGE [eddsdbo].[ServerSummary]
			USING
			(
				SELECT
					  ServerID
					, @MeasureHour MeasureDate
					, AVG(RAMPagesPerSec) AS RAMPagesPerSec
					, AVG(RAMPageFaultsPerSec) AS RAMPageFaultsPerSec
				FROM [eddsdbo].[ServerDW]
				WHERE CreatedOn BETWEEN DATEADD(HH, -1, @MeasureDate) AND @MeasureDate
				GROUP BY ServerID
			) AS Data ON Data.ServerID = [ServerSummary].ServerID
				AND Data.MeasureDate = [ServerSummary].MeasureDate
			WHEN MATCHED THEN UPDATE
                SET RAMPagesPerSec = Data.RAMPagesPerSec
				, RAMPageFaultsPerSec = Data.RAMPageFaultsPerSec
			WHEN NOT MATCHED THEN
				INSERT (
                            ServerID
							, MeasureDate
							, RAMPagesPerSec
							, RAMPageFaultsPerSec
							, CreatedOn
                        )
                VALUES ( 
                            Data.ServerID
							, Data.MeasureDate
							, Data.RAMPagesPerSec
							, Data.RAMPageFaultsPerSec
							, @MeasureDate
                        );
		END
		
		BEGIN --- Populate ServerDiskSummary table
			MERGE [eddsdbo].[ServerDiskSummary]
			USING
			(	
				SELECT
					  ServerID
					, DiskNumber
					, @MeasureHour MeasureDate
					, AVG( DiskAvgSecPerRead ) AS DiskAvgSecPerRead
					, AVG( DiskAvgSecPerWrite ) AS DiskAvgSecPerWrite
					, MIN(DriveLetter) AS DriveLetter
				FROM [eddsdbo].[ServerDiskDW]
				WHERE CreatedOn BETWEEN DATEADD(HH, -1, @MeasureDate) AND @MeasureDate
				GROUP BY ServerID, DiskNumber
			) AS Data ON Data.ServerID = [ServerDiskSummary].ServerID
				AND Data.MeasureDate = [ServerDiskSummary].MeasureDate
				AND Data.DiskNumber = [ServerDiskSummary].DiskNumber
			WHEN MATCHED THEN UPDATE 
                SET DiskAvgSecPerRead = Data.DiskAvgSecPerRead
					, DiskAvgSecPerWrite = Data.DiskAvgSecPerWrite
			WHEN NOT MATCHED THEN
				INSERT
				(
					ServerID
					, MeasureDate
					, DiskNumber
					, DiskAvgSecPerRead
					, DiskAvgSecPerWrite
					, CreatedOn
					, DriveLetter 
				)
				VALUES 
				(
					Data.ServerID
					, Data.MeasureDate
					, Data.DiskNumber
					, Data.DiskAvgSecPerRead
					, Data.DiskAvgSecPerWrite
					, @MeasureDate
					, Data.DriveLetter
				);
		END		
		
		BEGIN --- Populate ServerProcessorSummary table
			MERGE [eddsdbo].[ServerProcessorSummary]
			USING
			(	
				SELECT
					  ServerID
					, CoreNumber
					, @MeasureHour MeasureDate
					, AVG( CPUProcessorTimePct ) AS CPUProcessorTimePct
				FROM [eddsdbo].[ServerProcessorDW]
				WHERE CreatedOn BETWEEN DATEADD(HH, -1, @MeasureDate) AND @MeasureDate
				GROUP BY ServerID, CoreNumber
			) AS Data ON Data.ServerID = [ServerProcessorSummary].ServerID
				AND Data.MeasureDate = [ServerProcessorSummary].MeasureDate
				AND Data.CoreNumber = [ServerProcessorSummary].CoreNumber
			
			WHEN MATCHED THEN UPDATE 
                SET CPUProcessorTimePct = Data.CPUProcessorTimePct
			
			WHEN NOT MATCHED THEN
				INSERT
				(
					ServerID
					, MeasureDate
					, CoreNumber
					, CPUProcessorTimePct
					, CreatedOn
				)
				VALUES 
				(
					Data.ServerID
					, Data.MeasureDate
					, Data.CoreNumber
					, Data.CPUProcessorTimePct
					, @MeasureDate
				);
		END
		
		BEGIN --- Populate SQLServerSummary table
			MERGE [eddsdbo].[SQLServerSummary]
			USING
			(	
				SELECT
					ServerID
					, @MeasureHour MeasureDate
					, AVG( SQLPageLifeExpectancy ) AS SQLPageLifeExpectancy
				FROM [eddsdbo].[SQLServerDW]
				WHERE( CreatedOn BETWEEN DATEADD(HH, -1, @MeasureDate) AND @MeasureDate )
				GROUP BY ServerID
			) AS Data ON Data.ServerID = [SQLServerSummary].ServerID
				AND Data.MeasureDate = [SQLServerSummary].MeasureDate
			WHEN MATCHED THEN UPDATE 
				SET SQLPageLifeExpectancy = Data.SQLPageLifeExpectancy
			WHEN NOT MATCHED THEN
				INSERT
				(
					ServerID
					, MeasureDate
					, SQLPageLifeExpectancy
					, CreatedOn
				)
				VALUES 
				(
					Data.ServerID
					, Data.MeasureDate
					, Data.SQLPageLifeExpectancy
					, @MeasureDate
				);
		END
			
		COMMIT TRAN
		
		SET @IsRunsuccessfully = 1
		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
		
	IF (@IsRunsuccessfully = 1)
		BEGIN
			SELECT @IsRunsuccessfully AS IsRunsuccessfully, ''''	AS ErrorMessage
		END
	ELSE
		BEGIN
			SELECT @IsRunsuccessfully AS IsRunsuccessfully, ERROR_MESSAGE()	AS ErrorMessage
		END
END

', N'f4wTDBgoB21l0jcOL2mULg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (57, 1, N'eddsdbo.UpdateBISScores.sql', N'USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = ''UpdateBISScores'', @Type = ''PROCEDURE'', @Schema = ''eddsdbo''

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + ''.'' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = ''CREATE '' + @Type + '' '' + @Schema + ''.'' + @Name + '' AS SELECT * FROM sys.objects''
  EXECUTE(@SQL)
END 
PRINT ''Updating '' + @Type + '' '' + @Schema + ''.'' + @Name
GO

-- =============================================
-- Author:		Ron@Milyli
-- Create date: 2012-06-01
-- Description:	Populate BIS Values
-- 2013-02-18 : Ron@Milyli - Modified Percentage to use TotalNRQCount as denominator
-- 2013-02-18 : Ron@Milyli : Added the field encryption code
-- 2013-02-18 : Ron@Milyli : Updated to Josh''s most up to date code
-- 2013-04-19 : Ron@Milyli : Added ISNULL check around BS.StatusPercentageNRLRQDay
-- 2013-04-19 : Ron@Milyli : Added limiter to NinetyDayCursor.
-- 2013-04-23 : Ron@Milyli : Removed 90 day running average code. Does not appear to be displayed in the GUI.
-- 2013-06-06 : Ron@Milyli : Added additional logic to take into account Document Counts
-- 2013-09-17 : Ron@Milyli : Changed NRLRQCount to TotalNRQCount (>=50) per Ryan   
-- =============================================
ALTER PROCEDURE  [eddsdbo].[UpdateBISScores]
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @pwd NVarchar(100)
	SET @pwd = ''kCuraPassword1!''

	--=============================================================================================
	
	--Determine the percentage of NRLRQ per day and update the field
	UPDATE eddsdbo.BISSummary 
	SET StatusPercentageNRLRQDay = CONVERT(integer, (ISNULL((CONVERT(decimal, NRLRQCount) / NULLIF(CONVERT(decimal, TotalNRQCount),0)),0)) * 100)	

	--=============================================================================================

	--Determine the Status per day and update the field
	UPDATE eddsdbo.BISSummary
	SET StatusDay = 
		EncryptByPassPhrase(
			@pwd,
			CONVERT(nvarchar(max),
			CASE
				--Fail/Poor
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) <= 1000000) AND (StatusPercentageNRLRQDay >= 15) then 3 --Fail/Poor
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 1000001 AND 3000000) AND (StatusPercentageNRLRQDay >= 22.5) then 3 --Fail/Poor
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 3000001 AND 5000000) AND (StatusPercentageNRLRQDay >= 32.5) then 3 --Fail/Poor
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) >= 5000001) AND (StatusPercentageNRLRQDay >= 40) then 3 --Fail/Poor
				--Probation/Moderate
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) <= 1000000) AND (StatusPercentageNRLRQDay >= 10) then 2 --Probation/Moderate
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 1000001 AND 3000000) AND (StatusPercentageNRLRQDay >= 17.5) then 2 --Probation/Moderate 
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) BETWEEN 3000001 AND 5000000) AND (StatusPercentageNRLRQDay >= 27.5) then 2 --Probation/Moderate
				WHEN (TotalNRQCount >= 50) AND (COALESCE(DocumentCount,0) >= 5000001) AND (StatusPercentageNRLRQDay >= 35) then 2 --Probation/Moderate
				--Passed
				ELSE 1 --Passed
			END),
			1,
			CONVERT( varbinary, MeasureDate)
		)

	--=============================================================================================
	
END

PRINT N''Droping function [eddsdbo].[GETLRQHealthQry]''


', N'XGBUrAMSQTUhdAT/XiJ8/Q==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
INSERT [eddsdbo].[RHScriptsRun] ([id], [version_id], [script_name], [text_of_script], [text_hash], [one_time_script], [entry_date], [modified_date], [entered_by]) VALUES (59, 1, N'000001_eddsdbo.Indexes.sql', N'USE [EDDSPerformance]
GO


IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_CaseArtifactID'')
    CREATE NONCLUSTERED INDEX [IX_CaseArtifactID] ON [eddsdbo].[PerformanceSummary]
	(
      [CaseArtifactID] ASC
	)
	INCLUDE ( [MeasureDate],
	[UserCount],
	[ErrorCount],
	[LRQCount],
	[AverageLatency]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY];
GO
PRINT N''ADD [kie_measureDate] To [LRQCountDW]''
GO
/****** Object:  Index [kie_measureDate]    Script Date: 10/04/2013 11:09:44 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[eddsdbo].[LRQCountDW]'') AND name = N''kIE_MeasureDate'')
DROP INDEX [kie_measureDate] ON [eddsdbo].[LRQCountDW] WITH ( ONLINE = OFF )
GO
/****** Object:  Index [kie_measureDate]    Script Date: 10/04/2013 11:09:44 ******/
CREATE NONCLUSTERED INDEX [kIE_MeasureDate] ON [eddsdbo].[LRQCountDW] 
(
	[MeasureDate] ASC
)
INCLUDE ( [NRLRQCount],
[TotalQCount],
[TotalNRQCount],
[CaseArtifactID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
PRINT N''ADD [kie_measureDate] To [BISSummary]''
GO
/****** Object:  Index [KIE_measuredate]    Script Date: 10/04/2013 11:09:18 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[eddsdbo].[BISSummary]'') AND name = N''kIE_MeasureDate'')
DROP INDEX [kIE_MeasureDate] ON [eddsdbo].[BISSummary] WITH ( ONLINE = OFF )
GO
/****** Object:  Index [KIE_measuredate]    Script Date: 10/04/2013 11:09:18 ******/
CREATE NONCLUSTERED INDEX [kIE_MeasureDate] ON [eddsdbo].[BISSummary] 
(
	[MeasureDate] ASC
)
INCLUDE ( [TQCount],
[TotalNRQCount],
[NRLRQCount],
[DocumentCount]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]', N'ajT1wxENENisBLykW4tGzg==', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, NULL)
SET IDENTITY_INSERT [eddsdbo].[RHScriptsRun] OFF
