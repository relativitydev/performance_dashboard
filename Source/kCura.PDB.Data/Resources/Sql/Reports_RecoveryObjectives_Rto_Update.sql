UPDATE eddsdbo.Reports_RecoveryObjectives 
    SET RtoScore = @RtoScore,
        RtoTimeToRecover = @RtoTimeToRecover
    WHERE [DatabaseId] = @DatabaseId