

INSERT INTO [eddsdbo].[AgentHistory]
           ([AgentArtifactId]
           ,[TimeStamp]
           ,[Successful])
     VALUES
           (@agentArtifactId,
           @timeStamp,
           @successful)

SELECT * FROM [eddsdbo].[AgentHistory] WHERE ID = @@IDENTITY