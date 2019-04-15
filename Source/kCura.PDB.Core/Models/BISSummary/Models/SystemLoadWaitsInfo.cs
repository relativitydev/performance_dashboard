namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class SystemLoadWaitsInfo
	{
		public int Index;
		public int ServerId;
		public string Server;
		public string WaitType;
		public DateTime SummaryDayHour;
		public int OverallScore;
		public int SignalWaitsRatio;
		public long SignalWaitTime;
		public long TotalWaitTime;
		public bool IsPoisonWait;
		public bool IsActiveWeeklySample;
		public decimal PercentOfCPUThreshold;
		public decimal DifferentialWaitingTasksCount;

		public string WaitDescription
		{
			get
			{
				switch (WaitType)
				{
					case "ASYNC_IO_COMPLETION":
						return "Occurs when a task is waiting for I/O operations to finish.";
					case "ASYNC_NETWORK_IO":
						return "Occurs on network writes when the task is blocked behind the network. Verify that the client is processing data from the server. SQL Server built the query results, and it’s just sitting around waiting for the application on the other end of the pipe to consume the results faster. There’s nothing you can do to performance tune the SQL Server here – you have to figure out why the app can’t get the data down faster. It could be a slow network pipe between the app and the SQL Server (like a long distance wide area network), an underpowered client machine, or row-by-row processing happening the application server.";
					case "BACKUPIO":
						return "Occurs when a backup task is waiting for data, or is waiting for a buffer in which to store data. This type is not typical, except when a task is waiting for a tape mount.";
					case "CMEMTHREAD":
						return "Occurs when a task is waiting on a thread-safe memory object. The wait time might increase when there is contention caused by multiple tasks trying to allocate memory from the same memory object.";
					case "CXPACKET":
						return "Occurs with parallel query plans when trying to synchronize the query processor exchange iterator. If waiting is excessive and cannot be reduced by tuning the query (such as adding indexes), consider adjusting the cost threshold for parallelism or lowering the degree of parallelism.";
					case "IO_COMPLETION":
						return "Occurs while waiting for I/O operations to complete. This wait type generally represents non-data page I/Os. Data page I/O completion waits appear as PAGEIOLATCH_* waits.";
					case "LCK_M_BU":
						return "Occurs when a task is waiting to acquire a Bulk Update (BU) lock.";
					case "LCK_M_IS":
						return "Occurs when a task is waiting to acquire an Intent Shared (IS) lock.";
					case "LCK_M_IU":
						return "Occurs when a task is waiting to acquire an Intent Update (IU) lock.";
					case "LCK_M_IX":
						return "Occurs when a task is waiting to acquire an Intent Exclusive (IX) lock.";
					case "LCK_M_RIn_S":
						return "Occurs when a task is waiting to acquire a shared lock on the current key value, and an Insert Range lock between the current and previous key.";
					case "LCK_M_RIn_U":
						return "Task is waiting to acquire an Update lock on the current key value, and an Insert Range lock between the current and previous key.";
					case "LCK_M_RIn_X":
						return "Occurs when a task is waiting to acquire an Exclusive lock on the current key value, and an Insert Range lock between the current and previous key.";
					case "LCK_M_RS_S":
						return "Occurs when a task is waiting to acquire a Shared lock on the current key value, and a Shared Range lock between the current and previous key.";
					case "LCK_M_RS_U":
						return "Occurs when a task is waiting to acquire an Update lock on the current key value, and an Update Range lock between the current and previous key.";
					case "LCK_M_RX_S":
						return "Occurs when a task is waiting to acquire a Shared lock on the current key value, and an Exclusive Range lock between the current and previous key.";
					case "LCK_M_RX_U":
						return "Occurs when a task is waiting to acquire an Update lock on the current key value, and an Exclusive range lock between the current and previous key.";
					case "LCK_M_RX_X":
						return "Occurs when a task is waiting to acquire an Exclusive lock on the current key value, and an Exclusive Range lock between the current and previous key.";
					case "LCK_M_S":
						return "Occurs when a task is waiting to acquire a Shared lock.";
					case "LCK_M_SCH_M":
						return "Occurs when a task is waiting to acquire a Schema Modify lock.";
					case "LCK_M_SCH_S":
						return "Occurs when a task is waiting to acquire a Schema Share lock.";
					case "LCK_M_SIU":
						return "Occurs when a task is waiting to acquire a Shared With Intent Update lock.";
					case "LCK_M_SIX":
						return "Occurs when a task is waiting to acquire a Shared With Intent Exclusive lock.";
					case "LCK_M_U":
						return "Occurs when a task is waiting to acquire an Update lock.";
					case "LCK_M_UIX":
						return "Occurs when a task is waiting to acquire an Update With Intent Exclusive lock.";
					case "LCK_M_X":
						return "Occurs when a task is waiting to acquire an Exclusive lock.";
					case "PAGEIOLATCH_DT":
						return "Occurs when a task is waiting on a latch for a buffer that is in an I/O request. The latch request is in Destroy mode. Long waits may indicate problems with the disk subsystem. It could also be insufficient memory in SQL Server available to cache data, or a lack of indexes on the tables involved, or queries that aren’t sargable.";
					case "PAGEIOLATCH_EX":
						return "Occurs when a task is waiting on a latch for a buffer that is in an I/O request. The latch request is in Exclusive mode. Long waits may indicate problems with the disk subsystem. It could also be insufficient memory in SQL Server available to cache data, or a lack of indexes on the tables involved, or queries that aren’t sargable.";
					case "PAGEIOLATCH_KP":
						return "Occurs when a task is waiting on a latch for a buffer that is in an I/O request. The latch request is in Keep mode. Long waits may indicate problems with the disk subsystem. It could also be insufficient memory in SQL Server available to cache data, or a lack of indexes on the tables involved, or queries that aren’t sargable.";
					case "PAGEIOLATCH_SH":
						return "Occurs when a task is waiting on a latch for a buffer that is in an I/O request. The latch request is in Shared mode. Long waits may indicate problems with the disk subsystem. It could also be insufficient memory in SQL Server available to cache data, or a lack of indexes on the tables involved, or queries that aren’t sargable.";
					case "PAGEIOLATCH_UP":
						return "Occurs when a task is waiting on a latch for a buffer that is in an I/O request. The latch request is in Update mode. Long waits may indicate problems with the disk subsystem. It could also be insufficient memory in SQL Server available to cache data, or a lack of indexes on the tables involved, or queries that aren’t sargable.";
					case "RESOURCE_SEMAPHORE":
						return "Occurs when a query memory request cannot be granted immediately due to other concurrent queries. High waits and wait times may indicate excessive number of concurrent queries, or excessive memory request amounts.";
					case "RESOURCE_SEMAPHORE_QUERY_COMPILE":
						return "Occurs when the number of concurrent query compilations reaches a throttling limit. High waits and wait times may indicate excessive compilations, recompiles, or un-cacheable plans.";
					case "SOS_SCHEDULER_YIELD":
						return "Occurs when a task voluntarily yields the scheduler for other tasks to execute. During this wait the task is waiting for its quantum to be renewed.";
					case "THREADPOOL":
						return "Occurs when a task is waiting for a worker to run on. This can indicate that the maximum worker setting is too low, or that batch executions are taking unusually long, thus reducing the number of workers available to satisfy other batches.";
					case "WRITELOG":
						return "Occurs while waiting for a log flush to complete. Common operations that cause log flushes are checkpoints and transaction commits.";
					default:
						return string.Empty;
				}
			}
		}
	}
}
