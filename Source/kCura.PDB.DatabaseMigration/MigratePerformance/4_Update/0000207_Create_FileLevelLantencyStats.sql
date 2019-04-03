USE [EDDSPerformance]
GO

IF NOT EXISTS (select 1 from sysobjects where [name] = 'FileLevelLatencyDetails' and type = 'U')  
BEGIN
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

end

