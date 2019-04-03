use EDDSPerformance

IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'BusiestUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN BusiestUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'BusiestWorkspace') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN BusiestWorkspace
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'QConcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN QConcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'MaxConcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN MaxConcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'SimpleQueryconcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN SimpleQueryconcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'ComplexQueryConcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN ComplexQueryConcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalComplexQuery') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalComplexQuery
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalViews') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalViews
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalEdits') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalEdits
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalMassOps') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalMassOps
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalOtherActions') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalOtherActions
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalCLRQ') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalCLRQ
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'MaxConcurrenctUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN MaxConcurrenctUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'MaxArrivalRateUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN MaxArrivalRateUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'ProductivityPercent') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN ProductivityPercent
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalUniqueUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalUniqueUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalSQUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalSQUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalCQUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalCQUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalSQExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalSQExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalCQViewExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalCQViewExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalTOPQueryExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalTOPQueryExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalCOUNTQueryExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalCOUNTQueryExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalLongRunningExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalLongRunningExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalSLRQExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalSLRQExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'TotalCLRQExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN TotalCLRQExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageSQPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageSQPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageCQPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageCQPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageViewsPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageViewsPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageEditsPeruser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageEditsPeruser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageUserSLRQ') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageUserSLRQ
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageUserCLRQ') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageUserCLRQ
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageLVEPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageLVEPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageLEPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageLEPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AvgCQScorePerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AvgCQScorePerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'SLRQLimit') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN SLRQLimit
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'CLRQLimit') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN CLRQLimit
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageUserSLRQWaits') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageUserSLRQWaits
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AverageUserCLRQWaits') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AverageUserCLRQWaits
END


IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AVGSLRQOverage') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AVGSLRQOverage
END

IF COL_LENGTH ('eddsdbo.QoS_UserXInstanceSummary' ,'AVGCLRQOverage') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXInstanceSummary
    DROP COLUMN AVGCLRQOverage
END
