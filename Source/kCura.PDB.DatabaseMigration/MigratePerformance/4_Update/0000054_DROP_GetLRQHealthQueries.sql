USE [EDDSPerformance]
GO

/****** Object:  UserDefinedFunction [eddsdbo].[GetLRQHealthQueries]    Script Date: 07/29/2014 09:05:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[GetLRQHealthQueries]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [eddsdbo].[GetLRQHealthQueries]
GO