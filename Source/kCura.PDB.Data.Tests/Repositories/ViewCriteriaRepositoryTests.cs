namespace kCura.PDB.Data.Tests.Repositories
{
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class ViewCriteriaRepositoryTests
	{
		private ViewCriteriaRepository viewCriteriaRepository;

		[SetUp]
		public void SetUp()
		{
			this.viewCriteriaRepository = new ViewCriteriaRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test, Explicit("The given search is not guaranteed to exist on all instances/environments"), Category("Explicit")]
		public void ReadViewCriteriasForSearch()
		{
			// Arrange
			var workspaceId = Config.WorkSpaceId;
			var searchArtifactId = 1065483;

			// Act
			var result = this.viewCriteriaRepository.ReadViewCriteriasForSearch(workspaceId, searchArtifactId);

			// Assert
			Assert.That(result, Is.Not.Empty);
		}
	}
}
