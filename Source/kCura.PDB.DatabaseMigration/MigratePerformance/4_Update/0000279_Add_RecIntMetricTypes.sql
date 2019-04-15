USE [EDDSPerformance]

-- Delete recoverability integrity metric to category stubs
if exists (select * from eddsdbo.MetricTypesToCategoryTypes where MetricTypeID in (40, 41))
begin
	delete from eddsdbo.MetricTypesToCategoryTypes
	where MetricTypeID in (40, 41)
End

-- Delete recoverability integrity metric stubs
if exists (select * from eddsdbo.MetricTypes where id in (40, 41))
begin
	delete from eddsdbo.MetricTypes
	where id in (40, 41)
end



if not exists (select * from eddsdbo.MetricTypes where id in (42,43,44,45,46,47,48,49))
begin
	insert into eddsdbo.MetricTypes
	([ID], [Name], [SampleType])
	values
	(42, 'Backup Gaps', 0),
	(43, 'DBCC Gaps', 0),
	(44, 'RPO', 0),
	(45, 'RTO', 0),
	(46, 'Backup Frequency', 0),
	(47, 'DBCC Frequency', 0),
	(48, 'Backup Coverage', 0),
	(49, 'DBCC Coverage', 0)
end

if not exists (select * from eddsdbo.MetricTypesToCategoryTypes where MetricTypeID in (42,43,44,45,46,47,48,49))
begin
	insert into eddsdbo.MetricTypesToCategoryTypes
	(MetricTypeID, CategoryTypeID)
	values
	(42, 3),
	(43, 3),
	(44, 3),
	(45, 3),
	(46, 3),
	(47, 3),
	(48, 3),
	(49, 3)
end