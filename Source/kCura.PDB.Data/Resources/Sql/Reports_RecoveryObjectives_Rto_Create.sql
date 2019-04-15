INSERT INTO eddsdbo.Reports_RecoveryObjectives 
    (DatabaseId,
    RpoScore,
    RpoMaxDataLoss,
    RtoScore,
    RtoTimeToRecover) 
    VALUES 
    (@DatabaseId,
    NULL,
    NULL,
    @RtoScore,
    @RtoTimeToRecover)