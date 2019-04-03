USE EDDS;

INSERT INTO eddsdbo.[Configuration] (Section, [Name], MachineName, [Value])
VALUES(@Section, @Name, @MachineName, @Value)