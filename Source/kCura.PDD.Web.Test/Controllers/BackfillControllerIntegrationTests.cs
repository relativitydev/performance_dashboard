namespace kCura.PDD.Web.Test.Controllers
{
	using System.Threading.Tasks;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using kCura.PDD.Web.Controllers;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class BackfillControllerIntegrationTests
	{
		private BackfillController backfillController;

		[SetUp]
		public void SetUp()
		{
			var sqlRepo = new SqlServerRepository(TestUtilities.GetIntegrationConnectionFactory());
			this.backfillController = new BackfillController(sqlRepo);
		}

		[Test]
		public async Task GetBackfillStatus()
		{
			var result = await this.backfillController.GetBackfillStatus();

			Assert.That(result, Is.Not.Null);
		}
	}
}
