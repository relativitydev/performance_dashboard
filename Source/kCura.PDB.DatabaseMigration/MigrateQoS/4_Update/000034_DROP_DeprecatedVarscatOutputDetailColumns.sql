use EDDSQOS



IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetail' ,'IsCount') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetail
    DROP COLUMN IsCount
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetail' ,'IsErrored') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetail
    DROP COLUMN IsErrored
END


