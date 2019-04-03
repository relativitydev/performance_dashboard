USE [EDDSPerformance]


IF COL_LENGTH ('eddsdbo.Server' ,'IsQoSDeployed') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Server]
    ADD [IsQoSDeployed] bit not null DEFAULT 0
END