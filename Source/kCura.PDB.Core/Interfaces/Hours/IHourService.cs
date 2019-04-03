namespace kCura.PDB.Core.Interfaces.Hours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IHourService
	{
		Task<IList<int>> CreateNextHours();

		Task<int> StartHour(int hourId);

		Task<int> CompleteHour(int hourId);
	}
}
