use EDDS

select CAST(DATABASEPROPERTYEX('EDDS', 'COLLATION') as nvarchar(max))