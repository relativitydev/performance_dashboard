namespace kCura.PDB.Data
{
	using System;
	using System.Data.Common;
	using System.Data.Linq;
	using System.Linq;

	public interface IPDDModelDataContext : IDisposable
	{
		IQueryable<Measure> ReadMeasures();

		void SubmitChanges();

		DbConnection Connection { get; }

		int LoadLatencyHealthDWData(DateTime? processExecDate);

		ISingleResult<GetRAMHealthDataResult> GetRAMHealthData(DateTime? startDate, DateTime? endDate, int? timeZoneOffset);

		ISingleResult<GetSQLServerSummaryDataResult> GetSQLServerSummaryData(DateTime? startDate, DateTime? endDate, int? timeZoneOffset);

		ISingleResult<GetServerProcessorSummaryDataResult> GetServerProcessorSummaryData(DateTime? startDate, DateTime? endDate, int? timeZoneOffset);

		int LoadDocumentHealthDWData(DateTime? processExecDate);

		ISingleResult<GetServerDiskSummaryDataResult> GetServerDiskSummaryData(DateTime? startDate, DateTime? endDate, int? timeZoneOffset);

		IQueryable<GetLRQHealthQueriesResult> GetLRQHealthQueries(DateTime? processExecDate);

		ISingleResult<GetApplicationHealthDataResult> GetApplicationHealthData(DateTime? startDate, DateTime? endDate, int? timeZoneOffset);
	}
}
