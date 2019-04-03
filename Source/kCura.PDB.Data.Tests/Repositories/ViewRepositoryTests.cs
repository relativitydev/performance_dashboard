namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class ViewRepositoryTests
	{
		private ViewRepository viewRepository;

		[SetUp]
		public void SetUp()
		{
			this.viewRepository = new ViewRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test, Explicit("The given search is not guaranteed to exist on all instances/environments"), Category("Explicit")]
		public void GetSearchTextFromSearch()
		{
			// Arrange
			var searchArtifactId = 1037272;

			// Act
			var result = this.viewRepository.ReadById(Config.WorkSpaceId, searchArtifactId);

			// Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result, Is.Not.Null);
		}
	}
}
