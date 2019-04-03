namespace kCura.PDB.Service.Audits
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Transactions;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Workspace;
	using kCura.PDB.Core.Models.Audits;

	public class AuditBatchProcessor : IAuditBatchProcessor
	{
		private readonly ISearchAuditBatchRepository searchAuditBatchRepository;
		private readonly IHourRepository hourRepository;
		private readonly ISearchAnalysisService searchAnalysisService;
		private readonly IAuditBatchAnalyzer auditBatchAnalyzer;
		private readonly IWorkspaceAuditServiceFactory workspaceAuditServiceFactory;
		private readonly IWorkspaceAuditReporter workspaceAuditReporter;
		private readonly IServerRepository serverRepository;
		private readonly IAuditParsingService auditParsingService;
		private readonly IWorkspaceService workspaceService;
		private readonly ILogger logger;

		public AuditBatchProcessor(
			ISearchAuditBatchRepository searchAuditBatchRepository,
			IHourRepository hourRepository,
			ISearchAnalysisService searchAnalysisService,
			IAuditBatchAnalyzer auditBatchAnalyzer,
			IWorkspaceAuditServiceFactory workspaceAuditServiceFactory,
			IWorkspaceAuditReporter workspaceAuditReporter,
			IServerRepository serverRepository,
			IAuditParsingService auditParsingService,
			IWorkspaceService workspaceService,
			ILogger logger)
		{
			this.searchAuditBatchRepository = searchAuditBatchRepository;
			this.hourRepository = hourRepository;
			this.searchAnalysisService = searchAnalysisService;
			this.auditBatchAnalyzer = auditBatchAnalyzer;
			this.workspaceAuditServiceFactory = workspaceAuditServiceFactory;
			this.workspaceAuditReporter = workspaceAuditReporter;
			this.serverRepository = serverRepository;
			this.auditParsingService = auditParsingService;
			this.workspaceService = workspaceService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Audit);
		}

		/// <inheritdoc />
		public async Task<IList<int>> ProcessBatch(int batchId)
		{
			// Query Batch
			var batch = this.searchAuditBatchRepository.ReadBatch(batchId);
			ThrowOn.IsNull(batch, "batch");

			// Check that this workspace exists before doing anything else
			if (!await this.serverRepository.ReadWorkspaceExistsAsync(batch.WorkspaceId) ||
				!await this.workspaceService.WorkspaceIsAvailableAsync(batch.WorkspaceId))
			{
				// Just mark the batch as completed.  The workspace was deleted after the batches were created.
				// Throw them out/ignore them and keep the process going.
				this.logger.LogWarning(
					$"Workspace not found for batch ID ({batch.Id}), workspaceId ({batch.WorkspaceId}) not found.  For hourId {batch.HourId}, serverId {batch.ServerId}");
				batch.Completed = true;
				await this.searchAuditBatchRepository.UpdateAsync(batch);
				return new List<int>();
			}

			// Process Batch
			// Grab the end hour
			var hour = await this.hourRepository.ReadAsync(batch.HourId);
			ThrowOn.IsNull(hour, "hour");

			// Grab the correct audit service (Sql/DataGrid)
			var repo = await this.workspaceAuditServiceFactory.GetAuditService(batch.WorkspaceId, batch.HourId);

			await this.logger.LogVerboseAsync(
				$"Getting Audits for hour: {batch.HourId} - {hour.HourTimeStamp} from auditService: {repo.GetType()}");

			// --- 28 (SEARCH) AUDITS ---
			// Read the search audits
			var audits = await repo.ReadAuditsAsync(
				batch.WorkspaceId,
				batch.HourId,
				new[] { AuditActionId.DocumentQuery },
				batch.BatchSize,
				batch.BatchStart);

			// group by query Id
			var searchAudits =
				audits.Select(a => new SearchAudit
				{
					Audit = a,
					QueryId =
						!string.IsNullOrEmpty(a.Details)
							? this.auditParsingService.ParseRawQueryId(a.Details)
							: this.auditParsingService.ParseQueryId(a.ParsedDetails),
					QueryType =
						!string.IsNullOrEmpty(a.Details)
							? this.auditParsingService.ParseRawQueryType(a.Details)
							: this.auditParsingService.ParseQueryType(a.ParsedDetails)
				});

			var searchAuditGroups = searchAudits
				.GroupBy(sa => sa.QueryId)
				.SelectMany(sag =>
					string.IsNullOrEmpty(sag.Key)
					? sag.Select(sa => new SearchAuditGroup { Audits = new List<SearchAudit> { sa } })
					: new List<SearchAuditGroup>
					{
						new SearchAuditGroup
						{
							Audits = sag.ToList()
						}
					});

			// analyze search audits
			var analyzeSearchAudits =
				(await searchAuditGroups.Select(a => this.searchAnalysisService.AnalyzeSearchAudit(a)).WhenAllStreamed()).ToList();

			// Save Report Data
			var reportTask = this.workspaceAuditReporter.ReportWorkspaceAudits(analyzeSearchAudits, hour, batch.ServerId);

			// Separate TotalLongRunningQueries / TotalComplexQueries / TotalQueries per user
			var results = this.auditBatchAnalyzer.GetBatchResults(analyzeSearchAudits, batchId);
			ThrowOn.IsNull(results, "batch results");

			// Save BatchResult
			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				// Insert the BatchResult
				var createdResults = this.searchAuditBatchRepository.CreateBatchResults(results);
				ThrowOn.IsNull(createdResults, "created batch results");

				// Update HourSearchAuditBatch Status
				batch.Completed = true;
				await this.searchAuditBatchRepository.UpdateAsync(batch);

				// Make sure all of the above complete.  This should be a single point of failure.
				scope.Complete();

				// Await the reporter
				await reportTask;

				return createdResults;
			}
		}
	}
}
