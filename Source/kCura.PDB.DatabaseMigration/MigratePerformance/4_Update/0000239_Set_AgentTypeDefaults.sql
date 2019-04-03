use edds

update eddsdbo.AgentType
set DefaultInterval = 5
,DefaultLogging = 5
,MinInstanceEnvironment = 1
,MaxInstanceEnvironment = 1
where [Guid] = '79F33C93-4FC3-4E92-8C75-9D6F4073F334' -- QoS Manager

update eddsdbo.AgentType
set DefaultInterval = 5
,DefaultLogging = 5
,MinInstanceEnvironment = 1
where [Guid] = 'D943F8E1-CB2F-40FD-BA66-3970D3AB17C0' -- QoS Worker

update eddsdbo.AgentType
set DefaultInterval = 5
,DefaultLogging = 5
,MinInstanceEnvironment = 1
where [Guid] = 'AA805282-5F1A-487E-AAC9-17E3E9A5B4BA' -- Wmi Worker

update eddsdbo.AgentType
set DefaultInterval = 3600
,DefaultLogging = 5
,MinInstanceEnvironment = 1
where [Guid] = '4E7DBF83-C74A-462F-BF9A-4B4360ADBB42' -- Trust Worker