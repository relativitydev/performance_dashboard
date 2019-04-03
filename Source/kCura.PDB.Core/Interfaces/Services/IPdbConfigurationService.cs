namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public interface IPdbConfigurationService
	{
		PerformanceDashboardConfigurationSettings GetConfiguration();

		void SetConfiguration(PerformanceDashboardConfigurationSettings config);

		List<ConfigurationAudit> ListChanges(PerformanceDashboardConfigurationSettings previous, PerformanceDashboardConfigurationSettings current);

		ValidationResult ValidateConfiguration(PerformanceDashboardConfigurationSettings config);

		bool ElevatedScriptsInstalled();
	}
}
