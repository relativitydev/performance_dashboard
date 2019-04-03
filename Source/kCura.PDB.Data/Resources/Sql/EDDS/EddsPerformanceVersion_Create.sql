IF OBJECT_ID(N'eddsdbo.EddsPerformanceVersion', N'U') IS NULL
BEGIN 
CREATE TABLE [eddsdbo].[EddsPerformanceVersion](
	[Major] [int] NOT NULL,
	[Minor] [int] NOT NULL,
	[Build] [int] NOT NULL,
	[Revision] [int] NOT NULL,
 CONSTRAINT [PK_EddsPerformanceVersion] PRIMARY KEY CLUSTERED 
(
	[Major] ASC,
	[Minor] ASC,
	[Build] ASC,
	[Revision] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END;