USE [EDDSQoS]
GO

--UseHashJoin
delete from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
where ID in (
'cde8a5f4-e0a2-4f18-a9a3-10c3f2844838',
'a2107ce5-fe0d-4231-8153-d445b4b7d7d1'
)

--remote query timeout(s)
delete from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
where ID in (
'bf16f5ec-60c2-4f14-b983-10c3bd40b7e6',
'5940e6f8-f93f-4c58-a081-77d8dca38da0',
'eb37e0f1-a6bb-4942-a0d9-6b8e923c0021',
'c4f963af-ca5d-4dcc-b5b7-b990ca30e3b1',
'5a8674f4-f7b1-460d-92f9-a0ab6826d6fc'
)





