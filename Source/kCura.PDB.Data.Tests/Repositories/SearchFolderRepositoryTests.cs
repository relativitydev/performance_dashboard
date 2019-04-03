namespace kCura.PDB.Data.Tests.Repositories
{
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class SearchFolderRepositoryTests
	{
		private SearchFolderRepository searchFolderRepository;

		[SetUp]
		public void SetUp()
		{
			this.searchFolderRepository = new SearchFolderRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test, Explicit, Category("Explicit")]
		public void GetSearchFolderCountForSearch()
		{
			// Arrange
			var searchArtifactId = 1;

			// Act
			var result = this.searchFolderRepository.GetSearchFolderCountForSearch(Config.WorkSpaceId, searchArtifactId);

			// Assert
			Assert.That(result, Is.GreaterThan(0));
		}
	}
}
