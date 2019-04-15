INSERT INTO eddsdbo.Reports_RecoveryObjectives 
    (DatabaseId,
    RpoScore,
    RpoMaxDataLoss,
    RtoScore,
    RtoTimeToRecover) 
    VALUES 
    (@DatabaseId,
    @RpoScore,
    @RpoMaxDataLoss,
    NULL,
    NULL)