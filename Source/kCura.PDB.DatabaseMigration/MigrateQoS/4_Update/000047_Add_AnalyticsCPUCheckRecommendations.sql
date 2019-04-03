USE [EDDSQoS]
GO

--Analytics Server CPU/Connectors check

Delete from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
where ID in ('ab10edae-67f9-4636-a4c6-94d7ef20d705','d0b7cf77-9ec7-4510-9fd0-be594522930e')

INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
VALUES
    ('ab10edae-67f9-4636-a4c6-94d7ef20d705',
	'Analytics',
	50,
	'Warning',
	'Relativity.Core',
	'ContentAnalystMaxConnectorsPerIndexDefault',
	'ContentAnalystMaxConnectorsPerIndexDefault',
	'Update the cpu on analytics server',
	'N',
	'')

INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
VALUES
    ('d0b7cf77-9ec7-4510-9fd0-be594522930e',
	'Analytics',
	0,
	'Good',
	'Relativity.Core',
	'ContentAnalystMaxConnectorsPerIndexDefault',
	'ContentAnalystMaxConnectorsPerIndexDefault',
	'No change needed.',
	'N',
	'')






