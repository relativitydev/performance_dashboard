use EDDSPerformance

select top(1) DATEDIFF(SECOND,[timestamp], GETUTCDATE()) as SecondsSinceLastAgentHistoryRecord
	from eddsdbo.agenthistory
	order by [timestamp] desc

