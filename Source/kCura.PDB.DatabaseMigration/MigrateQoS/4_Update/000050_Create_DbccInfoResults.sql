USE [EDDSQoS]

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'DbccInfoResults')
BEGIN
	CREATE TABLE [eddsdbo].[DbccInfoResults](
		[ParentObject] [nvarchar](255) NULL,
		[Object] [nvarchar](255) NULL,
		[Field] [nvarchar](255) NULL,
		[Value] [varchar](255) NULL)
END