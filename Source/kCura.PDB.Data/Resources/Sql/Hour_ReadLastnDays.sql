

select distinct h.* from eddsdbo.[Hours] as h with(nolock)
where h.HourTimeStamp >= @startDate and h.HourTimeStamp <= @endDate and h.Status != 4