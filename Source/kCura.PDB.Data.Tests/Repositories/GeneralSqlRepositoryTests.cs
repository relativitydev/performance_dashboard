namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class GeneralSqlRepositoryTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			this.connectionFactory = ConnectionFactorySetup.ConnectionFactory;
			this.generalSqlRepository = new GeneralSqlRepository();
		}

		private IConnectionFactory connectionFactory;
		private GeneralSqlRepository generalSqlRepository;

		[Test]
		public async Task GetSqlTimezoneOffset()
		{
			// Arrange
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{

				// Act
				var result = await this.generalSqlRepository.GetSqlTimezoneOffset(conn);

				// Assert
				Assert.That(result, Is.EqualTo((DateTime.Now - DateTime.UtcNow).TotalMinutes));
			}
		}
	}
}
