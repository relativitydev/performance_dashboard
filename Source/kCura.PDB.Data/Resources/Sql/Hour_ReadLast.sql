

select top(1) * from [eddsdbo].[Hours] h with(nolock)
where h.Status != 4
order by HourTimeStamp desc