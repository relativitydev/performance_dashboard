namespace kCura.PDB.Core.Interfaces.Hours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IHourMigrationService
	{
		Task<IList<int>> IdentifyIncompleteHours();

		Task<int> CancelHour(int hourId);

		Task CancelEvents();
	}
}
