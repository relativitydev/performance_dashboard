namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;

	/// <inheritdoc />
	public class SearchAnalysisService : ISearchAnalysisService
	{
		private readonly IViewCriteriaRepository viewCriteriaRepository;
		private readonly IViewRepository viewRepository;
		private readonly ISearchFolderRepository searchFolderRepository;
		private readonly ILogger logger;
		private readonly IAuditParsingService auditParsingService;

		public SearchAnalysisService(
			IViewCriteriaRepository viewCriteriaRepository,
			IViewRepository viewRepository,
			ISearchFolderRepository searchFolderRepository,
			ILogger logger,
			IAuditParsingService auditParsingService)
		{
			this.viewCriteriaRepository = viewCriteriaRepository;
			this.viewRepository = viewRepository;
			this.searchFolderRepository = searchFolderRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Audit);
			this.auditParsingService = auditParsingService;
		}

		/// <inheritdoc />
		public async Task<SearchAuditGroup> AnalyzeSearchAudit(SearchAuditGroup searchAuditGroup)
		{
			// Get the details of the search from SQL
			foreach (var searchAudit in searchAuditGroup.Audits)
			{
				if (this.IsAdHocSearch(searchAudit.Audit))
				{
					// Search the queryText for like
				  var queryText = !string.IsNullOrEmpty(searchAudit.Audit.Details) ? this.auditParsingService.ParseRawQueryText(searchAudit.Audit.Details) : this.auditParsingService.ParseQueryText(searchAudit.Audit.ParsedDetails);
					searchAudit.IsComplex = this.IsSearchComplex(queryText);
				}
				else
				{
					// Get the saved search details that we can
					searchAudit.Search = await this.GetSearchDetails(searchAudit.Audit.ArtifactID, searchAudit.Audit.WorkspaceId);
					searchAudit.IsComplex = this.IsSearchComplex(searchAudit.Search);
				}
			}

			return searchAuditGroup;
		}

		internal bool IsAdHocSearch(Audit audit)
		{
			var search = this.viewRepository.ReadById(audit.WorkspaceId, audit.ArtifactID);
			return search == null;
		}

		internal bool IsSearchComplex(string queryText) => !string.IsNullOrEmpty(queryText) && (queryText.ComparisonContains(" LIKE '") || queryText.ComparisonContains(" LIKE N'"));

		internal bool IsSearchComplex(Search search) =>
			this.ScoreSearchDetails(search).IsComplexSearch;

		internal ComplexityScoreComponent ScoreSearchDetails(Search search) =>
			this.ScoreSearchDetails(search, new ComplexityScoreComponent());

		private async Task<Search> GetSearchDetails(int searchArtifactId, int workspaceId) =>
			await this.GetSearchDetails(searchArtifactId, workspaceId, new Dictionary<string, Search>());

		// Recursive for potential subsearches, arranged to deal with recursive loops
		private async Task<Search> GetSearchDetails(int searchArtifactId, int workspaceId, Dictionary<string, Search> dictionary)
		{
			// Get XML SearchText from the View table (SQL)
			await this.logger.LogVerboseAsync($"GetSearchDetails for ({searchArtifactId}, {workspaceId})");
			var search = this.viewRepository.ReadById(workspaceId, searchArtifactId);
			if (search == null)
			{
				await this.logger.LogVerboseAsync(@"Null search returned.  Ad Hoc search?");
				return null;
			}

			search.WorkspaceId = workspaceId;

			// Get all the Search ViewCriterias. (SQL) (Contains operators and values, potentially subsearches)
			var criterias = this.viewCriteriaRepository.ReadViewCriteriasForSearch(workspaceId, searchArtifactId);
			foreach (var crit in criterias)
			{
				if (crit.IsSubSearch && dictionary.ContainsKey(crit.Value))
				{
					crit.SubSearch = dictionary[crit.Value];
				}
				else if (crit.IsSubSearch)
				{
					var intCrit = int.Parse(crit.Value);
					crit.SubSearch = await this.GetSearchDetails(intCrit, workspaceId, dictionary);
					dictionary.Add(crit.Value, crit.SubSearch);
				}
			}

			search.Criterias = criterias;

			return search;
		}

		// Recursive for dealing with subsearches
		private ComplexityScoreComponent ScoreSearchDetails(Search search, ComplexityScoreComponent currentScore)
		{
			// Break out early if we're already complex enough
			if (currentScore.IsComplexSearch || search == null)
			{
				return currentScore;
			}

			// Determine if FullTextSearch
			if (search.SearchText?.Contains("SQLServer2005SearchProvider") ?? false)
			{
				currentScore.HasFullTextSearch = true;
			}

			// Determine if dtSearch
			var dtSearch = false;
			if (search.SearchText?.ToLower().Contains("dtsearch") ?? false)
			{
				// Set currentScore dtSearch flag, as well as the local scope for increasing the count on the correct character total
				currentScore.HasDTSearch = dtSearch = true;
			}

			// Count the number of 'like' operators in a search (include subsearches?)
			foreach (var crit in search.Criterias)
			{
				// Determine if FullTextSearch
				if (string.Equals(crit.Operator, "contains", StringComparison.InvariantCultureIgnoreCase))
				{
					currentScore.HasFullTextSearch = true;
				}

				// Aggregate number of 'like'/'non-like' operators
				else if (string.Equals(crit.Operator, "like", StringComparison.InvariantCultureIgnoreCase))
				{
					++currentScore.TotalLikes;
				}
				else if (!string.Equals(crit.Operator, "in", StringComparison.InvariantCultureIgnoreCase)
						&& !string.Equals(crit.Operator, "contains", StringComparison.InvariantCultureIgnoreCase))
				{
					++currentScore.TotalNonLikeOperators;
				}

				// Aggregate number of words in the Value property
				currentScore.TotalValueWords += crit.Value?.WordCount() ?? 0;

				// Aggregate number of characters
				if (dtSearch)
				{
					currentScore.TotalDTSearchCharacters += crit.Value?.Length ?? 0;
				}
				else
				{
					currentScore.TotalCharacters += crit.Value?.Length ?? 0;
				}

				// Aggregate the score of all the sub searches
				if (crit.IsSubSearch)
				{
					if (crit.SubSearch == null)
					{
						// Log warning when a recursive search is found. Otherwise, do nothing else.
						this.logger.LogWarning($"Found recursive search referenced. Search:{search.ArtifactId}, Workspace:{search.WorkspaceId}, SubSearch:{crit.Value}");
					}
					else
					{
						++currentScore.TotalSubsearches;
						currentScore = this.ScoreSearchDetails(crit.SubSearch, currentScore);
					}
				}

				if (currentScore.IsComplexSearch)
				{
					return currentScore;
				}
			}

			// Query the number of search folders
			currentScore.TotalSearchFolders += this.searchFolderRepository.GetSearchFolderCountForSearch(
				search.WorkspaceId,
				search.ArtifactId);
			return currentScore;
		}

		public static bool IsLongRunning(SearchAuditGroup searchAuditGroup) =>
			IsLongRunning(searchAuditGroup.IsComplex, searchAuditGroup.ExecutionTime);

		public static bool IsLongRunning(SearchAudit audit) =>
			IsLongRunning(audit.IsComplex, audit.Audit.ExecutionTime);

		internal static bool IsLongRunning(bool isComplex, long? executionTime) =>
			executionTime.HasValue && isComplex
			? executionTime > AuditConstants.LongRunningComplexThreshold
			: executionTime > AuditConstants.LongRunningSimpleThreshold;
	}
}
