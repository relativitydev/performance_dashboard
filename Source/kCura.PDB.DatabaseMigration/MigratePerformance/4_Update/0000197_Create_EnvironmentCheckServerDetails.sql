USE [EDDSPerformance]

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'EnvironmentCheckServerDetails') 
BEGIN
	CREATE TABLE [eddsdbo].[EnvironmentCheckServerDetails](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[ServerName] [varchar](150) NULL,
		[OSVersion] [varchar](150) NULL,
		[OSName] [varchar](150) NULL,
		[LocalProcessors] [int] NULL,
		[Hyperthreaded] [bit] NULL,
	 CONSTRAINT [PK_EnvironmentCheckServerDetails] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'EnvironmentCheckDatabaseDetails') 
BEGIN
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
END




