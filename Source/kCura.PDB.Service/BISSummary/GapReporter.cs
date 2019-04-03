namespace kCura.PDB.Service.BISSummary
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public class GapReporter : IGapReporter
	{
		private readonly IRecoverabilityIntegrityReportWriter recoverabilityIntegrityReportRepository;
		private readonly IDatabaseService databaseService;

		public GapReporter(
			IRecoverabilityIntegrityReportWriter recoverabilityIntegrityReportRepository,
			IDatabaseService databaseService)
		{
			this.recoverabilityIntegrityReportRepository = recoverabilityIntegrityReportRepository;
			this.databaseService = databaseService;
		}

		public async Task CreateGapReport<TGap>(
			Hour hour,
			Server server,
			IList<TGap> gaps,
			GapActivityType gapType)
			where TGap : Gap
		{
			// Add new resolved gaps of gapsize greater than the threshold window
			await gaps.Select(g => new GapReportEntry
				                       {
					                       DatabaseId = g.DatabaseId,
					                       ActivityType = (int)g.ActivityType,
					                       LastActivity = g.Start,
					                       GapResolutionDate = g.End,
					                       GapSize = g.Duration.Value
				                       })
				.Select(g => this.recoverabilityIntegrityReportRepository.CreateGapReportData(g))
				.WhenAllStreamed(1);

			// Clear unresolved gaps of report type
			await this.recoverabilityIntegrityReportRepository.ClearUnresolvedGapReportData(server.ServerId, gapType);

			// Get unresolved gaps
			var unresolvedGaps = await this.databaseService.ReadUnresolvedGapsAsync(hour, server, gapType);

			// Get 'Now' for unresolved gaps
			var now = hour.GetNextHour();
			var unresolvedGapReports = unresolvedGaps.Select(g => new GapReportEntry
				                                                      {
					                                                      DatabaseId = g.DatabaseId,
					                                                      ActivityType = (int)g.ActivityType,
					                                                      LastActivity = g.Start,
					                                                      GapResolutionDate = null,
					                                                      GapSize = (int)(now - g.Start).TotalSeconds
				                                                      });

			// Add unresolved gaps
			await unresolvedGapReports.Select(g => this.recoverabilityIntegrityReportRepository.CreateGapReportData(g))
				.WhenAllStreamed(1);
		}
	}
}
