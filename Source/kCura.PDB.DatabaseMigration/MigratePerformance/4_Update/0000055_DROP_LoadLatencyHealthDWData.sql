USE [EDDSPerformance]
GO

/****** Object:  StoredProcedure [eddsdbo].[LoadLatencyHealthDWData]    Script Date: 07/29/2014 09:06:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[LoadLatencyHealthDWData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[LoadLatencyHealthDWData]
GO