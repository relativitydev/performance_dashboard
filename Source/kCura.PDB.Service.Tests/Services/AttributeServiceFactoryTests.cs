namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class AttributeServiceFactoryTests
	{
		[SetUp]
		public void Setup()
		{
			metricLogic = new MockMetricLogic();
		}

		private IMetricLogic metricLogic;

		[Test]
		public void GetLogic_Success()
		{
			var list = new [] { metricLogic };
			var factory = new AttributeServiceFactory<IMetricLogic, MetricType>(list);
			var result = factory.GetService(MetricType.AgentUptime);

			Assert.That(result, Is.EqualTo(metricLogic));
		}

		[Test]
		public void GetLogic_InvalidMetric()
		{

			var list = new [] { metricLogic };
			var factory = new AttributeServiceFactory<IMetricLogic, string>(list);

			Assert.Throws<Exception>(() => factory.GetService(null));
		}

		[Test]
		public void GetLogic_NoMatchingLogic()
		{
			var list = new [] { metricLogic };
			var factory = new AttributeServiceFactory<IMetricLogic, MetricType>(list);

			//Act
			var result = factory.GetService(MetricType.WebUptime);

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void GetLogic_CantDetermineLogic()
		{
			var mockMetricLogic = new Mock<IMetricLogic>();
			var list = new [] { mockMetricLogic.Object };
			var factory = new AttributeServiceFactory<IMetricLogic, MetricType>(list);

			//Assert & Act
			Assert.Throws<Exception>(() => factory.GetService(MetricType.AgentUptime));
		}

		[MetricType(MetricType.AgentUptime)]
		public class MockMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
		{
			public Task<decimal> ScoreMetric(MetricData metricData)
			{
				throw new NotImplementedException();
			}

			public Task<object> CollectMetricData(MetricData metricData)
			{
				throw new NotImplementedException();
			}

			public Task<bool> IsReady(MetricData metricData)
			{
				throw new NotImplementedException();
			}
		}

	}
}
