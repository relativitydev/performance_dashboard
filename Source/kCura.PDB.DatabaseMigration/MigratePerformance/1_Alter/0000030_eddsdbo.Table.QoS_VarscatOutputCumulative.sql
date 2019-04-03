USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_VarscatOutputCumulative' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_VarscatOutputCumulative (
		kVOID  INT    IDENTITY ( 1 , 1 ),Primary Key (kVOID),
		ServerName nvarchar (150),
		QoSHourID BIGINT,
		GlassRunID INT,
		DatabaseName nvarchar(128),
		SearchName nvarchar(max),
		SearchArtifactID INT,
		isChild bit,
		isCLRQ bit,
		CreatedBy nvarchar(200),
		DateCreated datetime,
		totalSearchComplexityScore INT,
		LongestRunTime INT,
		ShortestRunTime INT,
		TotalLRQRunTime INT,
		QTYLikeOperators INT,
		QTYSubSearches INT,
		TotalQTYSubSearches INT,
		[QTY Select folders] INT,
		SearchTextLength INT,
		[SearchText || Conditions] nvarchar(max),
		TotalRuns INT,
		MaxRunsBySingleUser INT,
		MaxRunsUser varchar(200),
		MaxRunsUserArtifactID INT,
		RelationalItemsIncludedInCurrentVersion varchar(50),
		ParsedSearchText nvarchar(max),
		[total Bytes-FTC(conditions)] INT,
		SearchType varchar(50),
		[total words all conditions] INT,
		[Total Words in Search Conditions Search Text Box] INT,
		dtSearchTextLength INT,
		QTYOrderBy INT,
		QTYOrderByIndexed INT,
		SearchFieldTypes varchar(500),
		OrderedByFieldTypes varchar(500),
		SearchFieldNames nvarchar(max),
		QTYIndexedSearchFields INT,
		TotalUniqueSubSearches INT,
		LastQueryForm nvarchar(max),
		LongestRunningQueryForm nvarchar(max),
		SummaryDayHour Datetime,
		NumCancelled INT,
		NumErrored INT
	)
END
GO