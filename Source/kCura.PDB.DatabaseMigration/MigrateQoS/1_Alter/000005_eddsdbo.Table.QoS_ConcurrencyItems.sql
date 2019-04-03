USE EDDSQoS
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_ConcurrencyItems' AND TABLE_SCHEMA = 'EDDSDBO')
	DROP TABLE eddsdbo.QoS_ConcurrencyItems

GO

CREATE TABLE eddsdbo.QoS_ConcurrencyItems
(
	CIID INT IDENTITY (1, 1),
	QoS_VODID int,
	QoSHourID bigint,
	UserID int,
	QoSAction int,
	StartSec int,
	EndSec int,
	ExecutionTime int,
	WorkspaceID int,
	hr int,
	isCompleted bit,
	IsComplex bit,
	CONSTRAINT [CIQoS_CIID] PRIMARY KEY CLUSTERED (CIID)
)