UPDATE eddsdbo.Reports_RecoveryObjectives 
    SET RpoScore = @RpoScore,
        RpoMaxDataLoss = @RpoMaxDataLoss
    WHERE [DatabaseId] = @DatabaseId