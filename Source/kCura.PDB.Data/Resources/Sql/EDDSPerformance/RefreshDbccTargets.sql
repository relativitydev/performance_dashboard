--EDDSPerformance

DELETE t
FROM eddsdbo.DBCCTarget t
WHERE NOT EXISTS (SELECT TOP 1 1 FROM sys.servers WHERE name = t.ServerName COLLATE DATABASE_DEFAULT)

INSERT INTO eddsdbo.DBCCTarget (ServerName, DatabaseName, Active)
SELECT name, 'EDDSQoS', 0
FROM sys.servers s
WHERE NOT EXISTS (SELECT TOP 1 1 FROM eddsdbo.DBCCTarget WHERE ServerName = s.name COLLATE DATABASE_DEFAULT)