USE EDDSPerformance
INSERT INTO [EDDSPerformance].[eddsdbo].[ConfigurationAudit] ([FieldName], [ServerName], [OldValue], [NewValue], [UserID], [CreatedOn])
VALUES(@FieldName, @ServerName, @OldValue, @NewValue, @UserID, @CreatedOn)