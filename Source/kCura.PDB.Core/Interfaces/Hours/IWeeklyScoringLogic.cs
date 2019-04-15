namespace kCura.PDB.Core.Interfaces.Hours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IWeeklyScoringLogic
	{
		Task<IList<Hour>> GetSampleHoursAsync(IList<CategoryScore> scores);
	}
}
