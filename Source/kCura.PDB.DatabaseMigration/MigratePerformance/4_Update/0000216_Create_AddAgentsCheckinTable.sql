USE [EDDSPerformance]
GO

IF NOT EXISTS (select 1 from sysobjects where [name] = 'AgentHistory' and type = 'U')  
BEGIN
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

end

