

DECLARE @upper datetime,
    @lower datetime;

 --Take the most recent audited hour as the upper bound of the hours we've scored
SELECT @upper = MAX([HourTimeStamp])
FROM eddsdbo.[Hours] as h with(nolock)
WHERE h.Score is not null and h.Status != 4;

--The lower bound is 90 days prior to that or the earliest scored hour
SELECT @lower = MIN([HourTimeStamp])
FROM eddsdbo.[Hours] as h with(nolock)
WHERE h.Score is not null and [HourTimeStamp] > DATEADD(dd, -90, @upper) and h.Status != 4;

--Output the lower and upper bounds of the sample
SELECT @lower MinSampleDate, @upper MaxSampleDate;