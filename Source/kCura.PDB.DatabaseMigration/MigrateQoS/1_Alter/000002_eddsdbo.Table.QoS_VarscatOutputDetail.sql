USE EDDSQoS
GO

IF NOT EXISTS ( SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetail' AND TABLE_SCHEMA = 'EDDSDBO' )
BEGIN  
    CREATE TABLE eddsdbo.QoS_VarscatOutputDetail
        (
          VODID INT
            IDENTITY(1, 1)
            PRIMARY KEY ( VODID ) ,
          ServerName NVARCHAR(255) ,
          QoSHourID BIGINT ,
          DatabaseName NVARCHAR(128) ,
          SearchArtifactID INT,
          SearchName NVARCHAR(max),
          AuditID INT ,
          UserID INT ,
          ComplexityScore INT ,
          ExecutionTime INT ,--(rounded to nearest second unless it is less than 1, then it gets set to 1.)
          [Timestamp] DATETIME ,
          Bound INT ,
          Split INT ,
          Finish INT ,
          QoSAction INT ,
			--1	View
			--3	Update
			--281	Document Query (from audit action 28 - SELECT TOP)
			--282	Document Query (from audit action 28 - SELECT COUNT)
			--29	Query
			--32	Import
			--33	Export
			--34	ReportQuery
			--35	RelativityScriptExecution
			--47	RDC Imports
			--456 	Mass Actions (actions 4, 5, and 6 from AuditAction table combine to QoSAction 456)
          IsCount BIT ,
          IsComplex BIT ,
          IsLongRunning BIT ,
          IsErrored INT ,
          AgentID INT
        )

END