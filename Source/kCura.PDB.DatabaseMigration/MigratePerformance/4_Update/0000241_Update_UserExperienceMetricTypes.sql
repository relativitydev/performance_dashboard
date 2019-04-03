USE [EDDSPerformance]

if exists (select * from eddsdbo.MetricTypesToCategoryTypes where MetricTypeID in (10, 11))
begin
	delete from eddsdbo.MetricTypesToCategoryTypes
	where MetricTypeID in (10, 11)
End

if exists (select * from eddsdbo.MetricTypes where id in (10, 11))
begin
	delete from eddsdbo.MetricTypes
	where id in (10, 11)
end

if not exists (select * from eddsdbo.MetricTypes where id in (12))
begin
	insert into eddsdbo.MetricTypes
	([ID], [Name], [SampleType])
	values
	(12, 'AuditAnalysis', 0)
end

if not exists (select * from eddsdbo.MetricTypesToCategoryTypes where MetricTypeID in (12))
begin
	insert into eddsdbo.MetricTypesToCategoryTypes
	(MetricTypeID, CategoryTypeID)
	values
	(12, 1)
end