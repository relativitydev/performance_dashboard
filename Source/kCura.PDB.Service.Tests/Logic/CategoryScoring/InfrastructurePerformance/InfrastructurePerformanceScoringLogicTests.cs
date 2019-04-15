namespace kCura.PDB.Service.Tests.Logic.CategoryScoring.InfrastructurePerformance
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.CategoryScoring;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class InfrastructurePerformanceScoringLogicTests
	{
		private InfrastructurePerformanceScoringLogic logic;

		[SetUp]
		public void SetUp()
		{
			this.logic = new InfrastructurePerformanceScoringLogic();
		}

		[Test]
		public async Task ScoreMetrics()
		{
			// Arrange
			var categoryScore = new CategoryScore();
			var metricDatas = new List<MetricData>();

			// Act
			var result = await this.logic.ScoreMetrics(categoryScore, metricDatas);

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}
	}
}
