

select top(1) *
from [eddsdbo].[Hours] with(nolock)
where Status = 3
order by HourTimeStamp desc