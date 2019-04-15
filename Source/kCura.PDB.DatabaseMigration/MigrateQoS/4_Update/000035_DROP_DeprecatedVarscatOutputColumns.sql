use EDDSQOS


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'isChild') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN isChild
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'CreatedBy') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN CreatedBy
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'DateCreated') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN DateCreated
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'SearchText || Conditions') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN [SearchText || Conditions]
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'MaxRunsBySingleUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN MaxRunsBySingleUser
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'MaxRunsUser') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN MaxRunsUser
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'MaxRunsUserArtifactID') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN MaxRunsUserArtifactID
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'RelationalItemsIncludedInCurrentVersion') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN RelationalItemsIncludedInCurrentVersion
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'total Bytes-FTC(conditions)') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN [total Bytes-FTC(conditions)]
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'Total Words in Search Conditions Search Text Box') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN [Total Words in Search Conditions Search Text Box]
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'QTYOrderByIndexed') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN QTYOrderByIndexed
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'SearchFieldTypes') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN SearchFieldTypes
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'OrderedByFieldTypes') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN OrderedByFieldTypes
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'SearchFieldNames') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN SearchFieldNames
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'QTYIndexedSearchFields') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN QTYIndexedSearchFields
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'TotalUniqueSubSearches') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN TotalUniqueSubSearches
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'LastQueryForm') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN LastQueryForm
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'LongestRunningQueryForm') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN LongestRunningQueryForm
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'NumCancelled') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN NumCancelled
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutput' ,'NumErrored') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutput
    DROP COLUMN NumErrored
END


