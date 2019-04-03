INSERT INTO eddsdbo.Reports_RecoverabilityIntegrityRtoSummary 
        (HourId,
        WorstRtoDatabase,
        RtoTimeToRecover) 
    VALUES 
        (@HourId,
        @WorstRtoDatabase,
        @RtoTimeToRecover)