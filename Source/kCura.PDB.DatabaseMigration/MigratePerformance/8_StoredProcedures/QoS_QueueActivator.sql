USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = N'QoS_QueueActivator' AND type = 'P')
BEGIN
 DROP PROCEDURE eddsdbo.QoS_QueueActivator 
END
GO

CREATE PROCEDURE eddsdbo.QoS_QueueActivator
AS
  DECLARE @RecvReqDlgHandle UNIQUEIDENTIFIER;
  DECLARE @RecvReqMsg xml;
  DECLARE @RecvReqMsgName sysname;

  WHILE (1=1)
  BEGIN

    BEGIN TRANSACTION GetTask;

    WAITFOR
    ( RECEIVE TOP(1)
        @RecvReqDlgHandle = conversation_handle,
        @RecvReqMsg = message_body,
        @RecvReqMsgName = message_type_name
      FROM eddsdbo.QoS_TaskQueue
    ), TIMEOUT 5000;

    IF (@@ROWCOUNT = 0)
    BEGIN
		ROLLBACK TRANSACTION GetTask;
		BREAK;
    END
        
    IF @@TRANCOUNT > 0
		COMMIT TRANSACTION GetTask;
     
    DECLARE @SQL varchar(max) = CAST(@RecvReqMsg AS varchar(max))
    
    IF LEN(@SQL) > 0
    BEGIN
		--Message received, end initiator's side of the conversation
		END CONVERSATION @RecvReqDlgHandle;
		
		PRINT '[Command] ' + @SQL;
		EXEC(@SQL)
    END
    ELSE IF @RecvReqMsgName =
        N'http://schemas.microsoft.com/SQL/ServiceBroker/EndDialog'
    BEGIN
		END CONVERSATION @RecvReqDlgHandle;
    END
    ELSE IF @RecvReqMsgName =
        N'http://schemas.microsoft.com/SQL/ServiceBroker/Error'
    BEGIN
		RAISERROR(N'[Error] %s', 10, 1, @SQL);
		END CONVERSATION @RecvReqDlgHandle;
    END

  END
  
GO

IF EXISTS (SELECT * FROM sys.services WHERE name = N'//QoS/TargetService')
BEGIN
     DROP SERVICE [//QoS/TargetService];
END

IF EXISTS (SELECT * FROM sys.services WHERE name = N'//QoS/InitService')
BEGIN
     DROP SERVICE [//QoS/InitService];
END
        
IF EXISTS (SELECT * FROM sys.service_queues WHERE name = N'QoS_TaskQueue')
BEGIN
     DROP QUEUE [eddsdbo].[QoS_TaskQueue];
END     
     
IF EXISTS (SELECT * FROM sys.service_contracts WHERE name = N'//QoS/MSGContract')
BEGIN
     DROP CONTRACT [//QoS/MSGContract];
END

IF EXISTS (SELECT * FROM sys.service_message_types WHERE name = N'//QoS/RequestMessage')
BEGIN
     DROP MESSAGE TYPE [//QoS/RequestMessage];
END

IF EXISTS (SELECT * FROM sys.service_message_types WHERE name = N'//QoS/ReplyMessage')
BEGIN        
     DROP MESSAGE TYPE [//QoS/ReplyMessage];
END

CREATE MESSAGE TYPE [//QoS/RequestMessage] VALIDATION=WELL_FORMED_XML; 
CREATE MESSAGE TYPE [//QoS/ReplyMessage] VALIDATION=WELL_FORMED_XML;

CREATE CONTRACT [//QoS/MSGContract]
(
 [//QoS/RequestMessage] SENT BY INITIATOR,
 [//QoS/ReplyMessage] SENT BY TARGET 
);  

CREATE QUEUE eddsdbo.QoS_TaskQueue
 WITH STATUS=ON,
 ACTIVATION (
  STATUS = ON,
  PROCEDURE_NAME = eddsdbo.QoS_QueueActivator,
  MAX_QUEUE_READERS = 5,
  EXECUTE AS OWNER
  );

CREATE SERVICE [//QoS/TargetService] ON QUEUE eddsdbo.QoS_TaskQueue([//QoS/MSGContract]);
CREATE SERVICE [//QoS/InitService] ON QUEUE eddsdbo.QoS_TaskQueue([//QoS/MSGContract]);