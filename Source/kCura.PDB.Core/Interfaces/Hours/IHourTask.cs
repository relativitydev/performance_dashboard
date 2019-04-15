namespace kCura.PDB.Core.Interfaces.Hours
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IHourTask
	{
		Task<int> ScoreHour(int hourId);

		Task<EventResult> CheckIfHourReadyToScore(int hourId);
	}
}
