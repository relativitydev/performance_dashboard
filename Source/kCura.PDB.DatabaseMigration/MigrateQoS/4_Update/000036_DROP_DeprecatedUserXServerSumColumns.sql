use EDDSQOS

IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'BusiestUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN BusiestUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'BusiestWorkspace') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN BusiestWorkspace
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'QConcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN QConcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'MaxConcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN MaxConcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'SimpleQueryconcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN SimpleQueryconcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'ComplexQueryConcurrency') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN ComplexQueryConcurrency
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalComplexQuery') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalComplexQuery
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalViews') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalViews
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalEdits') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalEdits
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalMassOps') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalMassOps
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalOtherActions') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalOtherActions
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalCLRQ') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalCLRQ
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'MaxConcurrenctUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN MaxConcurrenctUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'MaxArrivalRateUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN MaxArrivalRateUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'ProductivityPercent') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN ProductivityPercent
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalUniqueUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalUniqueUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalSQUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalSQUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalCQUsers') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalCQUsers
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalSQExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalSQExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalCQViewExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalCQViewExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalTOPQueryExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalTOPQueryExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalCOUNTQueryExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalCOUNTQueryExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalLongRunningExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalLongRunningExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalSLRQExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalSLRQExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'TotalCLRQExecutionTime') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN TotalCLRQExecutionTime
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageSQPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageSQPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageCQPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageCQPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageViewsPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageViewsPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageEditsPeruser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageEditsPeruser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageUserSLRQ') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageUserSLRQ
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageUserCRLQ') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageUserCRLQ
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageLVEPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageLVEPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageLEPerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageLEPerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AvgCQScorePerUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AvgCQScorePerUser
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'SLRQLimit') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN SLRQLimit
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'CLRQLimit') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN CLRQLimit
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageUserSLRQWaits') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageUserSLRQWaits
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageUserCLRQWaits') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageUserCLRQWaits
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AVGSLRQOverage') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AVGSLRQOverage
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AVGCLRQOverage') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AVGCLRQOverage
END


IF COL_LENGTH ('eddsdbo.QoS_UserXServerSummary' ,'AverageUserCLRQ') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_UserXServerSummary
    DROP COLUMN AverageUserCLRQ
END




