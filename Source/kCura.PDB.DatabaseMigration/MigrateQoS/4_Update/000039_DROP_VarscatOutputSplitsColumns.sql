use EDDSQOS

IF EXISTS (SELECT TOP 1 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetail]') AND name = 'NCI_QoSHourID_Bound_Split')
BEGIN
	DROP INDEX NCI_QoSHourID_Bound_Split
	ON eddsdbo.QoS_VarscatOutputDetail
END

IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetail' ,'Split') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetail
    DROP COLUMN Split
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetail' ,'Bound') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetail
    DROP COLUMN Bound
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetail' ,'Finish') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetail
    DROP COLUMN Finish
END