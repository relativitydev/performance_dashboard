USE [EDDSPerformance]

declare @sql nvarchar(max);

IF COL_LENGTH ('eddsdbo.HourSearchAuditBatches' ,'ID') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[HourSearchAuditBatches]
    ADD ID INT IDENTITY(1,1)
END

-- Add new column
IF COL_LENGTH ('eddsdbo.SearchAuditBatch' ,'HourSearchAuditBatchId') IS NULL
BEGIN
	update sab
	set Completed = 1
	from eddsdbo.[SearchAuditBatch] sab
	where hourid in
		(select h.id from eddsdbo.hours h
		where h.Score is not null and h.Status != 4
		and h.hourtimestamp >= DATEADD(DAY, -7, getutcdate()))
	
	ALTER TABLE eddsdbo.[SearchAuditBatch]
    ADD [HourSearchAuditBatchId] int null
	
	set @sql =N'
	begin tran

	update sab
	set HourSearchAuditBatchId = (select top 1 hsab.Id from eddsdbo.HourSearchAuditBatches hsab where hsab.ServerId = sab.ServerId and hsab.HourId = sab.HourId)
	from eddsdbo.[SearchAuditBatch] sab
	
	ALTER TABLE eddsdbo.[SearchAuditBatch]
    DROP COLUMN [HourId]
	
	ALTER TABLE eddsdbo.[SearchAuditBatch]
    DROP COLUMN [ServerId]

	commit tran
	'
	exec sp_sqlexec @sql

END

-- Remove BatchesCompleted
IF COL_LENGTH ('eddsdbo.HourSearchAuditBatches' ,'BatchesCompleted') IS NOT NULL
BEGIN
	ALTER TABLE eddsdbo.[HourSearchAuditBatches]
    DROP COLUMN [BatchesCompleted]
END