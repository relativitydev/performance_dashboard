IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id
	WHERE st.name = N'RHScriptsRunType' AND ss.name = N'eddsdbo')
CREATE TYPE [eddsdbo].[RHScriptsRunType] AS TABLE(
	[id] [bigint] NOT NULL,
	[text_of_script] [text] NULL,
	[text_hash] [nvarchar](512) NULL
)