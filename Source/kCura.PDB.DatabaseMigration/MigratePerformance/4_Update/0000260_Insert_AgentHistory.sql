USE [EDDSPerformance]

IF NOT EXISTS(SELECT TOP 1 * FROM eddsdbo.[AgentHistory])
BEGIN
INSERT INTO eddsdbo.[AgentHistory]
(AgentArtifactId, TimeStamp, Successful)
VALUES (0, GETUTCDATE(), 1)
END