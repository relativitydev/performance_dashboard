namespace kCura.PDD.Web.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

    public interface IMaintenanceWindowSchedulingService
	{
        Task<MaintenanceWindow> ScheduleMaintenanceWindowAsync(MaintenanceWindow window);

	    Task<GeneralCheckGrid<MaintenanceWindow>> GetFilteredMaintenanceWindowsAsync(MaintenanceWindowDataTableQuery query);

	    Task<MaintenanceWindow> ReadMaintenanceWindowAsync(int id);

        Task DeleteMaintenanceWindowAsync(MaintenanceWindow window);
	}
}
