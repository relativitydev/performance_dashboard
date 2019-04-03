IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHScriptsRun')
BEGIN
	CREATE TABLE [eddsdbo].[RHScriptsRun](
		[id] [bigint] IDENTITY(1,1) NOT NULL,
		[version_id] [bigint] NULL,
		[script_name] [nvarchar](255) NULL,
		[text_of_script] [text] NULL,
		[text_hash] [nvarchar](512) NULL,
		[one_time_script] [bit] NULL,
		[entry_date] [datetime] NULL,
		[modified_date] [datetime] NULL,
		[entered_by] [nvarchar](50) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHScriptsRunErrors')
BEGIN
	CREATE TABLE [eddsdbo].[RHScriptsRunErrors](
		[id] [bigint] IDENTITY(1,1) NOT NULL,
		[repository_path] [nvarchar](255) NULL,
		[version] [nvarchar](50) NULL,
		[script_name] [nvarchar](255) NULL,
		[text_of_script] [ntext] NULL,
		[erroneous_part_of_script] [ntext] NULL,
		[error_message] [ntext] NULL,
		[entry_date] [datetime] NULL,
		[modified_date] [datetime] NULL,
		[entered_by] [nvarchar](50) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHVersion')
BEGIN
	CREATE TABLE [eddsdbo].[RHVersion](
		[id] [bigint] IDENTITY(1,1) NOT NULL,
		[repository_path] [nvarchar](255) NULL,
		[version] [nvarchar](50) NULL,
		[entry_date] [datetime] NULL,
		[modified_date] [datetime] NULL,
		[entered_by] [nvarchar](50) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END