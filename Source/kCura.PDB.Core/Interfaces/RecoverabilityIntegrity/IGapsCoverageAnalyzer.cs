namespace kCura.PDB.Core.Interfaces.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IGapsCoverageAnalyzer
	{
		decimal ScoreCoverageData(CoverageMetricData coverageMetricData);

		Task<CoverageMetricData> CaptureCoverageData<TGap>(Hour hour, Server server, GapActivityType activityType)
			where TGap : Gap;
	}
}
