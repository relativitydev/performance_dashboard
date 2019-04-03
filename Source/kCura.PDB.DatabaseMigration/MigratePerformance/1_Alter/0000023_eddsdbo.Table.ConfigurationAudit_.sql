USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ConfigurationAudit' AND TABLE_SCHEMA = N'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ConfigurationAudit] (
		[Id] INT IDENTITY(1,1) NOT NULL,
		[FieldName] nvarchar(200) NOT NULL,
		[ServerName] nvarchar(150) NULL,
		[OldValue] nvarchar(max) NOT NULL,
		[NewValue] nvarchar(max) NOT NULL,
		[UserID] INT NOT NULL,
		[CreatedOn] datetime NOT NULL
		CONSTRAINT [PK_ConfigurationHistory] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)
	)
END
GO