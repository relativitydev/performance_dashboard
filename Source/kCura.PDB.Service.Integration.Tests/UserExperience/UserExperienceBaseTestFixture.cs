namespace kCura.PDB.Service.Integration.Tests.UserExperience
{
	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using NUnit.Framework;
	using System.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Service.CategoryScoring;
	using kCura.PDB.Service.Hours;
	using kCura.PDB.Service.Metrics.UserExperience;
	using Ninject;

	public class UserExperienceBaseTestFixture : BaseServiceIntegrationTest
	{

		protected Metric UxMetric;
		protected MetricData UxMetricData;
		protected Category UxCategory;
		protected CategoryScore UxCategoryScore;
		
		protected IAuditBatchProcessor batchProcessor;
		protected IMetricLogic auditAnalysisMetricLogic;
		protected ICategoryScoringLogic userExperienceScoringLogic;
		protected IHourlyScoringLogic hourlyScoringLogic;
		protected IAuditServerBatcher serverBatcher;

		public void SetUpData()
		{
			// Arrange Data by grabbing the AuditAnalysis (UX) specific metric data
			this.UxMetric = this.Metrics.First(m => m.MetricType == MetricType.AuditAnalysis);
			//var uxMetricData = this.MetricDatas.Where(md => md.MetricId == uxMetric.Id).ToList();
			this.UxMetricData = this.MetricDatas.First(md => md.MetricId == this.UxMetric.Id);
			this.UxCategory = this.Categories.First(c => c.CategoryType == CategoryType.UserExperience);
			this.UxCategoryScore = this.CategoryScores.First(cs => cs.CategoryId == this.UxCategory.Id);
		}

		public void SetupServices()
		{
			this.auditAnalysisMetricLogic = this.Kernel.Get<AuditAnalysisMetricLogic>();
			this.userExperienceScoringLogic = this.Kernel.Get<UserExperienceScoringLogic>();
			this.batchProcessor = this.Kernel.Get<AuditBatchProcessor>();
			this.hourlyScoringLogic = this.Kernel.Get<HourlyScoringLogic>();
			this.serverBatcher = this.Kernel.Get<AuditServerBatcher>();
		}

		protected async Task SearchAuditAnalysis_Success()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			Console.WriteLine($@"Search Audit Analysis started for hour (ID:  {Hour.Id}, TimeStamp: {Hour.HourTimeStamp}");

			// Act
			var batchIds = await this.serverBatcher.CreateServerBatches(UxMetricData.Id);
			Console.WriteLine($@"Number of batchIds created - {batchIds.Count}");
			//Assert.That(batchIds, Is.Not.Empty, "Batch Ids should not be empty");
			// BatchIds can be empty if there was no audits for that hour.
			if (batchIds.Any())
			{
				Assert.That(batchIds.All(id => id > 0), Is.True, "Batch Ids should not be zero");

				var batchResults = await batchIds.Select(bid => this.batchProcessor.ProcessBatch(bid)).WhenAllStreamed();
				Assert.That(batchResults, Is.Not.Empty, "Batch processor results should not be empty");
				Assert.That(batchResults.All(br => br.Count > 0), Is.True, "Batch processor results count should be greater than zero");
			}
			
			stopWatch.Stop();
			Console.WriteLine($@"Search Audit Analysis completed in {stopWatch.Elapsed.TotalSeconds} seconds");
		}

		protected async Task UserExperienceEndToEnd_Success()
		{
			// Act
			await SearchAuditAnalysis_Success();

			var data = await this.auditAnalysisMetricLogic.CollectMetricData(UxMetricData);
			//update metric data with result data

			var metricScore = await this.auditAnalysisMetricLogic.ScoreMetric(UxMetricData);
			Assert.That(metricScore, Is.GreaterThanOrEqualTo(0), "Metric Data score should be greater than or equal to zero");

			var categoryScore = await this.userExperienceScoringLogic.ScoreMetrics(UxCategoryScore, new[] { UxMetricData });
			Assert.That(categoryScore, Is.GreaterThanOrEqualTo(0), "Category Score should be greater than or equal to zero");

			var hourScore = await this.hourlyScoringLogic.ScoreHour(this.Hour);
			Assert.That(hourScore, Is.GreaterThanOrEqualTo(0), "Hour Score should be greater than or equal to zero");

			// Assert
			Console.WriteLine($@"MetricScore - {metricScore}, CategoryScore - {categoryScore}");
		}
	}
}
