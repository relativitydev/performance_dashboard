namespace kCura.PDB.Core.Interfaces.Hours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IHourCleanupLogic
	{
		Task<IList<int>> CleanupForHour(int hourId);

		Task<int> CleanupQosTables(int serverCleanupId);
	}
}
