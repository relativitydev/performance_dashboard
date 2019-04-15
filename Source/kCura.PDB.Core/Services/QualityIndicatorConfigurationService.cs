namespace kCura.PDB.Core.Services
{
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class QualityIndicatorConfigurationService : IQualityIndicatorConfigurationService
	{
		private IConfigurationRepository configRepository;

		public QualityIndicatorConfigurationService(IConfigurationRepository configurationRepository)
		{
			this.configRepository = configurationRepository;
		}

		public QualityIndicatorLevel GetIndictatorConfiguration()
		{
			var configPassValue = this.configRepository?.ReadValue<int>(ConfigurationKeys.PassScore);
			var configWarnValue = this.configRepository?.ReadValue<int>(ConfigurationKeys.WarnScore);

			return new QualityIndicatorLevel()
			{
				PassScore = configPassValue.HasValue ? (int)configPassValue : Defaults.Scores.DefaultPassScore,
				WarnScore = configWarnValue.HasValue ? (int)configWarnValue : Defaults.Scores.DefaultWarnScore
			};
		}
	}
}
