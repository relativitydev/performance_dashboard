

UPDATE [eddsdbo].[AgentHistory]
   SET [AgentArtifactId] = @agentArtifactId,
	   [TimeStamp] = @timeStamp,
       [Successful] = @successful
 WHERE ID = @id