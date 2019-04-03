USE EDDSQoS
GO

IF NOT EXISTS ( SELECT TABLE_NAME FROM  EDDSQoS.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutput' AND TABLE_SCHEMA = 'EDDSDBO' )
BEGIN
	CREATE TABLE eddsdbo.QoS_VarscatOutput
	(
	  kVOID INT IDENTITY(1, 1) ,
	  PRIMARY KEY ( kVOID ) ,
	  AgentID INT ,
	  QoSHourID BIGINT ,
	  DatabaseName NVARCHAR(128) ,
	  SearchName NVARCHAR(MAX) ,
	  SearchArtifactID INT ,
	  isChild BIT ,
	  isCLRQ BIT ,
	  CreatedBy NVARCHAR(200) ,
	  DateCreated DATETIME ,
	  totalSearchComplexityScore  INT ,
	  LongestRunTime INT ,
	  ShortestRunTime INT ,
	  TotalLRQRunTime INT ,
	  QTYLikeOperators INT ,
	  QTYSubSearches INT ,
	  TotalQTYSubSearches INT ,
	  [QTY Select folders] INT ,
	  SearchTextLength INT ,
	  [SearchText || Conditions] NVARCHAR(MAX) ,
	  TotalRuns INT ,
	  MaxRunsBySingleUser INT ,
	  MaxRunsUser VARCHAR(200) ,
	  MaxRunsUserArtifactID INT ,
	  RelationalItemsIncludedInCurrentVersion VARCHAR(50) ,
	  ParsedSearchText   NVARCHAR(MAX) ,
	  [total Bytes-FTC(conditions)]  INT ,
	  SearchType VARCHAR(50) ,
	  [total words all conditions] INT ,
	  [Total Words in Search Conditions Search Text Box]  INT ,
	  dtSearchTextLength INT ,
	  QTYOrderBy INT ,
	  QTYOrderByIndexed INT ,
	  SearchFieldTypes  VARCHAR(500) ,
	  OrderedByFieldTypes VARCHAR(500) ,
	  SearchFieldNames    NVARCHAR(MAX) ,
	  QTYIndexedSearchFields INT ,
	  TotalUniqueSubSearches INT ,
	  LastQueryForm NVARCHAR(MAX) ,
	  LongestRunningQueryForm  NVARCHAR(MAX) ,
	  SummaryDayHour DATETIME ,
	  NumCancelled INT ,
	  NumErrored INT
	)
END