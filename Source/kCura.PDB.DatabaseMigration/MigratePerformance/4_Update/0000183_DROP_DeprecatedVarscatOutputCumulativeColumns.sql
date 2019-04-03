USE EDDSPerformance

IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'isChild') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN isChild
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'CreatedBy') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN CreatedBy
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'DateCreated') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN DateCreated
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'SearchText || Conditions') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN [SearchText || Conditions]
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'MaxRunsBySingleUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN MaxRunsBySingleUser
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'MaxRunsUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN MaxRunsUser
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'MaxRunsUserArtifactID') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN MaxRunsUserArtifactID
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'RelationalItemsIncludedInCurrentVersion') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN RelationalItemsIncludedInCurrentVersion
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'total Bytes-FTC(conditions)') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN [total Bytes-FTC(conditions)]
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'Total Words in Search Conditions Search Text Box') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN [Total Words in Search Conditions Search Text Box]
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'QTYOrderByIndexed') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN QTYOrderByIndexed
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'SearchFieldTypes') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN SearchFieldTypes
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'OrderedByFieldTypes') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN OrderedByFieldTypes
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'SearchFieldNames') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN SearchFieldNames
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'QTYIndexedSearchFields') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN QTYIndexedSearchFields
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'TotalUniqueSubSearches') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN TotalUniqueSubSearches
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'LastQueryForm') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN LastQueryForm
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'LongestRunningQueryForm') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN LongestRunningQueryForm
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'NumCancelled') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN NumCancelled
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputCumulative' ,'NumErrored') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
    DROP COLUMN NumErrored
END


