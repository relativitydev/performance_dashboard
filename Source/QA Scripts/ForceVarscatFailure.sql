USE [EDDS1015575]
GO
INSERT INTO [EDDSDBO].[VarscatRunHistory]
(RunDateTimeUTC, WhoRanMe, IsActive) VALUES
(GETUTCDATE(), 'sa', 1)