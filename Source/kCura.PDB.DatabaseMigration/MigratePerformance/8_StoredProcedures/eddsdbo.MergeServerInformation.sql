USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'MergeServerInformation', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO

ALTER PROCEDURE   [eddsdbo].[MergeServerInformation] 
(
	@XMLServerList XML = ''
)
 
AS  
BEGIN
	/*
	
	We're distincting on {ArtifactID, IP, Type}. The XML is considered a snapshot of current environment's servers and server table needs to updated to match that state.
	ArtifactID is not considered unique since multiple servers can have -1 as ArtifactID
	
	* if {ArtifactID, IP, Type} from XML doesnt exist in table than insert
		unless if {ArtifactID,Type} exits in table then update server with IP, type -- where artifactid != -1
	* if {ArtifactID, IP, Type} from table doesnt exist in XML than delete
		unless if {ArtifactID,Type} exits in XML then update server with IP, type. this should be done by the above logic -- where artifactid != -1
	* if {ArtifactID, IP, Type} exists in both XML and table do nothing?
	*/


	DECLARE @Server TABLE( ServerName nvarchar(255), ServerIPAddress nvarchar(100), ServerTypeID INT, ArtifactID INT NULL)

	INSERT INTO @Server 
	SELECT DISTINCT
		item.value('@Name','nvarchar(255)') AS ServerName,
		item.value('@IP','nvarchar(100)') AS ServerIPAddress,
		item.value('@TypeID','INT') AS ServerTypeID,
		item.value('@ArtifactID','INT') AS ArtifactID
	FROM 
		@XMLServerList.nodes('/ServerList/Server') d(item)

	/*
		Server is NOT new and needs to have IP address updated
		
		if server {ArtifactID, Type} exists in both XML and server table and DeletedOn is null
		then update ipaddress from XML
	*/
	UPDATE SS
	SET 
		SS.ServerIPAddress = S.ServerIPAddress,
		SS.ServerName = S.ServerName
	FROM [eddsdbo].[Server] SS
	INNER JOIN @Server S
	ON SS.ArtifactID = S.ArtifactID
	and SS.ServerTypeID = S.ServerTypeID
	--and SS.ServerIPAddress = S.ServerIPAddress
	and DeletedOn is null
	and SS.ArtifactID <> -1
	
	/*
		Server is new and needs to have a new record
		
		if server {ArtifactID, IP, Type} from XML doesn't exist in server table then insert xml record
	*/
	INSERT INTO [eddsdbo].[Server] (ServerName, CreatedOn, DeletedOn, ServerTypeID, ServerIPAddress, ArtifactID) 
	SELECT s.ServerName , GETUTCDATE(), null, s.ServerTypeID , s.ServerIPAddress, s.ArtifactID 
	FROM @Server as S
	where not exists (select 1 from [eddsdbo].[Server] as ss where ss.DeletedOn is null 
		and SS.ArtifactID = S.ArtifactID and SS.ServerIPAddress = S.ServerIPAddress and SS.ServerTypeID = S.ServerTypeID)
	
	/*
		Server doesn't exist anymore and needs to be 'deleted'
		
		if server {ArtifactID, IP, Type} from server table doesn't exist in XML then 'delete' server table record
	*/
	UPDATE S
	set deletedon = GETUTCDATE()
	from [eddsdbo].[Server] S
	WHERE 
	S.DeletedOn is null
	and not exists (select 1 from @Server as ss where 
		SS.ArtifactID = S.ArtifactID and SS.ServerIPAddress = S.ServerIPAddress and SS.ServerTypeID = S.ServerTypeID)
	
	
END

