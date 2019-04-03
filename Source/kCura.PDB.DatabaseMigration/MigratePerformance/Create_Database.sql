USE [master]
GO

/*=============================CREATE EDDSPERFORMANCE DATABASE============================*/

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EDDSPerformance') BEGIN
	-- Get file path used to store other Relativity databases
	DECLARE @dbFileName nvarchar(MAX) = '{{mdfDir}}EDDSPerformance.mdf',
		@dbFileLogName nvarchar(MAX) = '{{ldfDir}}EDDSPerformance_log.ldf',
		@createScript nvarchar(MAX),
		@createTemplate VARCHAR(MAX),
		@collation nvarchar(MAX) = (
			SELECT 'COLLATE ' + '{{collation}}'
		);
	SET @createTemplate = '
	CREATE DATABASE [EDDSPerformance] ON PRIMARY (
		NAME = N''EDDSPerformance'',
		FILENAME = ''{DbFileName}'',
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 1024MB
	)
	LOG ON (
		NAME = N''EDDSPerformance_log'',
		FILENAME = ''{DbFileLogName}'',
		MAXSIZE = 2048GB,
		FILEGROWTH = 512MB
	) '
	+ ISNULL(@collation, 'COLLATE SQL_Latin1_General_CP1_CI_AS'); --Default to the standard Relativity collation if we can't get this for some reason

	-- Replace tokens for filepaths
	SET @createScript = REPLACE(@createTemplate, '{DbFileName}', @dbFileName)
	SET @createScript = REPLACE(@createScript, '{DbFileLogName}', @dbFileLogName)
	EXECUTE (@createScript)
	
END
GO

/*=============================ALTER EDDSPERFORMANCE DATABASE=============================*/

USE [EDDSPerformance]
declare @createdByValue nvarchar(max);
SELECT @createdByValue = convert(nvarchar(max), value)
FROM [EDDSPerformance].sys.fn_listextendedproperty ( 'EDDSPerformanceCreatedBy', default, default, default, default, default, default)

IF NOT EXISTS (SELECT * FROM [EDDSPerformance].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHVersion')
AND @createdByValue is null
BEGIN
	ALTER DATABASE [EDDSPerformance]
		SET SINGLE_USER
		WITH ROLLBACK IMMEDIATE;
	
	EXEC EDDSPerformance.sys.sp_addextendedproperty @name = 'EDDSPerformanceCreatedBy', @value = 'Roundhouse'
		
	ALTER DATABASE [EDDSPerformance] SET COMPATIBILITY_LEVEL = 100

	IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) BEGIN
		EXEC [EDDSPerformance].[dbo].[sp_fulltext_database] @action = 'enable'
	END

	ALTER DATABASE [EDDSPerformance] SET ANSI_NULL_DEFAULT OFF 
	ALTER DATABASE [EDDSPerformance] SET ANSI_NULLS OFF 
	ALTER DATABASE [EDDSPerformance] SET ANSI_PADDING OFF 
	ALTER DATABASE [EDDSPerformance] SET ANSI_WARNINGS OFF 
	ALTER DATABASE [EDDSPerformance] SET ARITHABORT OFF 
	ALTER DATABASE [EDDSPerformance] SET AUTO_CLOSE OFF 
	ALTER DATABASE [EDDSPerformance] SET AUTO_CREATE_STATISTICS ON 
	ALTER DATABASE [EDDSPerformance] SET AUTO_SHRINK OFF 
	ALTER DATABASE [EDDSPerformance] SET AUTO_UPDATE_STATISTICS ON 
	ALTER DATABASE [EDDSPerformance] SET CURSOR_CLOSE_ON_COMMIT OFF 
	ALTER DATABASE [EDDSPerformance] SET CURSOR_DEFAULT  GLOBAL 
	ALTER DATABASE [EDDSPerformance] SET CONCAT_NULL_YIELDS_NULL OFF 
	ALTER DATABASE [EDDSPerformance] SET NUMERIC_ROUNDABORT OFF 
	ALTER DATABASE [EDDSPerformance] SET QUOTED_IDENTIFIER OFF 
	ALTER DATABASE [EDDSPerformance] SET RECURSIVE_TRIGGERS OFF 
	ALTER DATABASE [EDDSPerformance] SET DISABLE_BROKER 
	ALTER DATABASE [EDDSPerformance] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
	ALTER DATABASE [EDDSPerformance] SET DATE_CORRELATION_OPTIMIZATION OFF 
	ALTER DATABASE [EDDSPerformance] SET ALLOW_SNAPSHOT_ISOLATION OFF 
	ALTER DATABASE [EDDSPerformance] SET PARAMETERIZATION SIMPLE 
	ALTER DATABASE [EDDSPerformance] SET READ_COMMITTED_SNAPSHOT OFF 
	ALTER DATABASE [EDDSPerformance] SET READ_WRITE 
	ALTER DATABASE [EDDSPerformance] SET RECOVERY FULL 
	ALTER DATABASE [EDDSPerformance] SET MULTI_USER 
	ALTER DATABASE [EDDSPerformance] SET PAGE_VERIFY CHECKSUM
	
	ALTER DATABASE [EDDSPerformance]
		SET MULTI_USER;
END
GO

/*==========================CREATE EDDSDBO SCHEMA ON EDDSPERFORMANCE======================*/


USE [EDDSPerformance]
IF NOT EXISTS (SELECT * FROM [EDDSPerformance].INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'eddsdbo')
BEGIN
	EXECUTE('CREATE SCHEMA [eddsdbo] AUTHORIZATION [dbo]')
END
GO


