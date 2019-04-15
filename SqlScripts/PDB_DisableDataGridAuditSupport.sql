IF NOT EXISTS (select 1 from EDDS.eddsdbo.Toggle (UPDLOCK) where Name = 'kCura.PDB.Core.Toggles.DataGridAuditAnalysisToggle')
BEGIN
    INSERT INTO EDDS.eddsdbo.Toggle (Name, IsEnabled)
    VALUES ('kCura.PDB.Core.Toggles.DataGridAuditAnalysisToggle', 0);
END
ELSE 
BEGIN
    UPDATE EDDS.eddsdbo.Toggle
    SET IsEnabled = 0
    WHERE Name = 'kCura.PDB.Core.Toggles.DataGridAuditAnalysisToggle'
END