--Initialize new sample bits to zero

IF EXISTS (SELECT 1 FROM EDDSPerformance.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_SampleHistory' AND COLUMN_NAME = 'IsActiveWeeklySample')
BEGIN
	UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory]
	SET IsActiveWeeklySample = 0 WHERE IsActiveWeeklySample IS NULL
END

IF EXISTS (SELECT 1 FROM EDDSPerformance.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_SampleHistory' AND COLUMN_NAME = 'IsActiveWeekly4Sample')
BEGIN
	UPDATE [EDDSPerformance].[eddsdbo].[QoS_SampleHistory]
	SET IsActiveWeekly4Sample = 0 WHERE IsActiveWeekly4Sample IS NULL
END