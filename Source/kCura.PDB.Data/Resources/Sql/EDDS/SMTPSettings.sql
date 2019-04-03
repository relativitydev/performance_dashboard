DECLARE @server nvarchar(max),
	@port nvarchar(max),
	@from nvarchar(max),
	@to nvarchar(max),
	@username nvarchar(max),
	@password nvarchar(max),
	@encryptedPassword nvarchar(max),
	@SMTPSSLisRequired nvarchar(max);
	
SET @server = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'SMTPServer'
);

SET @port = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'SMTPPort'
);

SET @from = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'EmailFrom'
);

SET @to = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'EmailTo'
);

SET @username = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'SMTPUserName'
);

SET @password = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'SMTPPassword'
);

SET @encryptedPassword = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'EncryptedSMTPPassword'
);

SET @SMTPSSLisRequired = (
	SELECT TOP 1 [Value]
	FROM [EDDS].[eddsdbo].[Configuration] WITH(NOLOCK)
	WHERE Section = 'kCura.Notification'
	AND Name = 'SMTPSSLisRequired'
);

SELECT @server [SMTPServer],
	@port SMTPPort,
	@from EmailFrom,
	@to EmailTo,
	@username SMTPUsername,
	@password SMTPPassword,
	@encryptedPassword EncryptedSMTPPassword,
	@SMTPSSLisRequired SMTPSSLisRequired