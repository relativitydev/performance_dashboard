USE [EDDSPerformance]
GO

--IF NOT EXISTS (select 1 from sysobjects where [name] = 'PausedAgents' and type = 'U')  
--BEGIN
--	CREATE TABLE [eddsdbo].[PausedAgents](
--		[AgentArtifactId] [int] NOT NULL,
--		[ShouldPause] [bit] NOT NULL,
--		[IsPaused] [bit] NOT NULL,
--		[PausedOn] [datetime] NOT NULL,
--	 CONSTRAINT [PK_PausedAgents] PRIMARY KEY CLUSTERED 
--	(
--		[AgentArtifactId] ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--	) ON [PRIMARY]
--
--end

