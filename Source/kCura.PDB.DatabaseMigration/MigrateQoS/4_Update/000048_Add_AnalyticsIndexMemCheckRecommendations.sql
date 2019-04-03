USE [EDDSQoS]
GO

--Analytics Server memory check

Delete from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
where ID in ('46BC6D63-7054-4ECB-8DA2-7011DA7AD86C'
			,'86B83F5F-0A95-4BAF-ADCE-0F5988A8CC0F'
			,'1E373E4C-980C-457C-AC64-99C1F7390809'
			,'9643992C-90A2-4E36-93DF-5484ABFF7940')

INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
VALUES
    ('46BC6D63-7054-4ECB-8DA2-7011DA7AD86C',
	'analytics server',
	50,
	'Warning',
	'Analytics',
	'Searchable Documents Per 6GB Memory',
	'Searchable Documents Per 6GB Memory',
	'Update the memory on analytics server',
	'N',
	'')

INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
VALUES
    ('86B83F5F-0A95-4BAF-ADCE-0F5988A8CC0F',
	'analytics server',
	0,
	'Good',
	'Analytics',
	'Searchable Documents Per 6GB Memory',
	'Searchable Documents Per 6GB Memory',
	'No change needed.',
	'N',
	'')

INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
VALUES
    ('1E373E4C-980C-457C-AC64-99C1F7390809',
	'analytics server',
	50,
	'Warning',
	'Analytics',
	'Training Documents Per 6GB Memory',
	'Training Documents Per 6GB Memory',
	'Update the memory on analytics server',
	'N',
	'')

INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
VALUES
    ('9643992C-90A2-4E36-93DF-5484ABFF7940',
	'analytics server',
	0,
	'Good',
	'Analytics',
	'Training Documents Per 6GB Memory',
	'Training Documents Per 6GB Memory',
	'No change needed.',
	'N',
	'')




