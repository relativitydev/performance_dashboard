INSERT INTO eddsdbo.Reports_RecoveryGaps 
    (DatabaseId,
    ActivityType,
    LastActivity,
    GapResolutionDate,
    GapSize) 
    VALUES 
    (@DatabaseId,
    @ActivityType,
    @LastActivity,
    @GapResolutionDate,
    @GapSize)