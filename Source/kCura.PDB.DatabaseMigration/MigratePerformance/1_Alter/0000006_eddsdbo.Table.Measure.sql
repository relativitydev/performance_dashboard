USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[Measure]    Script Date: 03/14/2014 10:50:44 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Measure' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[Measure](
		[MeasureID] [int] IDENTITY(1,1) NOT NULL,
		[MeasureCd] [varchar](100) NOT NULL,
		[MeasureDesc] [varchar](500) NOT NULL,
		[MeasureTypeId] [int] NOT NULL,
		[IsActive] [bit] NOT NULL,
		[IsDeleted] [bit] NOT NULL,
		[CreatedOn] [datetime] NULL,
		[UpdatedOn] [datetime] NULL,
		[Frequency] [int] NOT NULL,
		[LastDataLoadDateTime] [datetime] NULL,
	 CONSTRAINT [PK_Measure] PRIMARY KEY CLUSTERED 
	(
		[MeasureID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO