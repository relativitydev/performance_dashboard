USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'TuningForkSysOutput' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[TuningForkSysOutput](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[ServerName] [varchar](150) NOT NULL,
		[Name] [varchar](35) NOT NULL,
		[Value] [int] NOT NULL,
		[Value_in_use] [int] NOT NULL,
		[Description] [varchar](255) NOT NULL,
		[Is_Dynamic] [bit] NOT NULL,
		[kIE_value] [int] NOT NULL,
		[kIE_Status] [varchar](25) NOT NULL,
		[kIE_Note] [varchar](max) NULL,
		CONSTRAINT [PK_TuningForkSysOutput] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO