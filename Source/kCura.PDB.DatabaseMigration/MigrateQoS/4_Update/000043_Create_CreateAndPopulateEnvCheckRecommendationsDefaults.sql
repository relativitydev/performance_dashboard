USE [EDDSQoS]
GO

IF NOT EXISTS (select 1 from sysobjects where [name] = 'EnvironmentCheckRecommendationsDefaults' and type = 'U')  
BEGIN
	CREATE TABLE [eddsdbo].[EnvironmentCheckRecommendationsDefaults](
	[ID] [uniqueidentifier] NOT NULL,
	[Scope] [varchar](200) NOT NULL,
	[Severity] [int] NULL,
	[Status] [varchar](50) NULL,
	[Section] [varchar](200) NULL,
	[Name] [varchar](200) NULL,
	[Description] [varchar](max) NULL,
	[Recommendation] [varchar](max) NULL,
	--[BusinessHours] [bit] NULL,
	[BusinessHours] [varchar](50) NULL,
	[DefaultValue] [varchar](max) NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	

	---------------------------------------
	---------------------------------------
	--		 Insert default values       --
	---------------------------------------
	---------------------------------------

	  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('a79514d8-c6f5-433d-a2d3-2c46458b9415','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','AdvancedSearchDefault','Determins the default setting for Advanced Search Default when creating new users. ?False? is private, ?true? Is public','<DEFAULT>','Y','false')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b50f8ac0-9059-4e40-b51e-65bf9f0da7bf','SQL',88,'Critical','SQL Configuration','affinity64 mask','affinity64 mask','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly, This value should never be set to anything other than <KIEVALUE>. Please use sp_configure to remedy this.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('8b1510b9-f50e-4eee-b2a2-2dd786a36d30','SQL',0,'Good','SQL Configuration','affinity64 mask','affinity64 mask','No change needed. This value should never be changed.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('f4b48f1f-b531-4bba-adad-a992e5434cc3','Relativity',50,'Warning','kCura.EDDS.Agents','AgentOffHourEndTime','This works in conjunction with the AgentOffHourStartTime 
value to specify a valid time range when ?off hour? agents 
should run. Currently, this only applies to the File Deletion 
manager, Case Statistics Manager, and Case Manager. The 
format should hh:mm:ss. Please note that the File Deletion 
manager will immediately stop deleting documents once the 
time of day passes the AgentOffHourEndTime. The Case Relativity | Configuration Table - 8 
Statistics manager and Case Manager will continue to process 
until it has completed, however.  ','Please ensure that your agent off hours window lasts a duration of at least 4 hours and not more than 10 hours, unless it is a unique business requirement for your instance. Please also ensure that this window does not overlap with business hours.','Y - to shorten,  N - to lengthen','5:00:00')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('63cb6880-dcc6-4b01-ace2-e2a251f78542','Relativity',0,'Good','kCura.EDDS.Agents','AgentOffHourEndTime','This works in conjunction with the AgentOffHourStartTime 
value to specify a valid time range when ?off hour? agents 
should run. Currently, this only applies to the File Deletion 
manager, Case Statistics Manager, and Case Manager. The 
format should hh:mm:ss. Please note that the File Deletion 
manager will immediately stop deleting documents once the 
time of day passes the AgentOffHourEndTime. The Case Relativity | Configuration Table - 8 
Statistics manager and Case Manager will continue to process 
until it has completed, however.  ','None','Y - to shorten,  N - to lengthen','5:00:00')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('e5f79406-3064-492c-bd68-bf936ea4ce5c','Relativity',50,'Warning','kCura.EDDS.Agents','AgentOffHourStartTime','This works in conjunction with the AgentOffHourEndTime value 
to specify a valid time range when ?off hour? agents should run. 
Currently, this only applies to the File Deletion manager, Case 
Statistics Manager, and Case Manager. The format should 
hh:mm:ss. ','Please ensure that your agent off hours window lasts a duration of at least 4 hours and not more than 10 hours, unless it is a unique business requirement for your instance. Please also ensure that this window does not overlap with business hours.','Y - to shorten, N - to lengthen','0:00:00')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('c24ca894-5566-4d6e-8777-8bac65c202d7','Relativity',0,'Good','kCura.EDDS.Agents','AgentOffHourStartTime','This works in conjunction with the AgentOffHourEndTime value 
to specify a valid time range when ?off hour? agents should run. 
Currently, this only applies to the File Deletion manager, Case 
Statistics Manager, and Case Manager. The format should 
hh:mm:ss. ','None','Y - to shorten, N - to lengthen','0:00:00')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('a92d1301-0f78-49e7-a37f-45c342b2e935','Relativity',10,'<DEFAULT>','Relativity.Core','AllowBrandingMultiThread','','<DEFAULT>','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('8e573772-f6e8-4677-9974-4a6787e8643e','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','AncestorCountWarningLevel','A warning is displayed in the Security page when the item''s security being changed has more than this number of ancestors.  This warning is displayed because the database will be locked for a substantial period of time.','<DEFAULT>','Y','50000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5d279a91-b3a1-42b6-b702-d463b49d8973','Relativity',50,'Warning','Relativity.Core','AuditCountQueries','For each list that is generated, the system initially runs a ""count"" query to get the total number of records that fulfill the criteria.  Setting this value to TRUE would write the SQL query to the History record.','This value should always be set to ''True'' as required by Performance Dashboard and the Best in Service program.','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('2d61c1d8-4a2c-49db-b2fc-7b2511c7934d','Relativity',0,'Good','Relativity.Core','AuditCountQueries','For each list that is generated, the system initially runs a ""count"" query to get the total number of records that fulfill the criteria.  Setting this value to TRUE would write the SQL query to the History record.','None','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('dc05ca38-2e66-459f-9b98-d8f3bb74f5e9','Relativity',50,'Warning','Relativity.Core','AuditFullQueries','For each list page that is generated, the system initially runs a query to get the data to fill the rows on the current page.  Setting this value to TRUE would write the SQL query to the History record.','This value should always be set to ''True'' as required by Performance Dashboard and the Best in Service program.','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ae4d0bf0-58a5-4f87-bf48-1b97fa358703','Relativity',0,'Good','Relativity.Core','AuditFullQueries','For each list page that is generated, the system initially runs a query to get the data to fill the rows on the current page.  Setting this value to TRUE would write the SQL query to the History record.','None','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('98f4a499-a514-4beb-8521-ee5f82635ed1','Relativity',50,'Warning','Relativity.Core','AuditIdQueries','The system will get a batch of Artifact IDs that fulfill the criteria and are need to populate the current page in the list.  Setting this value to TRUE would write the SQL query to the History record','This value should always be set to ''True'' as required by Performance Dashboard and the Best in Service program.','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('82411e6d-9b8f-418b-b8a5-a0458d8adf2c','Relativity',0,'Good','Relativity.Core','AuditIdQueries','The system will get a batch of Artifact IDs that fulfill the criteria and are need to populate the current page in the list.  Setting this value to TRUE would write the SQL query to the History record','None','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('81edb3ac-6246-44e5-b354-4e060e44746c','Relativity',10,'<DEFAULT>','Relativity.Core','AuthenticationTokenLifetimeInMinutes','When an application other than relativity is authenticated using Relativity?s internal mechanism, a token is received. This token will be valid for this period of time.','<DEFAULT>','Y','5')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('7f242ec3-e094-4391-aca9-894826ff4dd6','Relativity',10,'<DEFAULT>','Relativity.Core','BrandingManagerBatchAmount','The number of files the Branding Manager brands in a single 
batch. ','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ed94515b-c123-4ab6-8a0f-5f8ca0895a84','Relativity',10,'<DEFAULT>','kCura.EDDS.Agents','BrandingManagerBatchAmount','The number of files the Branding Manager brands in a single 
batch. ','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('1bf0a138-a4ab-48d2-a993-64d2cfc52a23','Relativity',10,'<DEFAULT>','Relativity.Core','BrandingRecordCreationBatchSize','The maximum number of records inserted into the BrandingQueue table in a single batch.  This is used by both the Production Manager and when attempting to Resolve Errors in the web. ','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5a8b9a5d-df25-407e-932a-006eb66d79b5','Relativity',10,'<DEFAULT>','kCura.EDDS.Web.Distributed','BufferAcquisitionTimeoutInMilliseconds','The amount of time the Distributed site will wait to acquire a 
free buffer to use for downloading before throwing an error','<DEFAULT>','Y','5000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ad52b62b-4cdc-4869-8efb-8fce3020c73c','Relativity',10,'<DEFAULT>','kCura.EDDS.Web.Distributed','BufferCount','The number of buffers that the Distributed site will allocate to 
servicing download requests.  ','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ed53a41f-a93f-438d-a4d9-572c6cababbb','Relativity',10,'<DEFAULT>','kCura.EDDS.Web.Distributed','BufferSize','The size, in bytes, of each buffer allocated by the Distributed 
site to service download requests. ','<DEFAULT>','Y','81920')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('275b2a68-8ec1-41b4-8896-b3eb62839bd4','Relativity',10,'<DEFAULT>','Relativity.Core','CancelRequestTimeDelay','Determines the length of time (in seconds) a request must run before the cancel request overlay appears.','<DEFAULT>','Y','3')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('d6ebdb41-7183-45bb-9c9f-21e8cba955ad','Relativity',10,'<DEFAULT>','Relativity.Core','ChoiceLimitForUI','For choices and single objects, the maximum number that can be rendered in a checkbox, drop-down, or radio button list before switching to a pop-up picker; 15 is the recommended value.','<DEFAULT>','Y','3')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5e9229fa-2c13-45e0-bfdf-729667720da5','SQL',90,'Critical','SQL Configuration','cost threshold for parallelism','cost threshold for parallelism','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly. This value should usually be set somewhere near 50 in Relativity environments. A lower value may cause excessive parallelization and increased overhead on your CPU for simple queries. You may wish to tune this value by using the plan cache. For more information, see this article: http://sqlblog.com/blogs/jonathan_kehayias/archive/2010/01/19/tuning-cost-threshold-of-parallelism-from-the-plan-cache.aspx. Please use sp_configure to remedy this.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('4922db77-b4b7-433d-ba06-49baf9c14434','SQL',50,'Tuning','SQL Configuration','cost threshold for parallelism','cost threshold for parallelism','This value is currently set to <CURRENTVALUE>. This value should usually be set somewhere near 50 in Relativity environments. A value too low may cause excessive parallelization and increased overhead on your CPU for simple queries. You may wish to tune this value by using the plan cache. For more information, see this article: http://sqlblog.com/blogs/jonathan_kehayias/archive/2010/01/19/tuning-cost-threshold-of-parallelism-from-the-plan-cache.aspx.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b7137b99-6cf4-4b52-9781-02bfbfe20ef8','SQL',90,'Critical','SQL Configuration','cost threshold for parallelism','cost threshold for parallelism','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly. This value should usually be set somewhere near 50 in Relativity environments. A value too high may keep costly queries from gaining the benefits of parallelization. You may wish to tune this value by using the plan cache. For more information, see this article: http://sqlblog.com/blogs/jonathan_kehayias/archive/2010/01/19/tuning-cost-threshold-of-parallelism-from-the-plan-cache.aspx. Please use sp_configure to remedy this.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ab7af983-e30d-455d-a968-9248f5973b15','SQL',50,'Tuning','SQL Configuration','cost threshold for parallelism','cost threshold for parallelism','This value is currently set to <CURRENTVALUE>. This value should usually be set somewhere near 50 in Relativity environments. A value too high may keep costly queries from gaining the benefits of parallelization. You may wish to tune this value by using the plan cache. For more information, see this article: http://sqlblog.com/blogs/jonathan_kehayias/archive/2010/01/19/tuning-cost-threshold-of-parallelism-from-the-plan-cache.aspx.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('4118f3f4-6b5b-4bb3-97ce-33b435ff18c7','SQL',0,'Good','SQL Configuration','cost threshold for parallelism','cost threshold for parallelism','No change needed. This value should never be changed.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('1eba7b80-5ddf-4daf-9696-1e7f8b520fcf','SQL',95,'Critical','SQL Configuration','cross db ownership chaining','Allow cross db ownership chaining','This setting should be set to 0.  If you have databases that require cross-database ownership chaining, the recommended practice is to turn off the cross db ownership chaining option for the instance using sp_configure; then turn on cross-database ownership chaining for individual databases that require it using the ALTER DATABASE statement. ','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('d003a94b-8f77-4756-a618-0b2f743af2f7','SQL',0,'Good','SQL Configuration','cross db ownership chaining','Allow cross db ownership chaining','No change needed.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('2cfa8840-d3f1-42c3-b723-14e29f180cf6','Relativity',80,'Warning','kCura.EDDS.SqlServer','DataDirectory','The path to the data directory used by SQL Server. This value should be relative to the server where SQL Server is installed.','Please ensure that your data, full text, and log directories all sit on separate physical disks.  Also be sure that none of this SQL data resides on the same disk as the OS.','N','***This value is set during Relativity SQL Server deployment***')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('f5e4fdd1-907e-46ad-afc5-986e78c50c5a','Relativity',0,'Good','kCura.EDDS.SqlServer','DataDirectory','The path to the data directory used by SQL Server. This value should be relative to the server where SQL Server is installed.','None','N','***This value is set during Relativity SQL Server deployment***')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('346ecd33-0dab-49ba-a651-c36ef52259a9','Relativity',10,'<DEFAULT>','Relativity.Core','dtSearchBatchSize','','<DEFAULT>','Y','-1')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('bd1e2f73-d295-4ed7-8b59-2d38c2bb2233','Relativity',10,'<DEFAULT>','Relativity.Data','dtSearchQueueBatchSize','','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('a8dd70dd-0bbc-4a3b-99b2-fa7be4354ae0','Relativity',10,'<DEFAULT>','Relativity.Data','dtSearchStreamBufferSize','This is the size, in bytes, of the buffer used to store text when streaming data to dtSearch. Increasing this value may slightly improve build times, but requires the agent server to use additional memory. ','<DEFAULT>','Y','52428800')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('d579541a-03f2-4017-8be9-2730a6aa1c0b','Relativity',10,'<DEFAULT>','Relativity.Data','dtSearchStreamThresholdInBytes','This is used by the dtSearch Index Manager when populating an index. If the size of all the long text fields associated with a document is larger than this value, the text is streamed to dtSearch. Otherwise, the text is sent directly to dtSearch all at once. Increasing this value may slightly improve build times, but will increase the chance of your agent server running out of memory.','<DEFAULT>','Y','1048576')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('d3ed8e84-6927-48cd-9f32-c86b443c7495','Relativity',10,'<DEFAULT>','Relativity.Core','EnableTransactionalImports','It is possible to turn off the SQL transactions that occur on import.  This is dangerous because it can cause invalid records in the database.  It could speed up performance in the database.','<DEFAULT>','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('3b59d86a-838a-4479-a107-8f1574bc1a62','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','FileBrowserMaximumElements','This is the maximum number of files or folders that the directory will display in a file browser.','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5dd091ee-1054-4e72-a5ec-a81d20c64fdb','Relativity',10,'<DEFAULT>','Relativity.Data','FileDeleteChunkSizeOnDocumentDelete','Determines the number of records that are deleted at once 
from associated tables when documents are deleted in mass. ','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5ee364c9-cfb2-40ae-b7ad-97d7b34f478a','Relativity',10,'<DEFAULT>','kCura.EDDS.Agents','FileDeletionManagerBatchAmount','The number of documents the File Deletion Manager deletes in 
a single batch. ','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('8d8ab8ae-ae85-4b74-afcc-f0c83c1eb3c8','Relativity',10,'<DEFAULT>','Relativity.Data','FileDeletionManagerBatchAmount','The number of documents the File Deletion Manager deletes in 
a single batch. ','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('72970751-a4b9-414a-a097-23903e9ced65','Relativity',80,'Warning','kCura.EDDS.SqlServer','FTDirectory','The path to the full text directory used by SQL Server. This value should be relative to the server where SQL Server is installed.','Please ensure that your data, full text, and log directories all sit on separate physical disks.  Also be sure that none of this SQL data resides on the same disk as the OS.','N','***This value is set during Relativity SQL Server deployment***')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('e9bbb9cc-f2a1-4975-9c88-bed7de4465ce','Relativity',0,'Good','kCura.EDDS.SqlServer','FTDirectory','The path to the full text directory used by SQL Server. This value should be relative to the server where SQL Server is installed.','None','N','***This value is set during Relativity SQL Server deployment***')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('83795ca3-c151-4cc8-a359-6e6a2e37dee1','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','ImageViewerBufferSize','The maximum number of bytes that the ImageViewer will 
preload into memory. ','<DEFAULT>','Y','10485760')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('481f0306-d691-4bc4-99f0-db834c324221','SQL',50,'Tuning','SQL Configuration','index create memory (KB)','Memory for index create sorts (kBytes)','This value is currently set to <CURRENTVALUE>. This may not be the optimal configuration. This setting should be set to the recommended value if it is to be set at all. You may wish to consider setting this value to the recommended value of <KIEVALUE> KB to improve performance of index builds.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('861952e8-ce31-4347-acbf-edc61c0dc4a9','SQL',0,'Good','SQL Configuration','index create memory (KB)','Memory for index create sorts (kBytes)','This value is currently set to 0, which means that SQL will manage this. This value should be set to the recommended value if it is to be set at all.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('81e8e25d-b348-4a64-8ad0-c67f30621dee','SQL',0,'Good','SQL Configuration','index create memory (KB)','Memory for index create sorts (kBytes)','This value is currently set to <CURRENTVALUE>. This value should be kept as is or set to 0 for SQL to manage it.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('e3c48460-0ae1-4051-9edf-790577a372a0','SQL',81,'Critical','SQL Configuration','index create memory (KB)','Memory for index create sorts (kBytes)','This value is currently set to <CURRENTVALUE> KB, which is less than the Minimum memory per query configuration value of <OTHER1> KB. This may cause undesirable performance because SQL server attempts to create an index using less than <OTHER1> KB. Keep the min memory per query configuration option at a lower number so the index creation job still starts even if all the requested memory is not available. The Index Create Memory value should be set to some value greater than or equal to <OTHER1>. After repairing this, please run this script again for additional recommendations.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b465130f-0784-4c0d-886c-c6e943824405','Windows/SQL',97,'Critical','SQL Configuration','Instant File Initialization','This permission improves file allocation performance by preventing SQL Server from zeroing out space on disk when allocated.','This setting is currently disabled. Enabling instant file initialization is recommended for improvements in file allocation performance. However, this introduces a small security risk in that it may be possible to read previously deleted data. Weight the cost and benefits in your environment to determine whether to enable instant file initialization.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('bd3b8f3a-a27a-46e3-a452-190132db91a3','Windows/SQL',0,'Good','SQL Configuration','Instant File Initialization','This permission improves file allocation performance by preventing SQL Server from zeroing out space on disk when allocated.','This setting is currently disabled. Enabling instant file initialization is recommended for improvements in file allocation performance. However, this introduces a small security risk in that it may be possible to read previously deleted data. Weight the cost and benefits in your environment to determine whether to enable instant file initialization.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('53a94a36-c76e-4d60-b88c-fc4fe15de28d','Relativity',80,'Warning','kCura.EDDS.SqlServer','LDFDirectory','The path to the log file directory used by SQL Server. This value should be relative to the server where SQL Server is installed.','Please ensure that your data, full text, and log directories all sit on separate physical disks.  Also be sure that none of this SQL data resides on the same disk as the OS.','N','***This value is set during Relativity SQL Server deployment***')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('fc92745d-2344-4350-86a1-c8d96c92feb0','Relativity',0,'Good','kCura.EDDS.SqlServer','LDFDirectory','The path to the log file directory used by SQL Server. This value should be relative to the server where SQL Server is installed.','None','N','***This value is set during Relativity SQL Server deployment***')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('65faaff3-4711-4052-b825-343764ff9a6b','SQL',95,'Critical','SQL Configuration','lightweight pooling','User mode scheduler uses lightweight pooling','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly,  This value should never be set to anything other than <KIEVALUE>. Please use sp_configure to remedy this.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5c727566-3acc-4689-b7c0-f39aab00ccdb','SQL',0,'Good','SQL Configuration','lightweight pooling','User mode scheduler uses lightweight pooling','No change needed.  This value should never be changed.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('9a53a0da-47bb-4e4f-bfd1-fc1a7f8dd90e','Relativity',10,'<DEFAULT>','kCura.Data.RowDataGateway','LongRunningQueryTimeout','This is used in various places in the system to time out queries 
that may run for a significant length of time, and potentially lock 
an associated table. ','<DEFAULT>','Y','1200')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('79c77ed3-ab5d-4ef5-94e8-bdbab8a057eb','Relativity',10,'<DEFAULT>','Relativity.Data','LongRunningQueryTimeout','This is used in various places in the system to time out queries 
that may run for a significant length of time, and potentially lock 
an associated table. ','<DEFAULT>','Y','1200')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('e5690250-27ad-4c34-ba3e-ad9f5a74a6c0','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MassCopyBatchAmount','Defaults to the currently configured value for MassDeleteBatchAmount, controls the batch amount on a dynamic object copy','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('79696f8c-072c-4ad6-aa0d-870b41280b0f','Relativity',10,'<DEFAULT>','kCura.Relativity','MassCreateBatchSize','The number of artifactss per batch for the mass create process of the API.','<DEFAULT>','Y','500')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('92e0e10a-402c-4cb9-a555-6ee565c619bb','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MassDeleteBatchAmount','The number of documents per batch for the mass delete 
process. ','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('22b90c03-0fe1-4e42-8e2e-3237df4d5679','Relativity',10,'<DEFAULT>','kCura.Relativity','MassDeleteBatchAmount','The number of documents per batch for the mass delete process. ','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('9f0b65ff-d74b-4cfc-8d6d-d1283bdca352','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MassEditBatchAmount','The number of documents per batch for the mass edit process. ','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5d2ff4e1-0fac-4faa-8176-de87be02fd32','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MassExportBatchAmount','The number of documents per batch for the mass export 
process. ','<DEFAULT>','Y','10000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ab37f92b-bb91-4af6-81fa-63ce46db1bc9','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MassMoveBatchAmount','The number of documents per batch for the mass move
process. ','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('3df3e233-d6d0-4edf-b09f-9c91ab412c26','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MassProduceBatchAmount','The number of documents per batch for the mass produce 
process. ','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('99ca8350-7bcc-411c-a0b0-eade691bdd90','SQL',90,'Critical','SQL Configuration','max degree of parallelism','maximum degree of parallelism','This value is currently set to <CURRENTVALUE>. This value may cause Severes as it allows SQL to spin queries across too many CPU cores.  This value should be changed to: <KIEVALUE>. Please use sp_configure to remedy this.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('e682ce1f-b80b-4fbd-b399-a258215be7e9','SQL',50,'Tuning','SQL Configuration','max degree of parallelism','maximum degree of parallelism','This value is currently set to <CURRENTVALUE>. You may see better performance by increasing this value to the recommended setting so that queries can be further parallelized.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('2469c191-46ae-4dac-bbbe-ccd871678cf3','SQL',90,'Critical','SQL Configuration','max degree of parallelism','maximum degree of parallelism','This value is currently set to <CURRENTVALUE>. You may see better performance by increasing this value to the recommended setting so that queries can be further parallelized.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('6eda7b8d-7f4f-4e22-bf82-420219d1bb64','SQL',0,'Good','SQL Configuration','max degree of parallelism','maximum degree of parallelism','No change needed.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ad0f38b8-96e4-4aca-b2ad-c3f0242c5523','SQL',99,'Critical','SQL Configuration','Max Server Memory (MB)','Maximum size of server memory (MB)','This value is currently set to <CURRENTVALUE>. Your SQL server does not comply with Relativity minimum memory requirements!','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('35a8fe79-5159-4bed-8f0f-c9128632cf0d','SQL',80,'Warning','SQL Configuration','Max Server Memory (MB)','Maximum size of server memory (MB)','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly, it should be set to within 4GB or 10% of total memory, whichever yields a smaller value for max server memory. The current memory committed is less than the recommended value, but this could cause an issue.  We recommend setting this to <KIEVALUE> ','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('4a457915-a881-4a82-9a54-9634b51b70d4','SQL',99,'Critical','SQL Configuration','Max Server Memory (MB)','Maximum size of server memory (MB)','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly, it should be set to within 4GB or 10% of total memory, whichever yields a smaller value for max server memory.  The current memory committed is above the recommended value and can be causing performance issues.  We recommend setting this to <KIEVALUE>   ','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ef9698c1-68e6-4483-9e21-86bdd4ae4579','SQL',80,'Warning','SQL Configuration','Max Server Memory (MB)','Maximum size of server memory (MB)','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly; it should be set to within 4GB or 10-20% of total memory, whichever yields a smaller value for max server memory.  You may see an increase in performance by raising this to the recommended value of <KIEVALUE> ','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('c1e546c1-6f91-4572-9c99-7d88f3f26319','SQL',0,'Good','SQL Configuration','Max Server Memory (MB)','Maximum size of server memory (MB)','No change needed.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('3c15f773-493a-402d-a3dd-241517c4c19d','SQL',0,'Good','SQL Configuration','max worker threads','Maximum worker threads','This setting is normally set to 0 to allow SQL server to automatically determine the correct number of worker threads based on user requests. Currently, it is set to 0, and the actual number of worker threads is less than the recommended threshold of <KIEVALUE>','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('c8d96bf0-c955-4cde-88e1-cb0a8a89211a','SQL',81,'Critical','SQL Configuration','max worker threads','Maximum worker threads','This setting is normally set to 0 to allow SQL server to automatically determine the correct number of worker threads based on user requests. However, you currently have more worker threads than the recommended threshold of <KIEVALUE>. Setting this sys.configurations value to the value recommended by kCura may improve performance.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('9ea8b538-e605-4a64-b1c4-1801f4ae54d8','SQL',75,'Warning','SQL Configuration','max worker threads','Maximum worker threads','This setting is normally set to 0 to allow SQL server to automatically determine the correct number of worker threads based on user requests. The current value is set to <CURRENTVALUE> which is less than the kCura recommendation. At the moment, worker threads are less than the configured value, but this could create a potential performance issue in the future. Please set it to either 0 or the value recommended by kCura, and run this script again during a time of heavy load to see if this recommendation changes.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('17bfdd92-ada0-421b-a9c7-252468df49a7','SQL',81,'Critical','SQL Configuration','max worker threads','Maximum worker threads','This setting is normally set to 0 to allow SQL server to automatically determine the correct number of worker threads based on user requests. The current value is set to <CURRENTVALUE> which is greater than the value recommended by kCura. At the moment, worker threads are in excess of the recommended value of <KIEVALUE> because it has been configured to a higher value. This could either be contributing to, or be the cause of, current performance issues. Please set it to either 0 or the recommended value and run again.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('26faa179-78e6-4896-9322-68183ded976b','SQL',75,'Warning','SQL Configuration','max worker threads','Maximum worker threads','This setting is normally set to 0 to allow SQL server to automatically determine the correct number of worker threads based on user requests. Currently, it has been configured to <CURRENTVALUE>. The number of worker threads running on this server (<OTHER1>) is less than the recommended maximum; however, your max worker threads is set to a value greater than the recommended value of <KIEVALUE>','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('eba17f10-555e-42b8-85c7-e2ff0b31b665','SQL',0,'Good','SQL Configuration','max worker threads','Maximum worker threads','No change needed.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('aeba142b-c2c6-4085-8402-bd3c7e047390','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MaximumListPageTextLength','The maximum amount of characters that the itemlist will 
display for a text field.','<DEFAULT>','Y','500')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('e200bb58-246e-4ed7-a83d-923141987275','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','MaximumNativeSizeForViewerInBytes','The maximum size, in bytes, of a native file that the Native 
Viewer will attempt to display.  If a native document is greater 
than this size, the user will instead see a page explaining that 
the document is too large to be displayed. ','<DEFAULT>','Y','10485760')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('2d69a8c4-4e57-48fe-abc2-79e13ffce117','Relativity',10,'<DEFAULT>','Relativity.Core','MaxPDVQueryLength','The timeout value, in seconds, associated with running queries 
related to displaying document lists. ','<DEFAULT>','Y','60')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('01817f23-4c36-4244-b47e-d707188871c6','Relativity',10,'<DEFAULT>','Relativity.Core','MaxQueryConditionCharacterLength','This is the max length of a criterion in the view criteria picker','<DEFAULT>','Y','100000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('dff1fabd-9b64-414f-8010-d5f93c662416','SQL',81,'Critical','SQL Configuration','min memory per query (KB)','minimum memory per query (kBytes)','This value is currently less than the index create memory (KB) configuration value.  This value should be equal to or higher than the index create memory value, as this is the minimum amount of memory that SQL will attempt to use for one query.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b5e3d2cf-dfc4-49a2-81ef-c1c4adf5af08','SQL',0,'Good','SQL Configuration','min memory per query (KB)','minimum memory per query (kBytes)','No change needed.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('aa479fa9-f5d2-4766-a148-822a9cd2d363','SQL',40,'Tuning','SQL Configuration','min memory per query (KB)','minimum memory per query (kBytes)','This value is currently set to <CURRENTVALUE>. You may be able to increase performance by raising this to 2048 or some higher value. Currently, you have <OTHER1> GB memory allocated to SQL server and only <OTHER2>GB committed. Exercise caution whenever making any configuration changes to SQL Server.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('4479210e-b9c5-4acd-b3f6-2ad7ffa45b0a','SQL',0,'Good','SQL Configuration','min server memory (MB)','Minimum size of server memory (MB)','No change needed.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5757f290-0305-45ae-8bf9-65e9d398d598','SQL',88,'Critical','SQL Configuration','min server memory (MB)','Minimum size of server memory (MB)','We advise against setting min server memory for SQL Server. SQL will use as much RAM as it can, thus the need to set max memory to leave room for the OS. If this value is set too high, SQL may not give up RAM that is needed by the OS and this can cause serious problems. There is no need to be concerned with SQL not using available RAM unless your MAX server memory value is set too low.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('af68bd6f-c8b1-478f-9a7f-82c533e66c8b','SQL',0,'Good','SQL Configuration','network Packet Size (B)','Network packet size','If an application does bulk copy operations and sends or receives large amounts of text or image data, adjusting this upwards may improve efficiency as it results in fewer network reads and writes. This works best on networks that support larger packets. In some circumstances, changing the packet size might lead to a communication link failure such as "Native Error: 233, no process is on the other end of the pipe." Do not change this unless you know it will improve performance. Regardless, this should not exceed 8060 Bytes.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('8fe08b80-0cf1-480e-8d2e-a93710fb8d8d','SQL',81,'Critical','SQL Configuration','network Packet Size (B)','Network packet size','This value is currently set to <CURRENTVALUE>, which is in excess of the max value of 8060. If an application does bulk copy operations and sends or receives large amounts of text or image data, adjusting this upwards may improve efficiency as it results in fewer network reads and writes. This works best on networks that support larger packets. In some circumstances, changing the packet size might lead to a communication link failure such as "Native Error: 233, no process is on the other end of the pipe." Regardless, this should not exceed 8060 Bytes. If the network packet size of any logged-in user is more than 8060 bytes, SQL Server performs different memory allocation operations. According to Microsoft, this can cause an increase in the process virtual address space that is not reserved for the buffer pool.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('fce3ac74-0746-417b-b3cb-a818653bfa80','SQL',75,'Warning','SQL Configuration','network Packet Size (B)','Network packet size','This value is currently set to <CURRENTVALUE>, which is less than the default value of 4096. If an application does bulk copy operations and sends or receives large amounts of text or image data, adjusting this upwards may improve efficiency as it results in fewer network reads and writes. This works best on networks that support larger packets. In some circumstances, changing the packet size might lead to a communication link failure such as "Native Error: 233, no process is on the other end of the pipe." Do not change this unless you know it will improve performance. Regardless, this should not exceed 8060 Bytes. If the network packet size of any logged-in user is more than 8060 bytes, SQL Server performs different memory allocation operations. According to Microsoft, this can cause an increase in the process virtual address space that is not reserved for the buffer pool.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('50dd2068-3948-4b49-8ff5-fc698e99c556','SQL',50,'Tuning','SQL Configuration','network Packet Size (B)','Network packet size','This value is currently set to <CURRENTVALUE>, which is greater than the default value of 4096. If an application does bulk copy operations and sends or receives large amounts of text or image data, adjusting this upwards may improve efficiency as it results in fewer network reads and writes. This works best on networks that support larger packets. In some circumstances, changing the packet size might lead to a communication link failure such as "Native Error: 233, no process is on the other end of the pipe." Do not change this unless you know it will improve performance. Regardless, this should not exceed 8060 Bytes. If the network packet size of any logged-in user is more than 8060 bytes, SQL Server performs different memory allocation operations. According to Microsoft, this can cause an increase in the process virtual address space that is not reserved for the buffer pool.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('7c8ed281-c4c7-4e02-8b0e-343c8266b677','Relativity',50,'Warning','N/A','Number of QoS Manager Agents','This is the number of Performance Dashboard - QoS Manager agents that are enabled in this Relativity environment. The QoS Manager is required for Performance Dashboard to generate scores.','There are not enough active QoS Manager agents in your environment. There must always be one QoS Manager agent running.','Y','1')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b2960953-5a7b-42ce-a777-cc53dc7d2261','Relativity',50,'Warning','N/A','Number of QoS Manager Agents','This is the number of Performance Dashboard - QoS Manager agents that are enabled in this Relativity environment. The QoS Manager is required for Performance Dashboard to generate scores.','There are too many QoS Manager agents in your environment. There should always be exactly one QoS Manager agent running. Additional agents will not perform any work.','Y','1')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('16e0576b-6d28-4ca9-833a-8cf18981dc4d','Relativity',0,'Good','N/A','Number of QoS Manager Agents','This is the number of Performance Dashboard - QoS Manager agents that are enabled in this Relativity environment. The QoS Manager is required for Performance Dashboard to generate scores.','No action required.','Y','1')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('0e53d4fc-6a75-4697-8b04-feae0e5e72bc','Relativity',50,'Warning','N/A','Number of QoS Worker Agents','This is the number of Performance Dashboard - QoS Worker agents that are enabled in this Relativity environment. The QoS Worker is required for Performance Dashboard to analyze hours for activity.','There are not enough active QoS Worker agents in your environment. There must always be at least one QoS Worker agent running.','Y','4')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ffb4954e-7b74-413b-8ef8-f110da1c1796','Relativity',50,'Warning','N/A','Number of QoS Worker Agents','This is the number of Performance Dashboard - QoS Worker agents that are enabled in this Relativity environment. The QoS Worker is required for Performance Dashboard to analyze hours for activity.','There may be too many QoS Worker agents in your environment. Reducing the number of QoS Worker agents may decrease load on SQL Server without introducing significant delays in scoring.','Y','4')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('79e95b24-468e-4dda-a572-fc10592e9e3e','Relativity',50,'Warning','N/A','Number of QoS Worker Agents','This is the number of Performance Dashboard - QoS Worker agents that are enabled in this Relativity environment. The QoS Worker is required for Performance Dashboard to analyze hours for activity.','There may not be enough QoS Worker agents in your environment. Performance Dashboard is currently generating scores with a delay of at least ten minutes. Adding QoS Worker agents may accelerate scoring.','Y','4')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('8b8271d0-c987-4fd2-8548-ee4f1010617c','Relativity',0,'Good','N/A','Number of QoS Worker Agents','This is the number of Performance Dashboard - QoS Worker agents that are enabled in this Relativity environment. The QoS Worker is required for Performance Dashboard to analyze hours for activity.','No action required.','Y','4')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('7b8c7d7c-edae-4677-8450-3e269e847235','Relativity',50,'Not Default','N/A','Number of Trust Worker Agents','This is the number of Performance Dashboard - Trust Worker agents that are enabled in this Relativity environment. The Trust Worker is required for Performance Dashboard to send data to Trust.','There are not enough active Trust Worker agents in your environment. There should always be exactly one Trust Manager agent running.','Y','1')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('f1343c4d-e696-439c-9c0e-211bb294756a','Relativity',50,'Not Default','N/A','Number of Trust Worker Agents','This is the number of Performance Dashboard - Trust Worker agents that are enabled in this Relativity environment. The Trust Worker is required for Performance Dashboard to send data to Trust.','There are too many Trust Worker agents in your environment. There should always be exactly one Trust Worker agent running. Additional agents will not perform any work.','Y','1')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('9d7e6c29-084e-4af6-b0ef-626776561a89','Relativity',0,'Good','N/A','Number of Trust Worker Agents','This is the number of Performance Dashboard - Trust Worker agents that are enabled in this Relativity environment. The Trust Worker is required for Performance Dashboard to send data to Trust.','No action required.','Y','1')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('2c1fb60f-701f-49b2-997b-617d44bfb4bb','Relativity',50,'Warning','N/A','Number of WMI Worker Agents','This is the number of Performance Dashboard - WMI Worker agents that are enabled in this Relativity environment. The WMI Worker is required for Performance Dashboard to collect performance metrics for each server.','There are not enough active WMI Worker agents in your environment. There must always be at least one WMI Worker agent running.','Y','4')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('baf830e5-1fd0-44bc-8c8c-87c9981d606e','Relativity',50,'Warning','N/A','Number of WMI Worker Agents','This is the number of Performance Dashboard - WMI Worker agents that are enabled in this Relativity environment. The WMI Worker is required for Performance Dashboard to collect performance metrics for each server.','Some servers are not being checked regularly by Performance Dashboard. There may not be enough WMI Worker agents in your environment, the agents may not be running, or errors may have prevented performance monitoring.','Y','4')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('e5a55396-c85e-40e2-8699-f4477b81ae42','Relativity',0,'Good','N/A','Number of WMI Worker Agents','This is the number of Performance Dashboard - WMI Worker agents that are enabled in this Relativity environment. The WMI Worker is required for Performance Dashboard to collect performance metrics for each server.','No action required.','Y','4')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('382a54a4-c537-4d5c-b5a1-960f8fd82906','Relativity',10,'<DEFAULT>','kCura.EDDS.Agents','OCRCompilationBatchSize','The number of updates to the document table, with the OCRed text, that the OCR Manager will make in a single transaction.','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('3fe5ba65-64d8-46e5-8248-dcb7184d9c65','Relativity',10,'<DEFAULT>','kCura.EDDS.Agents','OCRProcessingBatchSize','This value governs the amount by which items from OCR_POP tables get processed into OCR_TEXT','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('76ba10a3-b87f-4bcf-974f-4848bd7be2dd','Relativity',10,'<DEFAULT>','kCura.EDDS.Agents','OCRWorkerBatchAmount','The number of images a single OCR worker agent will pull and OCR at a time.','<DEFAULT>','Y','100')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('2f4046b9-1449-4ffe-9a4b-9cc682610b9c','Relativity',10,'<DEFAULT>','kCura.EDDS.Agents','OCRWorkerInsertBatchSize','The number of inserts to the OCR Worker table that the OCR Manager agent will batch into a single transaction.','<DEFAULT>','Y','500')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('a064cc47-1c49-4c3b-95c6-28471fc75110','SQL',88,'Critical','SQL Configuration','optimize for ad hoc workloads','When this option is set, plan cache size is further reduced for single-use adhoc OLTP workload.','This value is currently set to <CURRENTVALUE>. Your existing plan cache contains a large amount of space in RAM (prepared MBs:<OTHER1>, adHoc MBs:<OTHER2>) for single use plans. You should consider setting this option to a value of 1 to free up more RAM.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('855d14c2-122c-44e6-b1f7-e860be6e222d','SQL',50,'Tuning','SQL Configuration','optimize for ad hoc workloads','When this option is set, plan cache size is further reduced for single-use adhoc OLTP workload.','This value is currently set to <CURRENTVALUE>. Your existing plan cache may, at some point, grow to contain a large amount of space in RAM for single use plans. You should consider setting this option to the recommended value.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('d8d48787-41de-4ffc-95f0-a22fb4620ad3','SQL',0,'Good','SQL Configuration','optimize for ad hoc workloads','When this option is set, plan cache size is further reduced for single-use adhoc OLTP workload.','No Change needed.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('6b579262-c75e-4473-8d3d-c80f7ab04b1a','Relativity',10,'<DEFAULT>','kCura.Relativity','PDVCacheLifetime','The number of minutes the Performance DataView cache exists for.','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('4df08e01-4fcf-4530-8980-5294da2aaf42','Relativity',10,'<DEFAULT>','kCura.Relativity','PDVDefaultQueryCacheSize','The default number artifacts to be returned during each query cache read.','<DEFAULT>','Y','10000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('8c742afa-ee9b-4f98-88d9-b8c1ecc48c04','SQL',100,'Critical','SQL Configuration','priority boost','Priority boost','This value is currently set to <CURRENTVALUE>. This value should not be set without cause. This value should be set to the recommended value of <KIEVALUE>','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('6ea79ddb-8f08-4acf-beff-6819c3433d3a','SQL',0,'Good','SQL Configuration','priority boost','Priority boost','No change needed. This value should never be changed.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('caa0367d-0bb6-40f7-abf1-1a08e2e122ae','SQL',81,'Critical','SQL Configuration','query wait (s)','maximum time to wait for query memory (s)','This value is currently set to <CURRENTVALUE>. The normal value for this is -1. When set to -1, SQL allows for the query to wait for up to 25 times the query cost before a time-out occurs. You should strongly consider setting this to the default value. If you are experiencing an unusual amount of timeouts, and the duration prior to timeout is variable, you may consider changing this to some value as a temporary measure while you troubleshoot.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('f20a1c05-8f23-4d4e-b286-a7abcb8776ca','SQL',0,'Good','SQL Configuration','query wait (s)','maximum time to wait for query memory (s)','No change needed. This value should only be changed for temporary, troubleshooting reasons.','N','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('affbc853-d852-44d0-979c-78411be8a767','Relativity',50,'Warning','Relativity.Data','QueryCacheMode','Sets the type of caching used by Relativity. Valid options are SQLDependency and TimeBased.','This value should usually be set to ''TimeBased''. In recent versions of Relativity, the database upgrade tool (Procuro) will automatically set this value for you. Please check with kCura Client Services if ''TimeBased'' is not the option you have set.','Y','TimeBased')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('73bdd8f3-496e-43e7-94f0-12a879c34358','Relativity',0,'Good','Relativity.Data','QueryCacheMode','Sets the type of caching used by Relativity. Valid options are SQLDependency and TimeBased.','None','Y','TimeBased')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('bf16f5ec-60c2-4f14-b983-10c3bd40b7e6','SQL',40,'Tuning','SQL Configuration','remote query timeout (s)','remote query timeout','This value is currently set to <CURRENTVALUE>. This value is less than the recommendation of <KIEVALUE>','','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5940e6f8-f93f-4c58-a081-77d8dca38da0','SQL',40,'Tuning','SQL Configuration','remote query timeout (s)','remote query timeout','This value is set higher than the recommended value.','','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('eb37e0f1-a6bb-4942-a0d9-6b8e923c0021','SQL',0,'Good','SQL Configuration','remote query timeout (s)','remote query timeout','No change needed.','','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('c4f963af-ca5d-4dcc-b5b7-b990ca30e3b1','SQL',81,'Critical','SQL Configuration','remote query timeout (s)','remote query timeout','This value is currently set to <CURRENTVALUE>. This value is less than the recommendation of <KIEVALUE> in a non-distributed environment. This could pose a critical problem if certain agents timeout.','','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5a8674f4-f7b1-460d-92f9-a0ab6826d6fc','SQL',40,'Tuning','SQL Configuration','remote query timeout (s)','remote query timeout','This value is currently set to <CURRENTVALUE>. This value is more than the recommendation of <KIEVALUE> in a non-distributed environment.','','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('92bc723b-dfc2-41ac-8ce8-611b1b7bff16','Relativity',10,'<DEFAULT>','Relativity.Data','SearchIndexerLongRunningQueryTimeout','The timeout value, in seconds, associated with long running 
queries run by dtSearch and CA.','<DEFAULT>','Y','1200')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('468f759d-3d19-4ffa-b35b-95363168a300','Relativity',10,'<DEFAULT>','Relativity.Data','SearchIndexerTextFromSQLChunkSizeInBytes','The size of the text chunks streamed from SQL to either the 
dtSearch or CA agents when they are writing large files to disk. 
Increasing this value may improve performance when 
populating large files. The larger this value becomes, the greater 
the chance that either the dtSearch or CA agent will run out of 
memory. ','<DEFAULT>','Y','1048576')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('75e9de81-db8b-450c-8865-70ecdc506608','SQL',81,'Critical','SQL Configuration','set working set size','set working set size','This value is currently set to <CURRENTVALUE>. This value does not appear to be set properly. This value should never be set to anything other than <KIEVALUE>. Please use sp_configure to remedy this.','','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('8abd2c05-95e0-46bf-88d6-1c6c1c38b879','SQL',0,'Good','SQL Configuration','set working set size','set working set size','No change needed. This value should never be changed.','','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b0a57279-b945-4393-9d67-160711498dd2','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','ShowStackTraceOnError','If true, full stack traces will be displayed to users when an error 
occurs. ','<DEFAULT>','Y','true')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('550a324f-eab7-47a2-a9f6-01bb1855ffd8','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','TallyBatchAmount','The number of documents per batch for the tally process.','<DEFAULT>','Y','1000')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('a0551000-c45c-40eb-9b16-6b0f76ac01ed','Relativity',50,'Warning','N/A','Task: X (Process Controls)','The current status of a task in Performance Dashboard. These are managed in the eddsdbo.ProcessControl table in EDDSPerformance.','This task is currently disabled and will not run. This may affect Performance Dashboard score availability. Enable the task in EDDSPerformance.eddsdbo.ProcessControl to resume normal operation.','Y','Enabled')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('16cc163e-f310-48ae-97c3-0a9d8dfb1b65','Relativity',50,'Warning','N/A','Task: X (Process Controls)','The current status of a task in Performance Dashboard. These are managed in the eddsdbo.ProcessControl table in EDDSPerformance.','This task is currently failing or not running. Verify appropriate Performance Dashboard agent setup and that all agents are running. Refer to the event log on agent servers for relevant warnings or errors.','Y','Enabled')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('1a442de2-1f75-4f0b-b685-8c582f5ef878','Relativity',0,'Good','N/A','Task: X (Process Controls)','The current status of a task in Performance Dashboard. These are managed in the eddsdbo.ProcessControl table in EDDSPerformance.','No action required.','Y','Enabled')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('5b3b8dbc-0ef9-4bb9-bcb9-7cee61605278','SQL',95,'Critical','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','The total number of tempdbs on this server: <OTHER1> TempDB is suboptimal and your server is underpowered. Please add additional cores and increase the tempDBs to the recommended value.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('90f27695-c436-487b-8a6e-aaf128e55fc1','SQL',95,'Critical','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are more tempDBs on this server than there are cores:, <OTHER1> TempDB is suboptimal. Please decrease them to match the number of physical cores, and please add additional cores and then adjust to the recommended value.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('c7e707f3-779d-4480-8157-3e6d89ad96c4','SQL',80,'Warning','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are less tempDBs on this server than there are cores: <OTHER1> tempDBs is not enough. Please increase them to match at least <KIEVALUE>. You may wish to increase them even further, but bear in mind that too many tempDBs can cause an IO issue.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('6c32b75d-9d1c-4eba-8a60-58d46e571351','SQL',95,'Critical','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are less cores on this server than there are TempDBs:, <OTHER1> tempDBs is too many. Please decrease them to match at least <OTHER2>. Additionally, this server is underpowered, and does not meet the minimum specifications to run Relativity.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('45e257c4-84c5-40bb-9d70-7d447df77cf2','SQL',50,'Tuning','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are less tempDbs on this server than there are processing cores: <OTHER1> TempDBs. Our recommendation is to start with 8 tempDB data files or one per core, whichever value is smaller. You can opt to add more, up to 1 per core, but often the benefit after going above 8 is little to none. If you are running this script due to performance IO issues, you should consider other areas in this report in tandem with this finding. Your server may have too many tempDBs, or your max degree of parallelization may be too high, or not high enough.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b2e0da01-ad3f-4f07-96aa-063ea891f149','SQL',95,'Critical','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are more tempDBs on this server than there are cores: <OTHER1> tempDBs is too many. Please decrease them to match the number of cores or less. Please consider that <OTHER2> tempDBs could possibly cause IO issues. If you are running this script because you are experiencing IO issues, consider reducing the number of TempDBs even further if you continue to experience issues. kCura''''s recommendation is to start with eight tempDB data files.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('cf9bf7de-aa32-458a-bef1-1a412a4d123b','SQL',0,'Good','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are the correct number of TempDBs on this server: <OTHER1>','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ef44198b-38f6-45a4-92f4-e17b8d160995','SQL',50,'Tuning','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There may be too many tempDBs on this database: <OTHER1>. Consider that, if you are running this script to troubleshoot a performance issue, the number of tempDBs may be too high. Too many tempDBs can cause an IO issue.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('7f0e09b3-61fd-4a08-abf9-08a2b4d61471','SQL',95,'Critical','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are not enough cores or tempDBs on this database: <OTHER1> tempdbs and cores. The recommended minimum cores for a Relativity installation is <KIEVALUE>.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('27da9966-7381-4b8c-b185-bee3d0eaff6f','SQL',80,'Warning','SQL Configuration','Tempdb Check','The value is your current count of TempDBs, as is the value in use.  Minimum is the recommended value, and Maximum is the most you should ever have.  See notes for further analysis.','There are not enough TempDBs on this server: <OTHER1> TempDBs. Please add additional TempDBs to match the recommended value of <KIEVALUE>.','Y','')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('cde8a5f4-e0a2-4f18-a9a3-10c3f2844838','Relativity',50,'Warning','Relativity.Core','UseHashJoin','','This value should almost always be set to ''True''. The hash join query hint can yield signifcant increase in query performance for many document queries in Relativity.','Y','true')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('a2107ce5-fe0d-4231-8153-d445b4b7d7d1','Relativity',0,'Good','Relativity.Core','UseHashJoin','','None','Y','true')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('b00fc3b2-2a87-441e-ae77-b67da27958f7','Relativity',10,'<DEFAULT>','kCura.EDDS.WebAPI','ValidateAssemblyVersion','The value determines which parts of the Relativity version 
number is validated between the desktop client and the Relativity | Configuration Table - 17 
WebAPI version.  For example, given the WebAPI version is 
4.20.1.1 and the desktop client is 4.20.1.2 
o  *.*.*.* will match 
o  x.x.*.* will match 
o  x.x.x.x will not match (the last node between the two is 
different) 
o  x.x.x.* will match 
o  *.*.*.x will not match ','<DEFAULT>','Y','x.*.*.*')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('ed5ac061-d3f5-4ba8-be31-c5cef20049fe','Relativity',10,'<DEFAULT>','Relativity.Core','ViewQueryOptimization','If set to true, this value populates the total item count in an Item List view using one SQL query to get the IDs of all items in the list. This eliminates the need for a second SQL query to get the total count of items.','<DEFAULT>','Y','True')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('375c587c-aac2-4ab2-88d8-f14337f1e16f','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','WebClientNativeViewerCacheAheadMaxSizeInBytes','The maximum size, in bytes, a file is allowed to be in order to 
allow the Relativity Web Client Native Viewer Cache Ahead 
technology to cache it. ','<DEFAULT>','Y','52428800')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('9d91056e-5002-40d8-b370-40d6d6914c39','Relativity',10,'<DEFAULT>','kCura.EDDS.Web','WebClientTimeout','The value, in milliseconds, that the Web Client components will 
wait for a response when making a WebAPI call before 
assuming the request has timed out. ','<DEFAULT>','Y','52428800')

  INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
    ([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
  VALUES
    ('9442d9af-2692-467e-8bd1-03cf26690590','Relativity',10,'<DEFAULT>','kCura.EDDS.Agents','WordIndexMaxWordSize','The largest number of characters of a word when  creating a 
word index. Words larger than this value are truncated. ','<DEFAULT>','Y','200')




end