use EDDSPerformance


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetailCumulative' ,'IsCount') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
    DROP COLUMN IsCount
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetailCumulative' ,'IsErrored') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
    DROP COLUMN IsErrored
END


