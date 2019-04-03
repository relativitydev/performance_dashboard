namespace kCura.PDB.Service.Audits
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.MetricDataSources;

	public class UserExperienceService : IUserExperienceService
	{
		private readonly IServerAuditService auditService;
		private readonly IPoisonWaitRepository poisonWaitRepository;
		private readonly ISearchAuditBatchRepository searchAuditBatchRepository;
		private readonly ILogger logger;

		public UserExperienceService(
			IServerAuditService auditService,
			IPoisonWaitRepository poisonWaitRepository,
			ISearchAuditBatchRepository searchAuditBatchRepository,
			ILogger logger)
		{
			this.auditService = auditService;
			this.poisonWaitRepository = poisonWaitRepository;
			this.searchAuditBatchRepository = searchAuditBatchRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.UserExperience);
		}

		/// <inheritdoc />
		public async Task<UserExperience> BuildUserExperienceModel(int serverId, Hour hour)
		{
			var activeUsers = await this.auditService.ReadTotalUniqueUsersForHourAuditsAsync(serverId, hour.Id, AuditConstants.RelevantAuditActionIds);
			var hasPoisonWaits = await this.poisonWaitRepository.ReadPoisonWaitsForHourAsync(hour, serverId);
			var arrivalRate = this.CalculateArrivalRate(serverId, hour.Id);
			var concurrency = this.CalculateConcurrency(serverId, hour.Id);

			return new UserExperience
			{
				ActiveUsers = activeUsers,
				HasPoisonWaits = hasPoisonWaits,
				ArrivalRate = await arrivalRate,
				Concurrency = await concurrency,
				HourId = hour.Id,
				ServerId = serverId
			};
		}

		internal async Task<decimal> CalculateArrivalRate(int serverId, int hourId)
		{
			var totalAudits = await this.auditService.ReadTotalAuditsForHourAsync(serverId, hourId, AuditConstants.RelevantAuditActionIds);
			await this.logger.LogVerboseAsync(
				$"Calculating Arrival Rate for serverId: {serverId}, hourId: {hourId}, totalAudits: {totalAudits}, RelevantAuditActionIds: ({string.Join(",", AuditConstants.RelevantAuditActionIds)})");
			return this.CalculateFinalArrivalRate(totalAudits);
		}

		internal decimal CalculateFinalArrivalRate(long totalQueries)
		{
			return totalQueries / 3600.0m;
		}

		internal async Task<decimal> CalculateConcurrency(int serverId, int hourId)
		{
			var totalAudits3456 = this.auditService.ReadTotalAuditsForHourAsync(serverId, hourId, AuditConstants.Audits3456);
			var totalAudits47 = this.auditService.ReadTotalAuditsForHourAsync(serverId, hourId, new List<AuditActionId> { AuditActionId.UpdateImport });

			// Get all of the search audit batch results -- can be simplified in the future if Data Grid adds Sum(ExecutionTime) support to their API
			var batches = this.searchAuditBatchRepository.ReadByHourAndServer(hourId, serverId);
			var totalExecutionTime = batches.Sum(b => b.BatchResults.Sum(r => r.TotalExecutionTime));

			totalExecutionTime += (await totalAudits3456 * 2) + (await totalAudits47 * 10);
			return this.CalculateFinalConcurrency(totalExecutionTime);
		}

		internal decimal CalculateFinalConcurrency(long totalExecutionTime)
		{
			return totalExecutionTime / (3600.0m * 1000.0m);
		}
	}
}