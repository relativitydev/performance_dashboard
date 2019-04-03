select ResourceServerId, count_big(ID) as Count from 
(
	{0}
) as a
where ID not in
(select Document from eddsdbo.AnalyticsExcludedDocuments)
group by ResourceServerId