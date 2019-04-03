namespace kCura.PDB.Service.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Interfaces.Testing.Services;

	public class ReportCleanupService : IReportCleanupService
    {
        private readonly IReportCleanupRepository reportCleanupRepository;

        public ReportCleanupService(IReportCleanupRepository reportCleanupRepository)
        {
            this.reportCleanupRepository = reportCleanupRepository;
        }

        public Task ClearReportDataAsync(IList<DateTime> hours)
        {
            return Task.WhenAll(
                this.reportCleanupRepository.ClearReportData(hours),
                this.reportCleanupRepository.ClearExistingBackupHistory(),
                this.reportCleanupRepository.ClearExistingDbccHistory());
        }
    }
}
