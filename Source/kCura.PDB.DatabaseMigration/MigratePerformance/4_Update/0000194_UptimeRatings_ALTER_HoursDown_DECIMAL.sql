USE EDDSPerformance
GO

/*
 We're going to change the data type of HoursDown on QoS_Ratings from INT to DECIMAL.
 First, we need to remove the default constraint on this column...
*/

IF EXISTS (
	SELECT TOP 1
		default_constraints.name
	FROM sys.all_columns
	INNER JOIN sys.tables
		ON all_columns.object_id = tables.object_id
	INNER JOIN sys.schemas
		ON tables.schema_id = schemas.schema_id
	INNER JOIN sys.default_constraints
		ON all_columns.default_object_id = default_constraints.object_id
	WHERE 
		schemas.name = 'eddsdbo'
	AND tables.name = 'QoS_UptimeRatings'
	AND all_columns.name = 'HoursDown'
	AND default_constraints.name = 'DF_UptimeRatings_HoursDown'
)
BEGIN
	ALTER TABLE eddsdbo.QoS_UptimeRatings
	DROP CONSTRAINT DF_UptimeRatings_HoursDown
END

GO

--Next, remove the nonclustered index that depends on HoursDown... This index will be recreated by another script
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_UptimeRatings]') AND name = N'NCI_SummaryDayHour_RowHash')
BEGIN
	DROP INDEX NCI_SummaryDayHour_RowHash
	ON eddsdbo.QoS_UptimeRatings
END

GO

--Finally, change the data type
IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_UptimeRatings' AND COLUMN_NAME = 'HoursDown' AND DATA_TYPE = 'INT')
BEGIN
	ALTER TABLE eddsdbo.QoS_UptimeRatings
	ALTER COLUMN HoursDown DECIMAL(4, 3)
END