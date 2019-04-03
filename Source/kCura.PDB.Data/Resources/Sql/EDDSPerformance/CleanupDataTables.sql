--EDDSPerformance
WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ErrorCountDW WHERE MeasureDate < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ErrorCountDW WHERE MeasureDate < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.PerformanceSummary WHERE MeasureDate < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.PerformanceSummary WHERE MeasureDate < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ServerDiskDW WHERE CreatedOn < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ServerDiskDW WHERE CreatedOn < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ServerDiskSummary WHERE MeasureDate < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ServerDiskSummary WHERE MeasureDate < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ServerDW WHERE CreatedOn < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ServerDW WHERE CreatedOn < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ServerProcessorDW WHERE CreatedOn < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ServerProcessorDW WHERE CreatedOn < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ServerProcessorSummary WHERE MeasureDate < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ServerProcessorSummary WHERE MeasureDate < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ServerSummary WHERE MeasureDate < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ServerSummary WHERE MeasureDate < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.SQLServerDW WHERE CreatedOn < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.SQLServerDW WHERE CreatedOn < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.SQLServerSummary WHERE MeasureDate < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.SQLServerSummary WHERE MeasureDate < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.SQLServerPageouts WHERE SummaryDayHour < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.SQLServerPageouts WHERE SummaryDayHour < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.UserCountDW WHERE MeasureDate < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.UserCountDW WHERE MeasureDate < @cutoff;
END

WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.ConfigurationAudit WHERE CreatedOn < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.ConfigurationAudit WHERE CreatedOn < @cutoff;
END

WHILE EXISTS(SELECT TOP 1 1 FROM eddsdbo.VirtualLogFileSummary WHERE SummaryDayHour < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.VirtualLogFileSummary WHERE SummaryDayHour < @cutoff;
END

WHILE EXISTS(SELECT TOP 1 1 FROM eddsdbo.AgentHistory WHERE [TimeStamp] < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.AgentHistory WHERE [TimeStamp] < @cutoff;
END

WHILE EXISTS(SELECT TOP 1 1 FROM eddsdbo.QoS_CasesToAudit WHERE [AuditStartDate] < @cutoff)
BEGIN
	DELETE TOP (50000) FROM eddsdbo.QoS_CasesToAudit WHERE AuditStartDate < @cutoff
END