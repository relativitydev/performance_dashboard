USE [EDDSPerformance]
GO

DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'PopulateFactTableData', @Type = 'PROCEDURE', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO

-- =============================================
-- Author:		Jayesh Dhobi
-- Create date: 13th Sep 2011
-- Description:	Populate Fact Table data
-- Modified By: Justin Jarczyk 2/4/2014, to account
-- for the addition of drive letters into the 
-- ServerDiskSummary table.
-- =============================================  
ALTER PROCEDURE  [eddsdbo].[PopulateFactTableData]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN --- Declaration
		DECLARE @IsRunsuccessfully BIT = 0
		
		DECLARE @MeasureDate datetime = GETUTCDATE()
		DECLARE @MeasureHour datetime = DATEADD(hour, DATEDIFF(hour, 0, @MeasureDate) - 1, 0)	-- truncated to the last hour minus 1 hour

	END
	PRINT ' Start '
	BEGIN TRY
		BEGIN TRAN
		
		BEGIN --- Populate ServerSummary table
			MERGE [eddsdbo].[ServerSummary]
			USING
			(
				SELECT
					  ServerID
					, @MeasureHour MeasureDate
					, AVG(RAMPagesPerSec) AS RAMPagesPerSec
					, AVG(RAMPageFaultsPerSec) AS RAMPageFaultsPerSec
					, AVG(TotalPhysicalMemory) AS TotalPhysicalMemory
					, AVG(AvailableMemory) AS AvailableMemory
					, AVG(RAMPct) AS RAMPct
				FROM [eddsdbo].[ServerDW]
				WHERE CreatedOn BETWEEN @MeasureHour AND DATEADD(HH, 1, @MeasureHour)
				GROUP BY ServerID
			) AS Data ON Data.ServerID = [ServerSummary].ServerID
				AND Data.MeasureDate = [ServerSummary].MeasureDate
			WHEN MATCHED THEN UPDATE
                SET RAMPagesPerSec = Data.RAMPagesPerSec
				, RAMPageFaultsPerSec = Data.RAMPageFaultsPerSec
			WHEN NOT MATCHED THEN
				INSERT (
                            ServerID
							, MeasureDate
							, RAMPagesPerSec
							, RAMPageFaultsPerSec
							, TotalPhysicalMemory
							, AvailableMemory
							, RAMPct
							, CreatedOn
                        )
                VALUES ( 
                            Data.ServerID
							, Data.MeasureDate
							, Data.RAMPagesPerSec
							, Data.RAMPageFaultsPerSec
							, Data.TotalPhysicalMemory
							, Data.AvailableMemory
							, Data.RAMPct
							, @MeasureDate
                        );
		END
		
		BEGIN --- Populate ServerDiskSummary table
			MERGE [eddsdbo].[ServerDiskSummary]
			USING
			(	
				SELECT
					  ServerID
					, DiskNumber
					, @MeasureHour MeasureDate
					, AVG( DiskAvgReadsPerSec ) AS DiskAvgReadsPerSec
					, AVG( DiskAvgWritesPerSec ) AS DiskAvgWritesPerSec
					, MIN(DriveLetter) AS DriveLetter
					, AVG( DiskFreeMegabytes ) AS DiskFreeMegabytes
					, COALESCE(
						CAST((MAX(DiskSecPerRead)-MIN(DiskSecPerRead)) as decimal)
						/NULLIF(AVG(FrequencyPerfTime), 0)
						/NULLIF((MAX(DiskSecPerReadBase)-MIN(DiskSecPerReadBase)), 0),
					  0) AS DiskSecPerRead
					, COALESCE(
						CAST((MAX(DiskSecPerWrite)-MIN(DiskSecPerWrite)) as decimal)
						/NULLIF(AVG(FrequencyPerfTime), 0)
						/NULLIF((MAX(DiskSecPerWriteBase)-MIN(DiskSecPerWriteBase)), 0),
					  0) AS DiskSecPerWrite
				FROM [eddsdbo].[ServerDiskDW]
				WHERE CreatedOn BETWEEN @MeasureHour AND DATEADD(HH, 1, @MeasureHour)
				GROUP BY ServerID, DiskNumber
			) AS Data ON Data.ServerID = [ServerDiskSummary].ServerID
				AND Data.MeasureDate = [ServerDiskSummary].MeasureDate
				AND Data.DiskNumber = [ServerDiskSummary].DiskNumber
			WHEN MATCHED THEN UPDATE 
                SET DiskAvgReadsPerSec = Data.DiskAvgReadsPerSec
					, DiskAvgWritesPerSec = Data.DiskAvgWritesPerSec
					, DiskFreeMegabytes = Data.DiskFreeMegabytes
					, DiskSecPerRead = Data.DiskSecPerRead
					, DiskSecPerWrite = Data.DiskSecPerWrite
			WHEN NOT MATCHED THEN
				INSERT
				(
					ServerID
					, MeasureDate
					, DiskNumber
					, DiskAvgReadsPerSec
					, DiskAvgWritesPerSec
					, CreatedOn
					, DriveLetter
					, DiskFreeMegabytes
					, DiskSecPerRead
					, DiskSecPerWrite
				)
				VALUES 
				(
					Data.ServerID
					, Data.MeasureDate
					, Data.DiskNumber
					, Data.DiskAvgReadsPerSec
					, Data.DiskAvgWritesPerSec
					, @MeasureDate
					, Data.DriveLetter
					, Data.DiskFreeMegabytes
					, Data.DiskSecPerRead
					, Data.DiskSecPerWrite
				);
		END		
		
		BEGIN --- Populate ServerProcessorSummary table
			MERGE [eddsdbo].[ServerProcessorSummary]
			USING
			(	
				SELECT
					  ServerID
					, CoreNumber
					, @MeasureHour MeasureDate
					, AVG( CPUProcessorTimePct ) AS CPUProcessorTimePct
					, MIN( CPUName ) AS CPUName
				FROM [eddsdbo].[ServerProcessorDW]
				WHERE CreatedOn BETWEEN @MeasureHour AND DATEADD(HH, 1, @MeasureHour)
				GROUP BY ServerID, CoreNumber
			) AS Data ON Data.ServerID = [ServerProcessorSummary].ServerID
				AND Data.MeasureDate = [ServerProcessorSummary].MeasureDate
				AND Data.CoreNumber = [ServerProcessorSummary].CoreNumber
			
			WHEN MATCHED THEN UPDATE 
                SET CPUProcessorTimePct = Data.CPUProcessorTimePct,
					CPUName = Data.CPUName
			
			WHEN NOT MATCHED THEN
				INSERT
				(
					ServerID
					, MeasureDate
					, CoreNumber
					, CPUProcessorTimePct
					, CreatedOn
					, CPUName
				)
				VALUES 
				(
					Data.ServerID
					, Data.MeasureDate
					, Data.CoreNumber
					, Data.CPUProcessorTimePct
					, @MeasureDate
					, Data.CPUName
				);
		END
		
		BEGIN --- Populate SQLServerSummary table
			MERGE [eddsdbo].[SQLServerSummary]
			USING
			(	
				SELECT
					ServerID
					, @MeasureHour MeasureDate
					, AVG( SQLPageLifeExpectancy ) AS SQLPageLifeExpectancy
					, AVG( CAST(LowMemorySignalState as decimal) ) AS LowMemorySignalStateRatio
				FROM [eddsdbo].[SQLServerDW]
				WHERE CreatedOn BETWEEN @MeasureHour AND DATEADD(HH, 1, @MeasureHour)
				GROUP BY ServerID
			) AS Data ON Data.ServerID = [SQLServerSummary].ServerID
				AND Data.MeasureDate = [SQLServerSummary].MeasureDate
			WHEN MATCHED THEN UPDATE 
				SET SQLPageLifeExpectancy = Data.SQLPageLifeExpectancy,
					LowMemorySignalStateRatio = Data.LowMemorySignalStateRatio
			WHEN NOT MATCHED THEN
				INSERT
				(
					ServerID
					, MeasureDate
					, SQLPageLifeExpectancy
					, LowMemorySignalStateRatio
					, CreatedOn
				)
				VALUES 
				(
					Data.ServerID
					, Data.MeasureDate
					, Data.SQLPageLifeExpectancy
					, Data.LowMemorySignalStateRatio
					, @MeasureDate
				);
		END

		
		COMMIT TRAN
		
		SET @IsRunsuccessfully = 1
		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
		
	IF (@IsRunsuccessfully = 1)
		BEGIN
			SELECT @IsRunsuccessfully AS IsRunsuccessfully, ''	AS ErrorMessage
		END
	ELSE
		BEGIN
			SELECT @IsRunsuccessfully AS IsRunsuccessfully, ERROR_MESSAGE()	AS ErrorMessage
		END
END

