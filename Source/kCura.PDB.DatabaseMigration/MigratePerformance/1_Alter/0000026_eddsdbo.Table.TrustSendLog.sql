USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'TrustSendLog' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [EDDSPerformance].[eddsdbo].[TrustSendLog]
	(
		[LogId] [int] IDENTITY(1,1) NOT NULL UNIQUE,
		[ScoreDate] DATETIME NOT NULL,
		[TransmissionDate] DATETIME NOT NULL
		CONSTRAINT DF_TransmissionDate DEFAULT GETUTCDATE(),
		[QualityData] [varchar](2048) NOT NULL,
		[DataHash] [varchar](128) NOT NULL,
		[Success] [bit] NOT NULL,
		[Output] [varchar](max) NULL
	)
END
GO