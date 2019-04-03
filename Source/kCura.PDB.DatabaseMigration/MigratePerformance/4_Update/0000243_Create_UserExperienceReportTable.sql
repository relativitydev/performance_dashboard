IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Reports_WorkspaceSearchAudits' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[Reports_WorkspaceSearchAudits](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[ServerId] [int] NOT NULL,
		[HourId] [int] NOT NULL,
		[SearchId] [int] NOT NULL,
		[SearchName] varchar(75) NULL,
		[WorkspaceId] [int] NOT NULL,
		[TotalExecutionTime] [bigint] NOT NULL,
		[TotalSearchAudits] [bigint] NOT NULL,
		[IsComplex] [bit] NOT NULL,
	 CONSTRAINT [PK_Reports_WorkspaceSearchAudits] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Reports_SearchAudits' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
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
END