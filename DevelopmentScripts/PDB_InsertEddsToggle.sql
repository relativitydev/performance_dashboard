USE EDDS;

IF NOT EXISTS(SELECT TOP 1 1 FROM eddsdbo.[Toggle] WHERE [Name] = @Name)
INSERT INTO eddsdbo.[Toggle] ([Name], [IsEnabled])
VALUES(@Name, @Value)