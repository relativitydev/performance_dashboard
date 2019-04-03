namespace kCura.PDB.Core.Interfaces.Hours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IHourPrerequisitesService
	{
		Task<EventResult> CheckForPrerequisites();

		Task<bool> CheckAllPrerequisitesComplete();

		Task CompletePrerequisites();
	}
}
