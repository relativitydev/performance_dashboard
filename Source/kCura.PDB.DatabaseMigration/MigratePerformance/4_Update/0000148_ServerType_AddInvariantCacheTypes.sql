USE EDDSPerformance
GO

SET IDENTITY_INSERT [eddsdbo].[ServerType] ON
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (23, N'Invariant', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (24, N'InvariantWorker', NULL)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (25, N'CacheLocation', NULL)
SET IDENTITY_INSERT [eddsdbo].[ServerType] OFF