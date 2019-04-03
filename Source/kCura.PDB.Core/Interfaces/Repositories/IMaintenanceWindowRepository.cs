namespace kCura.PDB.Core.Interfaces.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IMaintenanceWindowRepository
	{
		Task<MaintenanceWindow> CreateAsync(MaintenanceWindow window);

		Task<MaintenanceWindow> ReadAsync(int id);

		Task UpdateAsync(MaintenanceWindow window);

		Task DeleteAsync(MaintenanceWindow window);

	    Task<bool> HourIsInMaintenanceWindowAsync(Hour hour);

	    Task<IEnumerable<MaintenanceWindow>> ReadSortedAsync(MaintenanceWindowDataTableQuery query);
	}
}
