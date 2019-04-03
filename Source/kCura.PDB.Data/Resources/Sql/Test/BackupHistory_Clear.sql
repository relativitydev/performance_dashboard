-- EDDSPerformance

TRUNCATE TABLE eddsdbo.QoS_BackupHistory

-- Clear last backup time
UPDATE eddsdbo.[Server]
SET LastServerBackup = NULL
WHERE ServerTypeID = 3