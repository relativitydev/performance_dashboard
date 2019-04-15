USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[MeasureType]    Script Date: 03/14/2014 10:51:07 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MeasureType' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[MeasureType](
		[MeasureTypeId] [int] NOT NULL,
		[MeasureTypeCd] [varchar](50) NOT NULL,
		[MeasureTypeDesc] [varchar](500) NOT NULL,
		[IsActive] [bit] NOT NULL,
		[IsDeleted] [bit] NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[UpdatedOn] [datetime] NULL,
	 CONSTRAINT [PK_MeasureType] PRIMARY KEY CLUSTERED 
	(
		[MeasureTypeId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO