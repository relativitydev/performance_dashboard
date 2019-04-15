USE EDDSQoS
GO

IF EXISTS (select 1 from sysobjects where [name] = 'TuningForkSys' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.TuningForkSys
END

GO

IF EXISTS ( SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TuningForkSystemConfig' )
BEGIN
	DROP TABLE EDDSDBO.TuningForkSystemConfig
END

GO

CREATE PROCEDURE EDDSDBO.[TuningForkSys]
	@currentServerName nvarchar (255)
AS
BEGIN

	/*

	Here are the specific things that this script will check:

	SQL SERVER SETTINGS

	1.  Remote Query Timeout (depreciated)
	2.  Max Server Memory
	3.  Lightweight Pooling
	4.  Affinity64 Mask
	5.  Optimize for Ad Hoc Workloads
	6.  Query Wait(s)
	7.  Set Working Set size
	8.  Tempdb Check
	9.  Cross DB Ownership Chaining
	10. Max Worker Threads
	11. Network Packet Size
	12. Priority Boost
	13. Max Degree of Parallelism
	14. Minimum Memory per Query
	15. Cost Threshold For Parallelism
	16. Min Server Memory

	Relativity Configuration Table

	--This script will check approximately 60 different values in the Configuration table of the EDDS database in your Relativity instance.
	--It will check for optimal settings and compare against default values.  It will return any values that are identified as non-default or not optimal for the environment.

	For values that are identified as problematic, the script will return recommendations for improved performance

	*/

	--Contact kCura support@kcura.com and request assistance from Infrastructure Engineering for assistance with this script.
	--Version 8.0 6/24/2013 - Scott R. Ellis, Jared Lander
	--Version 8.1 12/6/2013 - added min memory check, fixed some incorrect logic in tempdb check, expanded range of values that can pass the max server memory check (now accepts the kIE_value or anything in the range of 80-90% of total RAM)
	--Version 8.1.1 3/7/2014 - added logic to only run the min memory per query check if the value has been changed from the default, fixed an issue that would cause the script to fail in SQL Server 2012
	
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	/*Some options require a server restart before the new configuration value takes effect. 
	If you set the new value and run sp_configure before restarting the server, the new value 
	appears in the configuration options value column, but not in the value_in_use column. After 
	restarting the server, the new value appears in the value_in_use column.
	*/
	IF EXISTS(SELECT TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TuningForkSystemConfig ') DROP TABLE EDDSDBO.TuningForkSystemConfig

	DECLARE @TotalRAM INT
	DECLARE @SQL nvarchar(max)

	IF (SELECT LEFT(CONVERT(varchar, SERVERPROPERTY('productversion')),2)) >= '11'
	BEGIN	
		SET @SQL = 'select @RAM = convert(int, cast(ROUND((physical_memory_kb/1048576.),2) as numeric(10,0))) from sys.dm_os_sys_info'
		EXECUTE sp_executesql @SQL, N'@RAM INT OUTPUT', @RAM = @TotalRAM OUTPUT
	END
	ELSE
	BEGIN
		SET @SQL = 'select @RAM = convert(int, cast(ROUND((physical_memory_in_bytes/1073741824.),2) as numeric(10,0))) from sys.dm_os_sys_info'
		EXECUTE sp_executesql @SQL, N'@RAM INT OUTPUT', @RAM = @TotalRAM OUTPUT
	END


	DECLARE @CurrentValue int
	DECLARE @minimumCurrentValue int
	DECLARE @maximumCurrentValue int
	DECLARE @isDistributed int
	DECLARE @kIE_Val int
	DECLARE @TotalCores int
	DECLARE @CPUThreads int
	DECLARE @coresPerNode int
	DECLARE @message varchar(max)
	DECLARE @kIE_Status varchar(max)
	declare @recommendatonDefaultID uniqueidentifier

	--First, determine how many CPUs the server is using
	select @CPUThreads = sum(convert(int,is_online)) from sys.dm_os_schedulers where status='VISIBLE ONLINE'

	--Determine how many cores per node
	select @coresPerNode = MIN(online_scheduler_count) from sys.dm_os_nodes where node_state_desc NOT LIKE '%DAC%' and node_state_desc != 'OFFLINE'

	SET @TotalCores = @CPUThreads --logical cores

	SET NOCOUNT ON
	Select * into EDDSDBO.TuningForkSystemConfig from sys.configurations where name in (
	 'query wait (s)', /*Use the query wait option to specify the time in seconds (from 0 through 2147483647) that a query waits for resources before timing out. 
	 If the default value of -1 is used, or if –1 is specified, then the time-out is calculated as 25 times of the estimated query cost. 

	In Microsoft SQL Server, memory-intensive queries (such as those involving sorting and hashing) are queued when there is not enough memory available 
	to run the query. The query times out after a set time calculated by SQL Server (25 times the estimated cost of the query) or the time specified by the 
	nonnegative value of the query wait.
	*/
	 'min memory per query (KB)', /*Use the min memory per query option to specify the minimum amount of memory (in kilobytes (KB)) that are allocated for the 
	 execution of a query. For example, if min memory per query is set to 2,048 KB, the query is guaranteed to get at least that much total memory. You can set 
	 min memory per query to any value from 512 through 2,147,483,647 bytes (2 gigabytes (GB)). The default is 1,024 KB.

	The SQL Server query processor attempts to determine the optimal amount of memory to allocate to a query. The min memory per query option lets the administrator 
	specify the minimum amount of memory any single query receives. Queries generally receive more memory than this if they have hash and sort operations on a large 
	volume of data. Increasing the value of min memory per query may improve performance for some small to medium-sized queries, but doing so could lead to increased 
	competition for memory resources. The min memory per query option includes memory allocated for sorting and replaces the sort pages option in SQL Server 7.0 or 
	earlier versions.
	*/
	 'max worker threads',  /*Thread pooling helps optimize performance when large numbers of clients are connected to the server. Usually, a 
	 separate operating system thread is created for each query request. However, with hundreds of connections to the server, using one thread 
	 per query request can consume large amounts of system resources. The max worker threads option enables SQL Server to create a pool of worker 
	 threads to service a larger number of query request, which improves performance.  The default value for max worker threads, 0, allows SQL 
	 Server to automatically configure the number of worker threads at startup. This setting is best for most systems; however, depending on your 
	 system configuration, setting max worker threads to a specific value sometimes improves performance.  This script evaluates the number of cores, the number 
	 of CPUs in use by SQL server, the number of worker threads, and then makeas a recommendation based on a known good formula used to calculate 
	 the max number of worker threads your system can tolerate.  If the number of existing threads is higher than this amount, we recommend lowering it.*/
	 'affinity64 mask',     --0 this should not be set under any circumstances
	 'priority boost',   /*--0 Based on actual support experience, you do not need to use priority boost for good performance. If you do use 
	 priority boost, it can interfere with smooth server functioning under some conditions and you should not use it except under very 
	 unusual circumstances. For example, Microsoft Product Support Services might use priority boost when they investigate a performance issue. 
	 */
	 'index create memory (KB)',/*Due to introduction of partitioned tables and indexes in SQL Server 2005, the minimum 
	 memory requirements for index creation may increase significantly in case of non-aligned partitioned indexes and a 
	 high degree of parallelism. Starting with  SQL Server 2005, this option controls the total initial amount of memory 
	 allocated for all index partitions in a single index creation operation. The query will terminate with an error 
	 message if the amount set by this option is less than the minimum required to run the query. 
	  SQL server requires at least 1024 KB of memory per DOP.  The index create memory option is self-configuring when it is set to 0 and 
	  seldom requires adjustment.  This portion of the below script checks to see if there have been problems creating indexes and 
	  recommends an adjustment of this if needed.
	*/


	'lightweight pooling',  --should be set to 0
	--should be changed if the following are true:
	--Large multi-processor servers are in use.
	--All servers are running at or near maximum capacity.
	--A lot of context switching occurs (greater than 20,000 per second).
	'max degree of parallelism',  --should not be 0, should be some value between 1 and 8
	'Max Server Memory (MB)', --should be less than 1000000
	'min server memory (MB)', --should not be changed
	'set working set size',   --should not be changed
	'cross db ownership chaining',/*If you have databases that require cross-database ownership chaining, the recommended practice is to 
	turn off the cross db ownership chaining option for the instance using sp_configure; then turn on cross-database ownership chaining 
	for individual databases that require it using the ALTER DATABASE statement.*/
	'network Packet Size (B)',  -- If an application does bulk copy operations, or sends or receives large amounts of text or image data, a packet size larger than the default may improve efficiency because it results in fewer network reads and writes.
	'optimize for ad hoc workloads', --1, When this option is set, plan cache size is further reduced for single-use adhoc OLTP workload.
	'cost threshold for parallelism' --SQL Server creates and runs a parallel plan for a query only when the estimated cost to run a serial plan for the same query is higher than the value set in cost threshold for parallelism. The cost refers to an estimated elapsed time in seconds required to run the serial plan on a specific hardware configuration.
	)
	Alter TABLE EDDSDBO.TuningForkSystemConfig
		ADD  kIE_value int, kIE_Note varchar(max), kIE_Status varchar(25), Severity int

		
	/*****************************		cross db ownership chaining		*******************************/
	--Further investigation is needed on this item.
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 400
	--is it set to 0 already?
	set @recommendatonDefaultID = null
	IF @CurrentValue = 1
		set @recommendatonDefaultID = '1eba7b80-5ddf-4daf-9696-1e7f8b520fcf'
	IF @CurrentValue = 0
		set @recommendatonDefaultID = 'd003a94b-8f77-4756-a618-0b2f743af2f7'
		
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = 0,
	kIE_Status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,null,null,null,null,null)
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts 
	WHERE configuration_id = 400 and dflts.ID = @recommendatonDefaultID
		 

    /*****************************     Max Worker Threads Section        ******************************/
	
	set @recommendatonDefaultID = null
	DECLARE @maxWorkers int
	DECLARE @threadCount int
	--what is the current value of this?
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 503

	--IF @HyperThreading = 1
	--SET @TotalCores = @TotalCores / 2

	SET @kIE_Val = 512 + ((@TotalCores - 4) * 16)
	--This one lists the total number of CPUs and could be used to detect if the total number of CPUs is in use is less than the total number of CPUsselect * From sys.dm_os_sys_info 

	SELECT @maxWorkers = /*select*/ max_workers_count from sys.dm_os_sys_info 
	SELECT @threadCount = /*select*/ count(*) from sys.dm_os_threads

	--select count(*) from sys.dm_os_threads
	IF @CurrentValue = 0
	BEGIN
		/*possible scenarios: 
		--Max Worker Threads is set to 0,  actual worker processes are less than the formula recommends.  Everything is fine.*/
		IF @threadCount <=  @kIE_Val 
		BEGIN	
			set @recommendatonDefaultID = '3c15f773-493a-402d-a3dd-241517c4c19d'
		END
		/*--Max Worker Threads is set to 0, worker processes are more than the formula recommends. Setting the Worker threads to @kie_value may improve performance. */
		IF @threadCount >  @kIE_Val 
		BEGIN	
			set @recommendatonDefaultID = 'c8d96bf0-c955-4cde-88e1-cb0a8a89211a'
		END
	END
	
	/*--Max Worker Threads is set to > 0, actual worker processes are less than the formula recommends, and the set value is less than the formula
	"Perfomance may be impacted, the value for worker threads is set lower than the recommended value"
	*/
	IF @CurrentValue <  @kIE_Val and @threadCount < @kIE_Val and @CurrentValue != 0
	BEGIN	
		set @recommendatonDefaultID = '9ea8b538-e605-4a64-b1c4-1801f4ae54d8'
	END
	/*--Max Worker Threads is set to >0, actual worker processes are more than the formula recommends, and the set value is more than the formula
	"Perfomance may be impacted, the value for worker threads is set higher than the recommended value and worker threads are exceeding this value"
	*/	
	IF @threadCount >  @kIE_Val and  @CurrentValue > @kIE_Val
	BEGIN
		set @recommendatonDefaultID = '17bfdd92-ada0-421b-a9c7-252468df49a7'
	END
	
	/*--Max Worker Threads is set to > 0, actual worker processes are less than the formula recommends, and the set value is more than the formula
	"Perfomance may be impacted, the value for worker threads is set higher than the recommended value but worker threads are currently less than this value"
	*/	
	IF @threadCount < @kIE_Val and  @CurrentValue > @kIE_Val
	BEGIN
		set @recommendatonDefaultID = '26faa179-78e6-4896-9322-68183ded976b'
	END
	IF @threadCount <= @kIE_Val and  @CurrentValue = @kIE_Val
	BEGIN
		set @recommendatonDefaultID = 'eba17f10-555e-42b8-85c7-e2ff0b31b665'
	END
		
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),convert(varchar,@threadCount),null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 503 and dflts.ID = @recommendatonDefaultID
		
	/*****************************************     network packet size (B)				*********************************************/
	set @recommendatonDefaultID = null
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 505
	SET @kIE_Val = 4096

	IF @CurrentValue = @kIE_Val 
		set @recommendatonDefaultID = 'af68bd6f-c8b1-478f-9a7f-82c533e66c8b'

	IF @CurrentValue > 8060
		set @recommendatonDefaultID = '8fe08b80-0cf1-480e-8d2e-a93710fb8d8d'

	IF @CurrentValue < @kIE_Val
		set @recommendatonDefaultID = 'fce3ac74-0746-417b-b3cb-a818653bfa80'

	IF @CurrentValue <= 8060 and @CurrentValue > @kIE_Val
		set @recommendatonDefaultID = '50dd2068-3948-4b49-8ff5-fc698e99c556'	

	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),null,null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 505 and dflts.ID = @recommendatonDefaultID
	
	--This value is currently set to 0, which means that SQL will manage   this.  However, the MINIMUM index create memory is set to 704 Bytes, 
	--which is less than the value of the min memory per query configuration of  1024 Bytes.  This may cause undesirable performance when SQL 
	--server  attempts to create an index using less than 1024 Bytes. This value should be set to some value higher than 1024.  
	--Correct this by either setting the Index Create Memory to a value greater than the min memory per query setting, or by lowering the min 
	--memory per query setting.  Run this sscript again for additional recommendations.

	/*****************************************     index create memory (KB)				*********************************************/
	set @recommendatonDefaultID = null
	DECLARE @minMemPQVal int
	SET @kIE_Val = @TotalCores * 1024 

	SELECT @minMemPQVal = convert(varchar,value) from EDDSDBO.TuningForkSystemConfig where configuration_id = 1540
	--SELECT @minimumCurrentValue = convert(varchar,minimum) from EDDSDBO.TuningForkSystemConfig where configuration_id = 1505
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1505
	/* this should never be set less than the min memory per query option.  Check this.*/

	IF @CurrentValue <> 0 and @CurrentValue < @minMemPQVal
		set @recommendatonDefaultID = 'e3c48460-0ae1-4051-9edf-790577a372a0'

	--IF @CurrentValue = 0 and @minimumCurrentValue < @minMemPQVal   
	--update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = 'Fine Tuning', kIE_note = 'This value is currently set to 0, which means that SQL will manage 
	--this.  However, the MINIMUM index create memory is set to ' + CONVERT(varchar,@minimumCurrentValue) + ' KB, which is less than the value of the min memory per 
	--query configuration of  ' + CONVERT(varchar,@minMemPQVal) + ' KB.  This may cause undesirable performance when SQL server  attempts to create an index using 
	--less than 1024 Bytes. This value should be set to some value greater than or equal to ' + CONVERT(varchar,@minMemPQVal) + '.  Correct this by either setting the Index Create 
	--Memory to a value greater than or equal to the min memory per query setting, or by lowering the min memory per query setting. 
	--Run this script again for additional recommendations.' 
	--where configuration_id = 1505

	IF @CurrentValue <> 0 and (@CurrentValue < @kIE_Val  or @CurrentValue > @kIE_Val) and @CurrentValue >= @minMemPQVal
	 	set @recommendatonDefaultID = '481f0306-d691-4bc4-99f0-db834c324221'
	 
	IF @CurrentValue = 0
	 	 set @recommendatonDefaultID = '861952e8-ce31-4347-acbf-edc61c0dc4a9'
	 
	IF @CurrentValue = @kIE_Val
		set @recommendatonDefaultID = '81e8e25d-b348-4a64-8ad0-c67f30621dee'
	 
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),CONVERT(varchar,@minMemPQVal),null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1505 and dflts.ID = @recommendatonDefaultID

	/*****************************************     priority boost						*********************************************/
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1517
	SET @kIE_Val = 0
	IF @CurrentValue <> @kIE_Val 
		set @recommendatonDefaultID = '8c742afa-ee9b-4f98-88d9-b8c1ecc48c04'
	ELSE
		set @recommendatonDefaultID = '6ea79ddb-8f08-4acf-beff-6819c3433d3a'
	 
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),CONVERT(varchar,@minMemPQVal),null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1517 and dflts.ID = @recommendatonDefaultID

	/*****************************************     set working set size					*********************************************/
	
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1532
	SET @kIE_Val = 0
	IF @CurrentValue != @kIE_Val
		set @recommendatonDefaultID = '75e9de81-db8b-450c-8865-70ecdc506608'
	ELSE
		set @recommendatonDefaultID = '8abd2c05-95e0-46bf-88d6-1c6c1c38b879'
	 
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1532 and dflts.ID = @recommendatonDefaultID

	/*****************************************     max degree of parallelism			*********************************************/

	set @recommendatonDefaultID = null
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1539
	if @TotalCores <= 16 
	BEGIN
		SET @kIE_Val = @TotalCores / 2
		IF @kIE_Val > @coresPerNode
		BEGIN
			SET @kIE_Val = @coresPerNode
		END
	END
	if @TotalCores > 16 
	BEGIN
		SET @kIE_Val = 8
		IF @kIE_Val > @coresPerNode
		BEGIN
			SET @kIE_Val = @coresPerNode
		END
	END
	IF @CurrentValue > @kIE_Val
		set @recommendatonDefaultID = '99ca8350-7bcc-411c-a0b0-eade691bdd90'
	ELSE IF (@kIE_Val > @CurrentValue) AND (@kIE_Val - @CurrentValue) <= 2 AND (@kIE_Val <> @CurrentValue)
		set @recommendatonDefaultID = 'e682ce1f-b80b-4fbd-b399-a258215be7e9'
	ELSE IF (@kIE_Val - @CurrentValue) > 2
		set @recommendatonDefaultID = '2469c191-46ae-4dac-bbbe-ccd871678cf3'
	ELSE IF @kIE_Val = @CurrentValue
		set @recommendatonDefaultID = '6eda7b8d-7f4f-4e22-bf82-420219d1bb64'
		
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1539 and dflts.ID = @recommendatonDefaultID

	/*****************************************     max server memory (MB)				*********************************************/
	
	set @recommendatonDefaultID = null
	DECLARE @MemoryCommitted INT

	--nOTE - adjust this for a GIG of RAM for every 10 gb

	/*A Note about NUMA: 
	The NUMA architecture was designed to surpass the scalability limits of the SMP architecture. With SMP, which stands for Symmetric Multi-Processing, all 
	memory access are posted to the same shared memory bus. This works fine for a relatively small number of CPUs, but the problem with the shared bus appears 
	when you have dozens, even hundreds, of CPUs competing for access to the shared memory bus. NUMA alleviates these bottlenecks by limiting the number of CPUs 
	on any one memory bus, and connecting the various nodes by means of a high speed interconnect.
	*/
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1544
	/*Especially, we should pay attention to the size of buffer pool (since it is the source for
	 query execution memory grant) and the size of memory held by query execution. You can use 
	 this simple query to get the size of buffer pool:*/ 
	--Determine how much memory is currently in use:
	select @MemoryCommitted = sum(virtual_memory_committed_kb) from sys.dm_os_memory_clerks where type='MEMORYCLERK_SQLBUFFERPOOL'

	IF @TotalRAM <= 40 and @TotalRAM >= 32
		SET @kIE_Val = @TotalRAM*1024 - 4096
	ELSE IF @TotalRAM >40
		SET @kIE_Val = ROUND((@TotalRAM*1024)*.9, 0)
	ELSE IF @TotalRAM < 32
		SET @kIE_Val = NULL

	IF @kIE_Val IS NULL
		set @recommendatonDefaultID = 'ad0f38b8-96e4-4aca-b2ad-c3f0242c5523'
	--If MAX Memory is not set within 10% of the kIE value, but the current memory is not causing a problem, warn of Severe.
	ElSE IF @CurrentValue > @kIE_Val and @MemoryCommitted/1024 <= @kIE_Val and @kIE_Val IS NOT NULL
		set @recommendatonDefaultID = '35a8fe79-5159-4bed-8f0f-c9128632cf0d'
	--If MAX Memory is not set within 10% of the kIE value, and the current memory is causing a problem, warn of Critical.
	ELSE IF @CurrentValue > @kIE_Val and @MemoryCommitted/1024 > @kIE_Val and @kIE_Val IS NOT NULL
		set @recommendatonDefaultID = '4a457915-a881-4a82-9a54-9634b51b70d4'
	ELSE IF @CurrentValue < (@TotalRAM*1024)*.8
		set @recommendatonDefaultID = 'ef9698c1-68e6-4483-9e21-86bdd4ae4579'
	ELSE IF (@CurrentValue >= (@TotalRAM*1024)*.8 AND @CurrentValue <= (@TotalRAM*1024)*.9) OR @CurrentValue = @kIE_val
		set @recommendatonDefaultID = 'c1e546c1-6f91-4572-9c99-7d88f3f26319'
	
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1544 and dflts.ID = @recommendatonDefaultID

	/*****************************************    min memory per query (KB)			*********************************************/
	
	set @recommendatonDefaultID = null
	--Only perform this check if the value has been changed from the default of 1024
	IF (SELECT convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1540) <> 1024
	BEGIN
		SET @kIE_Val = @CurrentValue --grab the max memory setting from the previous section for use here

		SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1540
		/* The amount of min memory per query has precedence over the index create memory Option. In Microsoft SQL Server 2000 these options were independent, 
		while in Microsoft SQL Server 2005 they interact. If you alter both options and the index create memory is less than min memory per query, you receive a 
		warning message, but the value is set. During query execution you receive another similar warning.

		In some cases, if SQL Server has available RAM, the performance can be boosted by increasing this value, such as to 2048 KB, or perhaps a little higher. 
		As long as there is RAM that is not being used by SQL Server, boosting this setting can help overall SQL Server performance.
		If there is no extra memory available, increasing the amount of memory for this setting is more likely to hurt overall performance, not help it.

		Run this script several times on different days during high load times to see if the recommendation changes.*/

		--Total memory currently in use:
		--select sum(virtual_memory_committed_kb) from sys.dm_os_memory_clerks where type='MEMORYCLERK_SQLBUFFERPOOL'

		--the size of memory held by query execution:
		--select sum(total_memory_kb) from sys.dm_exec_query_resource_semaphores

		--If the total memory in use is 25% less than total RAM available (max mem) then recommend increasing this value.
		IF @MemoryCommitted/1024/1024 < (@kIE_Val * .75) and @CurrentValue <= 1024
			set @recommendatonDefaultID = 'aa479fa9-f5d2-4766-a148-822a9cd2d363'
		ELSE IF @CurrentValue < (SELECT value from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1505)
			set @recommendatonDefaultID = 'dff1fabd-9b64-414f-8010-d5f93c662416'
		ELSE
			set @recommendatonDefaultID = 'b5e3d2cf-dfc4-49a2-81ef-c1c4adf5af08'
	END
	ELSE
	begin
		--added by DWG 9/11/15
		set @recommendatonDefaultID = 'b5e3d2cf-dfc4-49a2-81ef-c1c4adf5af08'
	end
	
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),null,CONVERT(varchar, ROUND(@kie_Val/1024,0)),CONVERT(varchar, @MemoryCommitted/1024/1024) ,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1540 and dflts.ID = @recommendatonDefaultID

	/*****************************************     min server memory (MB)				*********************************************/
	SET @kIE_Val = 0
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1543
	IF @CurrentValue = @kIE_Val
		set @recommendatonDefaultID = '4479210e-b9c5-4acd-b3f6-2ad7ffa45b0a'
	ELSE
		set @recommendatonDefaultID = '5757f290-0305-45ae-8bf9-65e9d398d598'
	
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,null,null,null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1543 and dflts.ID = @recommendatonDefaultID
	
	/*****************************************     lightweight pooling					*********************************************/
	SET @kIE_Val = 0
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1546
	IF @CurrentValue != @kIE_Val
		set @recommendatonDefaultID = '65faaff3-4711-4052-b825-343764ff9a6b'
	ELSE
		set @recommendatonDefaultID = '5c727566-3acc-4689-b7c0-f39aab00ccdb'
	 
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,CONVERT(varchar,@currentValue),CoNVERT(varchar,@kIE_Val),null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1546 and dflts.ID = @recommendatonDefaultID

	/*****************************************     affinity64 mask						*********************************************/

	SET @kIE_Val = 0
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1549
	IF @CurrentValue != @kIE_Val
		set @recommendatonDefaultID = 'b50f8ac0-9059-4e40-b51e-65bf9f0da7bf'
	ELSE
		set @recommendatonDefaultID = '8b1510b9-f50e-4eee-b2a2-2dd786a36d30'
	 
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,CONVERT(varchar,@currentValue),CoNVERT(varchar,@kIE_Val),null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1549 and dflts.ID = @recommendatonDefaultID

	/*****************************************     optimize for ad hoc workloads		*********************************************/
	--the premise of this is that if the total MBs used by adhoc is high, then this option should be set. This examines the plan cache 
	--and makes a recommendation based on percent use.  Currently, it looks to see if the adhoc cache is larger than the prepared one.
	DECLARE @adHocMB int
	DECLARE	@PreparedMB int
	SET @kIE_Val = 1 
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1581
	--First, check to see how the plan cache is currently being used

	SELECT @adHocMB = sum(cast(size_in_bytes as decimal(18,2)))/1024/1024 from sys.dm_exec_cached_plans where objtype = 'Adhoc'
	GROUP BY objtype

	SELECT @PreparedMB  = sum(cast(size_in_bytes as decimal(18,2)))/1024/1024 from sys.dm_exec_cached_plans where objtype = 'Prepared'
	GROUP BY objtype

	IF @adHocMB < @PreparedMB  and @CurrentValue != 1
		set @recommendatonDefaultID = 'a064cc47-1c49-4c3b-95c6-28471fc75110'
	ELSE IF @adHocMB > @PreparedMB and @CurrentValue != 1
		set @recommendatonDefaultID = '855d14c2-122c-44e6-b1f7-e860be6e222d'
	ELSE
		set @recommendatonDefaultID = 'd8d48787-41de-4ffc-95f0-a22fb4620ad3'

	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,CONVERT(varchar,@currentValue),null,CONVERT(varchar,@PreparedMB),CONVERT(varchar,@adHocMB),null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1581 and dflts.ID = @recommendatonDefaultID
	 
	--sp_configure 'optimize for ad hoc workloads',0
	--reconfigure


	/*
	SELECT objtype AS [CacheType]
			, count_big(*) AS [Total Plans]
			, sum(cast(size_in_bytes as decimal(18,2)))/1024/1024 AS [Total MBs]
			, avg(usecounts) AS [Avg Use Count]
			, sum(cast((CASE WHEN usecounts = 1 THEN size_in_bytes ELSE 0 END) as decimal(18,2)))/1024/1024 AS [Total MBs - USE Count 1]
			, sum(CASE WHEN usecounts = 1 THEN 1 ELSE 0 END) AS [Total Plans - USE Count 1]
	from sys.dm_exec_cached_plans
	GROUP BY objtype
	ORDER BY [Total MBs - USE Count 1] DESC   */


	--Read more: http://www.sqlskills.com/blogs/kimberly/post/procedure-cache-and-optimizing-for-adhoc-workloads.aspx#ixzz1Jt2p8zOq

	/*****************************************     Query Wait (s)								*********************************************/
	/*--this is the maximum amount of time that SQL will wait for a resrouce before timing out.  if it is set to -1, then the timeout is calculated to be 25 times the estimated cost of the query.  If it is set 

	to a non-negative number,
	then that is the nuymber of seconds it will wait. This value should only be increased if you know that timeouts are a problem and you wish to increase it.  However, this is 
	not wise to change because some queries take longer than others, and a waiting query may hold locks while it waits for memory.  If this setting is too high, it is possible that deadlocks will occur.  
	*/
	SET @kIE_Val = -1
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1541
	IF @CurrentValue != @kIE_Val 
		set @recommendatonDefaultID = 'caa0367d-0bb6-40f7-abf1-1a08e2e122ae'
	ELSE
		set @recommendatonDefaultID = 'f20a1c05-8f23-4d4e-b286-a7abcb8776ca'
	 
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,CONVERT(varchar,@currentValue),null,null,null,null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1541 and dflts.ID = @recommendatonDefaultID

	/*****************************************     Temp dbs								*********************************************/
	--total number of free pages and total free space in megabytes available in all files in the tempdb.
	set @recommendatonDefaultID = null
	--Get the total number of tempdbs
	DECLARE @tempdbCount int 

	SET @kIE_Val = 8

	SELECT @tempdbCount = COUNT(*) from tempdb.sys.database_files where type_desc like '%rows%'
	
	IF @tempdbCount < @totalcores and @Totalcores < @kIE_Val  --the total number of tempDBs is less than cores and there aren't enough cores
		set @recommendatonDefaultID = '5b3b8dbc-0ef9-4bb9-bcb9-7cee61605278'

	IF @tempdbCount > @totalcores and @TotalCores < @kIE_Val  --the total number of temDBs is more than cores, and there aren't enough cores
		set @recommendatonDefaultID = '90f27695-c436-487b-8a6e-aaf128e55fc1'
			
	IF @tempdbCount < @totalcores and @TotalCores >= @kIE_Val and @tempdbCount < @kIE_Val --the total number of tempDBs is less than the number of cores.
		set @recommendatonDefaultID = 'c7e707f3-779d-4480-8157-3e6d89ad96c4'

	IF @tempdbCount > @totalcores and @TotalCores < @kIE_Val  -- the total number of tempDBs is less than the number of cores and there aren't enough.
		set @recommendatonDefaultID = '6c32b75d-9d1c-4eba-8a60-58d46e571351'
		
	IF @tempdbCount < @totalcores and @tempdbCount > @kIE_Val  and @TotalCores > @kIE_Val  -- the total number of tempDBs is less than the number of cores and there aren't enough.
		set @recommendatonDefaultID = '45e257c4-84c5-40bb-9d70-7d447df77cf2'
		
	IF @tempdbCount > @totalcores and @tempdbCount > @kIE_Val  and @TotalCores > @kIE_Val
		set @recommendatonDefaultID = 'b2e0da01-ad3f-4f07-96aa-063ea891f149'

	IF @tempdbCount = @kIE_Val and @kIE_Val <= @TotalCores
		set @recommendatonDefaultID = 'cf9bf7de-aa32-458a-bef1-1a412a4d123b'
		
	IF @tempdbCount = @totalcores and @TotalCores > @kIE_Val 
		set @recommendatonDefaultID = 'ef44198b-38f6-45a4-92f4-e17b8d160995'

	IF @tempdbCount = @totalcores and @TotalCores < @kIE_Val 
		set @recommendatonDefaultID = '7f0e09b3-61fd-4a08-abf9-08a2b4d61471'

	IF @tempdbCount < @totalcores and @TotalCores >= @kIE_Val and @tempdbCount < @kIE_Val
		set @recommendatonDefaultID = '27da9966-7381-4b8c-b185-bee3d0eaff6f'

	Insert into EDDSDBO.TuningForkSystemConfig (
		configuration_id, name, value, minimum, maximum, value_in_use, [description], is_dynamic, is_advanced, kIE_value, kIE_Note, kIE_Status, Severity
	) values (
		100000,'Tempdb Check',@tempdbCount,0 ,16,@tempdbCount
		,'The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.'
		, 0, 0, @kIE_Val, null, null, 0)

	
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),CONVERT(varchar,@tempDBCount),convert(varchar,@TotalCores),null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 100000 and dflts.ID = @recommendatonDefaultID


	/*****************************************     Cost Threshold for Parallelism 	*********************************************/

	SET @kIE_Val = 50
	SELECT @CurrentValue = convert(int,value) from EDDSDBO.TuningForkSystemConfig WHERE configuration_id = 1538
	IF (@CurrentValue + 25) < @kIE_Val
		set @recommendatonDefaultID = '5e9229fa-2c13-45e0-bfdf-729667720da5'
	ELSE IF @CurrentValue < @kIE_Val AND (@CurrentValue >= (@kIE_Val - 25))
		set @recommendatonDefaultID = '4922db77-b4b7-433d-ba06-49baf9c14434'
	ELSE IF (@CurrentValue - 25) > @kIE_Val
		set @recommendatonDefaultID = 'b7137b99-6cf4-4b52-9781-02bfbfe20ef8'
	ELSE IF @CurrentValue > @kIE_Val AND (@CurrentValue <= (@kIE_Val + 25))
		set @recommendatonDefaultID = 'ab7af983-e30d-455d-a968-9248f5973b15'
	ELSE
		set @recommendatonDefaultID = '4118f3f4-6b5b-4bb3-97ce-33b435ff18c7'
	
	update EDDSDBO.TuningForkSystemConfig SET kIE_value = @kIE_Val, kIE_status = dflts.[Status], Severity = dflts.[Severity],
	kIE_note = eddsdbo.GetEnvCheckRecommendationText(dflts.ID,convert(varchar,@CurrentValue),CONVERT(varchar,@kIE_Val),CONVERT(varchar,@tempDBCount),convert(varchar,@TotalCores),null) 
	from [eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	WHERE configuration_id = 1538 and dflts.ID = @recommendatonDefaultID
	
	
	 
	/* declare @sqloutput nvarchar(max)
	set @sqloutput = '
	DELETE FROM [' + @Server + '].EDDSPerformance.eddsdbo.EnvironmentCheckRecommendations
	WHERE 
	Scope not in (select distinct ServerName from [' + @Server + '].EDDSPerformance.eddsdbo.[Server] where DeletedOn is null)
	and Scope <> ''Relativity''
	
	--clear out previous results
	DELETE FROM [' + @Server + '].EDDSPerformance.eddsdbo.EnvironmentCheckRecommendations
	WHERE Scope = ''' + @ServerName + ''' and [section] = ''SQL Configuration'';
		
	--insert new results
	INSERT INTO [' + @Server + '].EDDSPerformance.eddsdbo.EnvironmentCheckRecommendations
		([Scope], Name, [Description], [Status], [Recommendation], [Value], Section, Severity) 
	SELECT
		''' + @ServerName + ''', name, [description], isnull(kIE_Status, ''Good''), kIE_Note, convert(varchar(100), value), ''SQL Configuration'', isnull(Severity, 0)
		FROM EDDSDBO.TuningForkSystemConfig
	' */
	
	--exec sp_executesql @sqloutput
	
	SELECT
	@currentServerName as [Scope],
	name,
	[description],
	isnull(kIE_Status, 'Good') as [Status],
	kIE_Note as [Recommendation],
	convert(varchar(100), value) as [Value],
	'SQL Configuration' as Section,
	isnull(Severity, 0) as Severity
	FROM EDDSDBO.TuningForkSystemConfig
	
	IF EXISTS(SELECT TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TuningForkSystemConfig') DROP TABLE EDDSDBO.TuningForkSystemConfig


end