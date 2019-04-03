INSERT INTO eddsdbo.Reports_RecoverabilityIntegrityRpoSummary 
        (HourId,
        WorstRpoDatabase,
        RpoMaxDataLoss) 
    VALUES 
        (@HourId,
        @WorstRpoDatabase,
        @RpoMaxDataLoss)