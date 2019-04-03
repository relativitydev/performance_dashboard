namespace kCura.PDB.Core.Interfaces.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IGapsFrequencyAnalyzer
	{
		decimal ScoreFrequencyData(FrequencyMetricData frequencyMetricData);

		Task<FrequencyMetricData> CaptureFrequencyData<TGap>(Hour hour, Server server, GapActivityType activityType)
			where TGap : Gap;
	}
}
