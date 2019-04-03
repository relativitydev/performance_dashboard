USE EDDSResource

DECLARE @id INT
DECLARE @tempDBName NVARCHAR(255)

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CommandLog' AND TABLE_SCHEMA = 'dbo')
BEGIN
	CREATE TABLE [dbo].[CommandLog](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[DatabaseName] [sysname] NULL,
		[SchemaName] [sysname] NULL,
		[ObjectName] [sysname] NULL,
		[ObjectType] [char](2) NULL,
		[IndexName] [sysname] NULL,
		[IndexType] [tinyint] NULL,
		[StatisticsName] [sysname] NULL,
		[PartitionNumber] [int] NULL,
		[ExtendedInfo] [xml] NULL,
		[Command] [nvarchar](max) NOT NULL,
		[CommandType] [nvarchar](60) NOT NULL,
		[StartTime] [datetime] NOT NULL,
		[EndTime] [datetime] NULL,
		[ErrorNumber] [int] NULL,
		[ErrorMessage] [nvarchar](max) NULL,
	 CONSTRAINT [PK_CommandLog] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

--DROP TABLE #QoS_DBInfo
CREATE TABLE #QoS_DBInfo
(
	TID  INT  IDENTITY ( 1 , 1 ),Primary Key (TID),
	ParentObject nVARCHAR(255),
	[Object] nVARCHAR(255),
	Field nVARCHAR(255) ,
	[Value] VARCHAR(255)
)

--DROP TABLE #QoS_Databases
CREATE TABLE #QoS_Databases
(
	DatabaseID INT IDENTITY (1, 1), Primary Key(DatabaseID),
	DBName nVARCHAR(255) NOT NULL
)

INSERT INTO #QoS_Databases (DBName)
SELECT Name
FROM sys.databases
WHERE name = 'EDDS' OR name like 'EDDS[0-9]%'

WHILE EXISTS (SELECT TOP 1 1 FROM #QoS_Databases)
BEGIN
	SELECT TOP 1
		@id = DatabaseID,
		@tempDBName = DBName
	FROM #QoS_Databases

	INSERT INTO #QoS_DBInfo
	EXECUTE('DBCC DBINFO([' + @tempDBName + ']) WITH TABLERESULTS, NO_INFOMSGS') 

	INSERT INTO dbo.CommandLog
	(DatabaseName, Command, CommandType, StartTime, EndTime, ErrorNumber)
	SELECT TOP 1
		@tempDBName,
		'DBCC CHECKDB ([' + @tempDBName + ']) WITH NO_INFOMSGS, ALL_ERRORMSGS, PHYSICAL_ONLY',
		'DBCC_CHECKDB',	
		Value,
		Value,
		0
	FROM #QoS_DBInfo
	WHERE Field = 'dbi_dbccLastKnownGood'
	AND Value IS NOT NULL

	TRUNCATE TABLE #QoS_DBInfo

	DELETE FROM #QoS_Databases
	WHERE DatabaseID = @id;
END

DROP TABLE #QoS_DBInfo
DROP TABLE #QoS_Databases