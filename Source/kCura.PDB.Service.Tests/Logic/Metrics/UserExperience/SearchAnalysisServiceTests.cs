namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Audits;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class SearchAnalysisServiceTests
	{
		private SearchAnalysisService searchAnalysisService;
		private Mock<IViewCriteriaRepository> viewCriteriaRepositoryMock;
		private Mock<IViewRepository> viewRepositoryMock;
		private Mock<ISearchFolderRepository> searchFolderRepositoryMock;
		private Mock<ILogger> loggerMock;
		private Mock<IAuditParsingService> auditParsingServiceMock;

		[SetUp]
		public void SetUp()
		{
			this.viewCriteriaRepositoryMock = new Mock<IViewCriteriaRepository>();
			this.viewRepositoryMock = new Mock<IViewRepository>();
			this.searchFolderRepositoryMock = new Mock<ISearchFolderRepository>();
			this.loggerMock = new Mock<ILogger>();
			this.auditParsingServiceMock = new Mock<IAuditParsingService>();
			this.searchAnalysisService = new SearchAnalysisService(
				this.viewCriteriaRepositoryMock.Object,
				this.viewRepositoryMock.Object,
				this.searchFolderRepositoryMock.Object,
				this.loggerMock.Object,
				this.auditParsingServiceMock.Object);
		}

		[Test]
		public void IsAdHocSearch_True_NoSavedSearch()
		{
			// Arrange
			var audit = new Audit();

			// Act
			var result = this.searchAnalysisService.IsAdHocSearch(audit);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public void IsAdHocSearch_False()
		{
			// Arrange
			var audit = new Audit();
			var search = new Search();
			this.viewRepositoryMock.Setup(m => m.ReadById(audit.WorkspaceId, audit.ArtifactID)).Returns(search);

			// Act
			var result = this.searchAnalysisService.IsAdHocSearch(audit);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		[TestCase("Select * FROM eddsdbo.TestTable WHERE x = 14", false)]
		[TestCase("Select * FROM eddsdbo.TestLikeTable WHERE x = 14", false)]
		[TestCase("Select * FROM eddsdbo.TestTable WHERE x is like '%testing%'", true)]
		[TestCase("Select * FROM eddsdbo.TestTable WHERE x is like N''%testing%'", true)]
		// [TestCase("Select * FROM eddsdbo.TestTable WHERE x is\r\nlike\r\n'%testing%'", true)] // Didn't support these cases in the past, but may want to in the future
		// [TestCase("Select * FROM eddsdbo.TestTable WHERE x is\tlike\t'%testing%'", true)]

		public void IsSearchComplex_AdHoc(string queryText, bool expectedResult)
		{
			// Arrange
			// Act
			var result = this.searchAnalysisService.IsSearchComplex(queryText);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public async Task GetAnalyzedSearch_True()
		{
			// Arrange
			var viewCriteria = new List<ViewCriteria> { new ViewCriteria { Operator = "like", Value = "%Something really cool%" } };
			var searchAudit = new Audit { ArtifactID = 1 };
			var searchAuditGroup = new SearchAuditGroup { Audits = new List<SearchAudit> { new SearchAudit { Audit = searchAudit } } };
			var searchText = "";
			this.viewCriteriaRepositoryMock.Setup(m => m.ReadViewCriteriasForSearch(searchAudit.WorkspaceId, searchAudit.ArtifactID))
				.Returns(viewCriteria);
			this.viewRepositoryMock.Setup(
				m => m.ReadById(searchAudit.WorkspaceId, searchAudit.ArtifactID)).Returns(new Search { ArtifactId = 1, SearchText = searchText });
			this.searchFolderRepositoryMock.Setup(
					m => m.GetSearchFolderCountForSearch(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(0);

			// Act
			var result = await this.searchAnalysisService.AnalyzeSearchAudit(searchAuditGroup);

			// Assert
			Assert.That(result.IsComplex, Is.True);
		}

		[Test]
		public async Task GetAnalyzedSearch_False()
		{
			// Arrange
			var viewCriteria = new List<ViewCriteria> { };
			var searchAudit = new Audit { ArtifactID = 1 };
			var searchAuditGroup = new SearchAuditGroup { Audits = new List<SearchAudit> { new SearchAudit { Audit = searchAudit } } };
			var searchText = "";
			this.viewCriteriaRepositoryMock.Setup(m => m.ReadViewCriteriasForSearch(searchAudit.WorkspaceId, searchAudit.ArtifactID))
				.Returns(viewCriteria);
			this.viewRepositoryMock.Setup(
				m => m.ReadById(searchAudit.WorkspaceId, searchAudit.ArtifactID)).Returns(new Search { ArtifactId = 1, SearchText = searchText });
			this.searchFolderRepositoryMock.Setup(
					m => m.GetSearchFolderCountForSearch(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(0);

			// Act
			var result = await this.searchAnalysisService.AnalyzeSearchAudit(searchAuditGroup);

			// Assert
			Assert.That(result.IsComplex, Is.False);
		}

		[Test, Explicit("TODO - Provide search and expected search complexity test cases"), Category("Explicit")]
		[TestCaseSource(nameof(ScoreSearchDetailsData))]
		public void ScoreSearchDetails_OneLevel(Search searchToScore, int searchFolderCount, ComplexityScoreComponent expectedComplexity)
		{
			this.searchFolderRepositoryMock.Setup(
					m => m.GetSearchFolderCountForSearch(searchToScore.WorkspaceId, searchToScore.ArtifactId))
				.Returns(searchFolderCount);

			var result = this.searchAnalysisService.ScoreSearchDetails(searchToScore);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalLikes, Is.EqualTo(expectedComplexity.TotalLikes));
			Assert.That(result.TotalValueWords, Is.EqualTo(expectedComplexity.TotalValueWords));
			Assert.That(result.TotalCharacters, Is.EqualTo(expectedComplexity.TotalCharacters));
			Assert.That(result.TotalDTSearchCharacters, Is.EqualTo(expectedComplexity.TotalDTSearchCharacters));
			Assert.That(result.TotalSubsearches, Is.EqualTo(expectedComplexity.TotalSubsearches));
			Assert.That(result.TotalNonLikeOperators, Is.EqualTo(expectedComplexity.TotalNonLikeOperators));
			Assert.That(result.TotalSearchFolders, Is.EqualTo(expectedComplexity.TotalSearchFolders));
			Assert.That(result.HasDTSearch, Is.EqualTo(expectedComplexity.HasDTSearch));
			Assert.That(result.HasFullTextSearch, Is.EqualTo(expectedComplexity.HasFullTextSearch));
			Assert.That(result.HasInOrContainsOperator, Is.EqualTo(expectedComplexity.HasInOrContainsOperator));
			Assert.That(result.TotalScore, Is.EqualTo(expectedComplexity.TotalScore));
			Assert.That(result.IsComplexSearch, Is.EqualTo(expectedComplexity.IsComplexSearch));
		}

		[Test]
		[TestCase(false, 0, false)]
		[TestCase(false, 2001, true)]
		[TestCase(false, 8001, true)]
		[TestCase(true, 0, false)]
		[TestCase(true, 2001, false)]
		[TestCase(true, 8001, true)]
		public void IsLongRunning(bool isComplex, long executionTime, bool expectedResult)
		{
			// Arrange
			var audit = new Audit { ExecutionTime = executionTime };
			var search = new Search();
			var searchAudit = new SearchAudit { Search = search, Audit = audit, IsComplex = isComplex };

			// Act
			var result = SearchAnalysisService.IsLongRunning(searchAudit);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		public static IEnumerable<TestCaseData> ScoreSearchDetailsData
		{
			get
			{
				var searchOne = new Search { Criterias = new ViewCriteria[] { } };
				var searchFoldersOne = 0;
				var resultsOne = new ComplexityScoreComponent();
				yield return new TestCaseData(searchOne, searchFoldersOne, resultsOne).SetName("Test One");
			}
		}

		[Test, Explicit("TODO - Figure out what to assert")]
		public void GetSearchDetails()
		{
			// Arrange
			var searchArtifactId = 1;
			var workspaceId = 2;

			var searchText = "";
			this.viewRepositoryMock.Setup(m => m.ReadById(workspaceId, searchArtifactId)).Returns(new Search { SearchText = searchText });

			var viewCriteria = new List<ViewCriteria> { };
			this.viewCriteriaRepositoryMock.Setup(m => m.ReadViewCriteriasForSearch(searchArtifactId, searchArtifactId))
				.Returns(viewCriteria);

			// Act
			//var result = this.searchAnalysisService.GetSearchDetails(searchArtifactId, workspaceId);

			// Assert

		}
	}
}
