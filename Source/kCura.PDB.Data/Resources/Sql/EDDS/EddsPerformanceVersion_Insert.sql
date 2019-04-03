INSERT INTO eddsdbo.[EddsPerformanceVersion]
(
    Major,
    Minor,
    Build,
    Revision
) 
VALUES
(
    @major,
    @minor,
    @build,
    @revision
)