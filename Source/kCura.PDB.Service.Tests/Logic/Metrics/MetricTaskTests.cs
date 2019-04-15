namespace kCura.PDB.Service.Tests.Logic.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Service.Metrics;
	using Moq;
	using NUnit.Framework;
	using kCura.PDB.Tests.Common;

	[TestFixture, Category("Unit")]
	public class MetricTaskTests
	{
		[SetUp]
		public void Setup()
		{
			this.metricFactory = new Mock<IServiceFactory<IMetricLogic, MetricType>>();
			this.isReadyMetricFactory = new Mock<IServiceFactory<IMetricReadyForDataCollectionLogic, MetricType>>();
			hourRepository = new Mock<IHourRepository>();
			metricDataRepository = new Mock<IMetricDataRepository>();
			metricDataService = new Mock<IMetricDataService>();
			mockMetricLogic = new Mock<IMetricLogic>();
			logger = TestUtilities.GetMockLogger();

			//Arrange
			metricData = new MetricData
			{
				MetricId = 444,
				ServerId = 555,
				Metric = new Metric
				{
					MetricType = MetricType.AuditAnalysis,
					HourId = 222,
					Hour = new Hour { HourTimeStamp = DateTime.UtcNow.AddSeconds(-1) }
				}
			};
			metricDataRepository.Setup(mr => mr.ReadAsync(metricDataId)).ReturnsAsync(metricData);
			hourRepository.Setup(h => h.ReadAsync(222)).ReturnsAsync(new Hour { HourTimeStamp = DateTime.UtcNow.AddSeconds(2) });
			mockMetricLogic.Setup(ml => ml.CollectMetricData(metricData)).ReturnsAsync(metricData);
			metricReadyForDataCollectionLogic = new Mock<IMetricReadyForDataCollectionLogic>();
			metricReadyForDataCollectionLogic.Setup(l => l.IsReady(It.IsAny<MetricData>())).ReturnsAsync(true);
			metricDataRepository.Setup(mdr => mdr.UpdateAsync(It.IsAny<MetricData>())).Returns(Task.Delay(10));
			this.metricDataService.Setup(s => s.GetMetricData(metricDataId)).ReturnsAsync(metricData);

			this.metricTask = new MetricTask(
				metricDataRepository.Object,
				metricFactory.Object,
				isReadyMetricFactory.Object,
				metricDataService.Object,
				logger.Object);
		}

		private Mock<IServiceFactory<IMetricLogic, MetricType>> metricFactory;
		private Mock<IServiceFactory<IMetricReadyForDataCollectionLogic, MetricType>> isReadyMetricFactory;
		private Mock<IHourRepository> hourRepository;
		private Mock<IMetricDataRepository> metricDataRepository;
		private Mock<IMetricDataService> metricDataService;
		private Mock<IMetricLogic> mockMetricLogic;
		private Mock<IMetricReadyForDataCollectionLogic> metricReadyForDataCollectionLogic;
		private Mock<ILogger> logger;
		private int metricDataId = 123;
		private MetricTask metricTask;
		private MetricData metricData;

		[Test]
		public async Task CollectMetricData()
		{
			// Arrange
			metricFactory.Setup(mf => mf.GetService(MetricType.AuditAnalysis)).Returns(mockMetricLogic.Object);

			// Act
			var result = await this.metricTask.CollectMetricData(metricDataId);

			// Assert
			mockMetricLogic.Verify(ml => ml.CollectMetricData(It.IsAny<MetricData>()));
			metricDataRepository.Verify(mdr => mdr.UpdateAsync(It.IsAny<MetricData>()));
			Assert.That(result.Types.First(), Is.EqualTo(EventSourceType.ScoreMetricData));
		}

		[Test]
		public async Task CollectMetricData_WaitTillMetricHour()
		{
			// Arrange
			var metricData = new MetricData { MetricId = 444, ServerId = 555, Metric = new Metric { HourId = 222, Hour = new Hour { HourTimeStamp = DateTime.UtcNow.AddHours(1) } } };
			this.metricDataService.Setup(s => s.GetMetricData(metricDataId)).ReturnsAsync(metricData);
			metricFactory.Setup(mf => mf.GetService(MetricType.AuditAnalysis)).Returns(mockMetricLogic.Object);

			// Act
			var result = await this.metricTask.CollectMetricData(metricDataId);

			// Assert
			Assert.That(result.Types.First(), Is.EqualTo(EventSourceType.CollectMetricData));
		}

		[Test]
		public async Task CollectMetricData_NoImplementation()
		{
			// Arrange
			this.metricFactory.Setup(f => f.GetService(MetricType.AuditAnalysis)).Returns((IMetricLogic)null);

			// Act
			var result = await this.metricTask.CollectMetricData(metricDataId);

			// Assert
			this.logger.Verify(l => l.LogVerboseAsync(It.IsAny<string>(), It.IsAny<List<string>>()));
			mockMetricLogic.Verify(ml => ml.CollectMetricData(It.IsAny<MetricData>()), Times.Never());
			Assert.That(result.Types.First(), Is.EqualTo(EventSourceType.ScoreMetricData));
		}

		[Test]
		public void CollectMetricData_Error()
		{
			// Arrange
			this.metricDataService.Setup(r => r.GetMetricData(555)).Throws(new Exception("test error"));

			// Act
			Assert.ThrowsAsync<Exception>(()=> this.metricTask.CollectMetricData(555));

			// Assert
			logger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<List<string>>()));
		}

		[Test]
		public async Task ScoreMetric()
		{
			// Arrange
			metricFactory.Setup(mf => mf.GetService(MetricType.AuditAnalysis)).Returns(mockMetricLogic.Object);

			// Act
			await this.metricTask.ScoreMetric(metricDataId);

			// Assert
			mockMetricLogic.Verify(ml => ml.ScoreMetric(It.IsAny<MetricData>()));
			metricDataRepository.Verify(mdr => mdr.UpdateAsync(It.IsAny<MetricData>()));
		}

		[Test]
		public async Task ScoreMetric_NoImplementation()
		{
			// Arrange
			this.metricFactory.Setup(f => f.GetService(MetricType.AuditAnalysis)).Returns((IMetricLogic)null);

			// Act
			await this.metricTask.ScoreMetric(metricDataId);

			// Assert
			this.logger.Verify(l => l.LogVerboseAsync(It.IsAny<string>(), It.IsAny<List<string>>()));
			mockMetricLogic.Verify(ml => ml.CollectMetricData(It.IsAny<MetricData>()), Times.Never());
		}

		[Test]
		public void ScoreMetric_Error()
		{
			// Arrange
			this.metricDataService.Setup(r => r.GetMetricData(555)).Throws(new Exception("test error"));

			// Act & Assert
			Assert.ThrowsAsync<Exception>(() => this.metricTask.ScoreMetric(555));
		}

		[Test]
		public async Task CheckMetricReadyForDataCollection()
		{
			// Arrange
			this.isReadyMetricFactory.Setup(mf => mf.GetService(MetricType.AuditAnalysis))
				.Returns(metricReadyForDataCollectionLogic.Object);

			// Act
			var result = await this.metricTask.CheckMetricReadyForDataCollection(metricDataId);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task CheckMetricReadyForDataCollection_NoImplementation()
		{
			// Arrange
			this.isReadyMetricFactory.Setup(f => f.GetService(MetricType.AuditAnalysis))
				.Returns((IMetricReadyForDataCollectionLogic)null);

			// Act
			var result = await this.metricTask.CheckMetricReadyForDataCollection(metricDataId);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task StartPrerequisitesForMetricData()
		{
			// Arrange
			this.metricData.Metric.MetricType = MetricType.AuditAnalysis;
			this.isReadyMetricFactory.Setup(mf => mf.GetService(MetricType.AuditAnalysis))
				.Returns(metricReadyForDataCollectionLogic.Object);

			// Act
			var result = await this.metricTask.StartPrerequisitesForMetricData(metricDataId);

			// Assert
			Assert.That(result.Types.Count, Is.EqualTo(MetricConstants.MetricPrerequisites[MetricType.AuditAnalysis].Count() + 1));
			Assert.That(result.Types.Contains(EventSourceType.CheckMetricDataIsReadyForDataCollection), Is.True);
		}

		[Test,
			TestCase(SampleType.Continuously, 1, EventSourceType.CheckSamplingPeriodForMetricData),
			TestCase(SampleType.Continuously, 0, EventSourceType.StartPrerequisitesForMetricData),
			TestCase(SampleType.Continuously, -1, EventSourceType.StartPrerequisitesForMetricData),
			TestCase(SampleType.Hourly, 1, EventSourceType.CheckSamplingPeriodForMetricData),
			TestCase(SampleType.Hourly, 0, EventSourceType.CheckSamplingPeriodForMetricData),
			TestCase(SampleType.Hourly, -1, EventSourceType.StartPrerequisitesForMetricData)]
		public async Task CheckSamplingPeriodForMetricData_NotInSamplingPeriod(SampleType sampleType, int additionalHours, EventSourceType expectedEventType)
		{
			// Arrange
			this.metricData.Metric.SampleType = sampleType;
			this.metricData.Metric.Hour.HourTimeStamp = DateTime.UtcNow.NormilizeToHour().AddHours(additionalHours);

			// Act
			var result = await this.metricTask.CheckSamplingPeriodForMetricData(metricDataId);

			// Assert
			Assert.That(result.Types.First(), Is.EqualTo(expectedEventType));
		}

		[Test,
			TestCase(SampleType.Continuously, 1, false),
			TestCase(SampleType.Continuously, 0, true),
			TestCase(SampleType.Continuously, -1, true),
			TestCase(SampleType.Hourly, 1, false),
			TestCase(SampleType.Hourly, 0, false),
			TestCase(SampleType.Hourly, -1, true)]
		public void IsDuringOrAfterSamplePeriod(SampleType sampleType, int additionalHours, bool expectedResult)
		{
			// Arrange
			var metric = new Metric
			{
				SampleType =  sampleType,
				Hour = new Hour
				{
					HourTimeStamp = DateTime.UtcNow.NormilizeToHour().AddHours(additionalHours)
				}
			};

			// Act
			var result = MetricTask.IsDuringOrAfterSamplePeriod(metric);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
