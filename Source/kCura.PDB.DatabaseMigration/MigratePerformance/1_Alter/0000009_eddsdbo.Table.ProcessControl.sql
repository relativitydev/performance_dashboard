USE [EDDSPerformance]
GO

/****** Object:  Table [eddsdbo].[ProcessControl]    Script Date: 03/14/2014 10:51:53 ******/
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ProcessControl' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ProcessControl](
		[ProcessControlID] [int] NOT NULL,
		[ProcessTypeDesc] [nvarchar](200) NOT NULL,
		[LastProcessExecDateTime] [datetime] NOT NULL,
		[Frequency] [int] NULL,
	 CONSTRAINT [PK_ProcessControl] PRIMARY KEY CLUSTERED 
	(
		[ProcessControlID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO