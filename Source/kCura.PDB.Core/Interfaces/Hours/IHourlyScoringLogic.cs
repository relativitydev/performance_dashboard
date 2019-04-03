namespace kCura.PDB.Core.Interfaces.Hours
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IHourlyScoringLogic
	{
		Task<decimal> ScoreHour(Hour hour);
	}
}
