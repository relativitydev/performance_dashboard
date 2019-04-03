namespace kCura.PDB.Service.Integration.Tests.HourTests
{
	using System.Linq;
	using kCura.PDB.Core.Models;
	using NUnit.Framework;

	[TestFixture, Category("Integration"), Explicit]
	public class HourMetricAndCategoryDataCreated : BaseServiceIntegrationTest
	{
		[Test]
		public void CreateHourData()
		{
			// Assert
			Assert.That(this.Hour, Is.Not.Null, "Hour should not be null");
			Assert.That(this.Metrics, Is.Not.Empty, "Metrics should not empty");
			Assert.That(this.MetricDatas, Is.Not.Empty, "Metric Datas should not empty");
			Assert.That(this.MetricDatas.Any(md => md.Metric.MetricType == MetricType.AuditAnalysis && md.ServerId.HasValue), Is.True, "Metric Datas for audit analysis should have a server id");
			Assert.That(this.Categories, Is.Not.Empty, "Categories should not empty");
			Assert.That(this.CategoryScores, Is.Not.Empty, "Category Scores should not empty");
		}
	}
}
