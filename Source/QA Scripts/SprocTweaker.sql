USE EDDSPerformance
GO

--Modify BuildAndRateSample
EXEC('
ALTER PROCEDURE eddsdbo.QoS_BuildAndRateSample
	@QoSHourID bigint
	,@summaryDayHour datetime
	,@GlassRunID INT 
AS
	DECLARE @valid bit = 0;
	EXEC EDDSPerformance.eddsdbo.IsLookingGlassDisabled @valid;')

--Modify LookingGlass
EXEC('
ALTER PROCEDURE eddsdbo.QoS_LookingGlass
	@msThreshold int = 0,
	@beginDate datetime = 24943,
	@endDate datetime = 24943,
	@workspace varchar(18) = '',
	@cleanup bit = 0,
	@debug int = 0,
	@install varchar(7) = '',
	@logging bit = 1
AS
	DECLARE @valid bit = 0;
	EXEC EDDSPerformance.eddsdbo.IsLookingGlassDisabled @valid;')

--Modify LookingGlassDateSource
EXEC('
ALTER PROCEDURE eddsdbo.QoS_LookingGlassDateSource
	@startDate datetime = 24943
AS
	DECLARE @valid bit = 0;
	EXEC EDDSPerformance.eddsdbo.IsLookingGlassDisabled @valid;')

--Modify QualityDataGenerator
EXEC('
ALTER PROCEDURE eddsdbo.QoS_QualityDataGenerator
	@GlassRunDateTime datetime,
	@GlassRunID int,
	@summaryDayHour datetime,
	@isRetry int,
	@logging int = 0,
	@debug int
AS
	DECLARE @valid bit = 0;
	EXEC EDDSPerformance.eddsdbo.IsLookingGlassDisabled @valid;')