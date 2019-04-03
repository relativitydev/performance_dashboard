USE [master]
GO

/*=============================CREATE {{resourcedbname}} DATABASE=============================*/

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{{resourcedbname}}') BEGIN
	-- Get file path used to store other Relativity databases
	DECLARE @dbFileName nvarchar(MAX) = '{{mdfDir}}{{resourcedbname}}.mdf',
		@dbFileLogName nvarchar(MAX) = '{{ldfDir}}{{resourcedbname}}_log.ldf',
		@createScript nvarchar(MAX),
		@createTemplate VARCHAR(MAX),
		@collation nvarchar(MAX) = (
			SELECT 'COLLATE ' + '{{collation}}'
		);
	SET @createTemplate = '
	CREATE DATABASE [{{resourcedbname}}]
	CONTAINMENT = NONE
	ON PRIMARY (
		NAME = N''{{resourcedbname}}'',
		FILENAME = ''{DbFileName}'',
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 1024MB
	)
	LOG ON (
		NAME = N''{{resourcedbname}}_log'',
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

/*=============================CREATE EDDSDBO SCHEMA ON {{resourcedbname}}====================*/

USE [{{resourcedbname}}]
/****** Object:  Schema [eddsdbo]    Script Date: 10/11/2011 13:32:07 ******/
--create eddsdbo user for PDB resource database, if eddsdbo does not exist
IF (NOT EXISTS (SELECT name FROM sys.database_principals WHERE (name = 'eddsdbo' )))
BEGIN
	EXEC sp_adduser 'eddsdbo', 'eddsdbo', 'db_owner';
end
GO

/*=============================ALTER {{resourcedbname}} DATABASE===============================*/

USE [{{resourcedbname}}]
IF NOT EXISTS (SELECT * FROM [{{resourcedbname}}].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHVersion') BEGIN
	ALTER DATABASE [{{resourcedbname}}]
		SET SINGLE_USER
		WITH ROLLBACK IMMEDIATE;
		
	ALTER DATABASE [{{resourcedbname}}] SET COMPATIBILITY_LEVEL = 100

	IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
	BEGIN
		EXEC [{{resourcedbname}}].[dbo].[sp_fulltext_database] @action = 'enable'
	END

	ALTER DATABASE [{{resourcedbname}}] SET ANSI_NULL_DEFAULT OFF 
	ALTER DATABASE [{{resourcedbname}}] SET ANSI_NULLS OFF 
	ALTER DATABASE [{{resourcedbname}}] SET ANSI_PADDING OFF 
	ALTER DATABASE [{{resourcedbname}}] SET ANSI_WARNINGS OFF 
	ALTER DATABASE [{{resourcedbname}}] SET ARITHABORT OFF 
	ALTER DATABASE [{{resourcedbname}}] SET AUTO_CLOSE OFF 
	ALTER DATABASE [{{resourcedbname}}] SET AUTO_CREATE_STATISTICS ON 
	ALTER DATABASE [{{resourcedbname}}] SET AUTO_SHRINK OFF 
	ALTER DATABASE [{{resourcedbname}}] SET AUTO_UPDATE_STATISTICS ON 
	ALTER DATABASE [{{resourcedbname}}] SET CURSOR_CLOSE_ON_COMMIT OFF 
	ALTER DATABASE [{{resourcedbname}}] SET CURSOR_DEFAULT  GLOBAL 
	ALTER DATABASE [{{resourcedbname}}] SET CONCAT_NULL_YIELDS_NULL OFF 
	ALTER DATABASE [{{resourcedbname}}] SET NUMERIC_ROUNDABORT OFF 
	ALTER DATABASE [{{resourcedbname}}] SET QUOTED_IDENTIFIER OFF 
	ALTER DATABASE [{{resourcedbname}}] SET RECURSIVE_TRIGGERS OFF 
	ALTER DATABASE [{{resourcedbname}}] SET DISABLE_BROKER
	ALTER DATABASE [{{resourcedbname}}] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
	ALTER DATABASE [{{resourcedbname}}] SET DATE_CORRELATION_OPTIMIZATION OFF 
	ALTER DATABASE [{{resourcedbname}}] SET ALLOW_SNAPSHOT_ISOLATION OFF 
	ALTER DATABASE [{{resourcedbname}}] SET PARAMETERIZATION SIMPLE 
	ALTER DATABASE [{{resourcedbname}}] SET READ_COMMITTED_SNAPSHOT OFF 
	ALTER DATABASE [{{resourcedbname}}] SET RECOVERY FULL 
	ALTER DATABASE [{{resourcedbname}}] SET MULTI_USER 
	ALTER DATABASE [{{resourcedbname}}] SET PAGE_VERIFY CHECKSUM  
	ALTER DATABASE [{{resourcedbname}}] SET READ_WRITE 
	--copied from eddsresource create
	ALTER DATABASE [{{resourcedbname}}] SET TRUSTWORTHY ON 
	ALTER DATABASE [{{resourcedbname}}] 
		SET MULTI_USER;
END
GO
/*==========================INDICATE SCRIPT RESULTS =================================
==	RoundhousE expects an output indicating the results of running this script.	   ==
==																				   ==
==	1: Database was created successfully (this run)								   ==
==	0: Database was not created successfully or already existed					   ==
===================================================================================*/
USE [{{resourcedbname}}]
IF NOT EXISTS (SELECT * FROM [{{resourcedbname}}].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHVersion') BEGIN
	SELECT 1
END ELSE BEGIN
	SELECT 0
END