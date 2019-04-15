USE [master]
GO

/*=============================CREATE EDDSQOS DATABASE=============================*/

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EDDSQoS') BEGIN
	-- Get file path used to store other Relativity databases
	DECLARE @dbFileName nvarchar(MAX) = '{{mdfDir}}EDDSQoS.mdf',
		@dbFileLogName nvarchar(MAX) = '{{ldfDir}}EDDSQoS_log.ldf',
		@createScript nvarchar(MAX),
		@createTemplate VARCHAR(MAX),
		@collation nvarchar(MAX) = (
			SELECT 'COLLATE ' + '{{collation}}'
		);
	SET @createTemplate = '
	CREATE DATABASE [EDDSQoS] ON PRIMARY (
		NAME = N''EDDSQoS'',
		FILENAME = ''{DbFileName}'',
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 1024MB
	)
	LOG ON (
		NAME = N''EDDSQoS_log'',
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

/*=============================ALTER EDDSQoS DATABASE==============================*/

USE [EDDSQoS]
IF NOT EXISTS (SELECT * FROM [EDDSQoS].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHVersion') BEGIN
	ALTER DATABASE [EDDSQoS]
		SET SINGLE_USER
		WITH ROLLBACK IMMEDIATE;
	
	ALTER DATABASE [EDDSQoS] SET COMPATIBILITY_LEVEL = 100

	IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
	BEGIN
		EXEC [EDDSQoS].[dbo].[sp_fulltext_database] @action = 'enable'
	END

	ALTER DATABASE [EDDSQoS] SET ANSI_NULL_DEFAULT OFF 
	ALTER DATABASE [EDDSQoS] SET ANSI_NULLS OFF 
	ALTER DATABASE [EDDSQoS] SET ANSI_PADDING OFF 
	ALTER DATABASE [EDDSQoS] SET ANSI_WARNINGS OFF 
	ALTER DATABASE [EDDSQoS] SET ARITHABORT OFF 
	ALTER DATABASE [EDDSQoS] SET AUTO_CLOSE OFF 
	ALTER DATABASE [EDDSQoS] SET AUTO_CREATE_STATISTICS ON 
	ALTER DATABASE [EDDSQoS] SET AUTO_SHRINK OFF 
	ALTER DATABASE [EDDSQoS] SET AUTO_UPDATE_STATISTICS ON 
	ALTER DATABASE [EDDSQoS] SET CURSOR_CLOSE_ON_COMMIT OFF 
	ALTER DATABASE [EDDSQoS] SET CURSOR_DEFAULT  GLOBAL 
	ALTER DATABASE [EDDSQoS] SET CONCAT_NULL_YIELDS_NULL OFF 
	ALTER DATABASE [EDDSQoS] SET NUMERIC_ROUNDABORT OFF 
	ALTER DATABASE [EDDSQoS] SET QUOTED_IDENTIFIER OFF 
	ALTER DATABASE [EDDSQoS] SET RECURSIVE_TRIGGERS OFF 
	ALTER DATABASE [EDDSQoS] SET DISABLE_BROKER
	ALTER DATABASE [EDDSQoS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
	ALTER DATABASE [EDDSQoS] SET DATE_CORRELATION_OPTIMIZATION OFF 
	ALTER DATABASE [EDDSQoS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
	ALTER DATABASE [EDDSQoS] SET PARAMETERIZATION SIMPLE 
	ALTER DATABASE [EDDSQoS] SET READ_COMMITTED_SNAPSHOT OFF 
	ALTER DATABASE [EDDSQoS] SET RECOVERY FULL 
	ALTER DATABASE [EDDSQoS] SET MULTI_USER 
	ALTER DATABASE [EDDSQoS] SET PAGE_VERIFY CHECKSUM  
	ALTER DATABASE [EDDSQoS] SET READ_WRITE 
	
	ALTER DATABASE [EDDSQoS]
		SET MULTI_USER;
END

/*=============================CREATE EDDSDBO SCHEMA ON EDDSQOS====================*/

USE [EDDSQoS]
IF NOT EXISTS (SELECT * FROM [EDDSQoS].INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'eddsdbo') BEGIN
	EXECUTE('CREATE SCHEMA [eddsdbo] AUTHORIZATION [dbo]')
END
GO


/*==========================INDICATE SCRIPT RESULTS =================================
==	RoundhousE expects an output indicating the results of running this script.	   ==
==																				   ==
==	1: Database was created successfully (this run)								   ==
==	0: Database was not created successfully or already existed					   ==
===================================================================================*/
USE [EDDSQoS]
IF NOT EXISTS (SELECT * FROM [EDDSQoS].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHVersion') BEGIN
	SELECT 1
END ELSE BEGIN
	SELECT 0
END