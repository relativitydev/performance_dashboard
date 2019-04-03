USE EDDSQoS

declare @sql nvarchar(max) = N'
delete from '+QUOTENAME(@eddsPerformanceServerName)+'.eddsperformance.eddsdbo.FileLevelLatencyDetails
where ServerName = @currentServerName;

WITH 
fsdata AS (
	select
		FileStatsID,
		DBName,
		ReadLatency,
		WriteLatency,
		LatencyScore,
		ROW_NUMBER() OVER(PARTITION BY DBName ORDER BY ReadLatency+WriteLatency desc) AS rk
	from eddsqos.eddsdbo.QoS_FileStats
	where IsDataFile = 1
),
fslog AS (
	select
		FileStatsID,
		DBName,
		ReadLatency,
		WriteLatency,
		LatencyScore,
		ROW_NUMBER() OVER(PARTITION BY DBName ORDER BY ReadLatency+WriteLatency desc) AS rk
	from eddsqos.eddsdbo.QoS_FileStats
	where IsDataFile = 0
)
insert into '+QUOTENAME(@eddsPerformanceServerName)+'.eddsperformance.eddsdbo.FileLevelLatencyDetails
	([ServerName]
	,[DatabaseName]
	,[Score]
	,[DataReadLatency]
	,[DataWriteLatency]
	,[LogReadLatency]
	,[LogWriteLatency])
(
	select
		@currentServerName as ServerName, 
		fsdata.DBName,
		Score =
		case when fsdata.LatencyScore >= (select top 1 fslog.LatencyScore from fslog where fsdata.DBName = fslog.DBName and fslog.rk = 1)
		then fsdata.LatencyScore
		else (select top 1 fslog.LatencyScore from fslog where fsdata.DBName = fslog.DBName and fslog.rk = 1)
		end,
		fsdata.ReadLatency as DataReadLatency,
		fsdata.WriteLatency as DataWriteLatency,
		LogReadLatency = (select top 1 ReadLatency from fslog where fsdata.DBName = fslog.DBName and fslog.rk = 1),
		LogWriteLatency = (select top 1 WriteLatency from fslog where fsdata.DBName = fslog.DBName and fslog.rk = 1)
	from fsdata
	where fsdata.rk = 1 
)
'

exec sp_executesql @sql, N'@currentServerName nvarchar(255)', @currentServerName
