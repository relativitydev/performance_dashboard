namespace kCura.PDB.Core.Interfaces.Testing.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IReportCleanupService
    {
        Task ClearReportDataAsync(IList<DateTime> hours);
    }
}
