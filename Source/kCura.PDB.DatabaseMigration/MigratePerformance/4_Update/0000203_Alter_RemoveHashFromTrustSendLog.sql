USE [EDDSPerformance]
GO

IF COL_LENGTH ('eddsdbo.TrustSendLog' ,'DataHash') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.TrustSendLog
    DROP COLUMN DataHash
END
