--EDDSPerformance

SELECT TOP 1 CAST(value as varchar(max))
	FROM fn_listextendedproperty(default, default, default, default, default, default, default)
	WHERE objtype is null and objname is null and name = 'QoS'