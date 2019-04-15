namespace kCura.PDB.Service.Tests.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.RecoverabilityIntegrity;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class GapsCollectionVerifierTests
	{
		private GapsCollectionVerifier gapsCollectionVerifier;
		private Mock<IEventRepository> eventRepository;
		private Mock<IMetricDataRepository> metricDataRepository;

		[SetUp]
		public void Setup()
		{
			this.eventRepository = new Mock<IEventRepository>();
			this.metricDataRepository = new Mock<IMetricDataRepository>();
			this.gapsCollectionVerifier = new GapsCollectionVerifier(this.metricDataRepository.Object, this.eventRepository.Object);
		}

		[Test]
		[TestCase(EventStatus.Completed, true)]
		[TestCase(EventStatus.Error, false)]
		public async Task GapsCollectionVerifier_VerifyGapsCollected_DifferentMetricData(EventStatus status, bool expectedResult)
		{
			// Arrange
			var hour = new Hour();
			var server = new Server();
			this.metricDataRepository.Setup(r => r.ReadByHourAndMetricTypeAsync(hour, server, MetricType.BackupGaps))
				.ReturnsAsync(new MetricData { Id = 1234 });
			this.eventRepository.Setup(r => r.ReadLastBySourceIdAndTypeAsync(
					EventSourceType.ProcessRecoverabilityForServer,
					1234))
				.ReturnsAsync(new Event { Status = status });

			// Act
			var result = await this.gapsCollectionVerifier.VerifyGapsCollected(new MetricData { Id = 555, Metric = new Metric { Hour = hour }, Server = server });

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(EventStatus.Completed, true)]
		[TestCase(EventStatus.Error, false)]
		public async Task GapsCollectionVerifier_VerifyGapsCollected_SameMetricData(EventStatus status, bool expectedResult)
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadLastBySourceIdAndTypeAsync(
					EventSourceType.ProcessRecoverabilityForServer,
					555))
				.ReturnsAsync(new Event { Status = status });

			// Act
			var result = await this.gapsCollectionVerifier.VerifyGapsCollected(555);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
