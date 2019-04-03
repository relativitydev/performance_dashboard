USE EDDSPerformance
GO

IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ServerDiskSummary' AND COLUMN_NAME = 'DiskAvgSecPerRead')
BEGIN
	EXEC sp_rename 'eddsdbo.ServerDiskSummary.DiskAvgSecPerRead', 'DiskAvgReadsPerSec', 'COLUMN'
END

GO

IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ServerDiskSummary' AND COLUMN_NAME = 'DiskAvgSecPerWrite')
BEGIN
	EXEC sp_rename 'eddsdbo.ServerDiskSummary.DiskAvgSecPerWrite', 'DiskAvgWritesPerSec', 'COLUMN'
END

GO

IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ServerDiskSummary' AND COLUMN_NAME = 'DiskSecPerRead')
BEGIN
	ALTER TABLE eddsdbo.ServerDiskSummary
	ADD DiskSecPerRead DECIMAL(10, 3) NULL
END

GO

IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ServerDiskSummary' AND COLUMN_NAME = 'DiskSecPerWrite')
BEGIN
	ALTER TABLE eddsdbo.ServerDiskSummary
	ADD DiskSecPerWrite DECIMAL(10, 3) NULL
END