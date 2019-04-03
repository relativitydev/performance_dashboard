USE [EDDSPerformance]
GO

IF NOT EXISTS (select 1 from sysobjects where [name] = 'SystemVersionHistory' and type = 'U')  
BEGIN
	CREATE TABLE [eddsdbo].[SystemVersionHistory](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[SummaryDayHour] [datetime] NOT NULL,
		[ServerName] [varchar](200) NOT NULL,
		[RowHash] [binary](20) NOT NULL,
		[RelativityVersion] [varchar](200) NOT NULL,
		[OSVersion] [nvarchar](256) NOT NULL,
		[OSServicePack] [nvarchar](256) NOT NULL,
		[SqlServerVersion] [nvarchar](128) NOT NULL,
		[SqlServerLevel] [nvarchar](128) NOT NULL,
	 CONSTRAINT [PK_SystemVersionHistory] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

end

