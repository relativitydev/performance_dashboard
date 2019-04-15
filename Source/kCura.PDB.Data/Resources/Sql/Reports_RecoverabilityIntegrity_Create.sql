INSERT INTO eddsdbo.Reports_RecoverabilityIntegritySummary 
        (HourId,
        OverallScore,
        RpoScore,
        RtoScore,
        BackupFrequencyScore,
        BackupCoverageScore,
        DbccFrequencyScore,
        DbccCoverageScore) 
    VALUES 
        (@HourId,
        @OverallScore,
        @RpoScore,
        @RtoScore,
        @BackupFrequencyScore,
        @BackupCoverageScore,
        @DbccFrequencyScore,
        @DbccCoverageScore)