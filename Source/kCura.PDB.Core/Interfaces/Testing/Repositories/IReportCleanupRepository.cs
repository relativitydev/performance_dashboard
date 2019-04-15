namespace kCura.PDB.Core.Interfaces.Testing.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IReportCleanupRepository
    {
        Task ClearReportData(IList<DateTime> summaryDayHour);

        Task ClearExistingBackupHistory();

        Task ClearExistingDbccHistory();
    }
}
