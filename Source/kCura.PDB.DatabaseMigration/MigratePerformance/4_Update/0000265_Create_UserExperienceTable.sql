use [EddsPerformance]

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'UserExperience' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE [eddsdbo].[UserExperience](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[HourId] [int] NOT NULL,
		[ServerId] [int] NOT NULL,
		[ActiveUsers] [int] NOT NULL,
		[HasPoisonWaits] [bit] NOT NULL,
		[ArrivalRate] [decimal](12, 5) NOT NULL,
		[Concurrency] [decimal](12, 5) NOT NULL,
 CONSTRAINT [PK_UserExperience] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END