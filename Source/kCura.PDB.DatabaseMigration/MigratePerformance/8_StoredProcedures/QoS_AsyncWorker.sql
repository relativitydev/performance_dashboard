USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = N'QoS_AsyncWorker' and type = 'P')
BEGIN
  DROP PROCEDURE eddsdbo.QoS_AsyncWorker 
END
GO

CREATE PROCEDURE eddsdbo.QoS_AsyncWorker
@SQL varchar(max) = ''
AS

DECLARE @InitDlgHandle UNIQUEIDENTIFIER,
	@RequestMsg xml;

BEGIN TRANSACTION ExecProc;
BEGIN DIALOG @InitDlgHandle
     FROM SERVICE [//QoS/InitService]
     TO SERVICE N'//QoS/TargetService'
     ON CONTRACT [//QoS/MSGContract]
     WITH ENCRYPTION = OFF, LIFETIME = 60;
 
 SELECT @RequestMsg = CAST(@SQL as XML);
       
--Send the Message
SEND ON CONVERSATION @InitDlgHandle
	MESSAGE TYPE [//QoS/RequestMessage](@RequestMsg);

COMMIT TRANSACTION ExecProc;
GO
