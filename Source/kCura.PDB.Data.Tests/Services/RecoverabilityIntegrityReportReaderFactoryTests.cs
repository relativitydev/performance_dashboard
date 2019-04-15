namespace kCura.PDB.Data.Tests.Services
{
	using global::Relativity.Toggles;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Data.Services;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class RecoverabilityIntegrityReportReaderFactoryTests
	{
		private RecoverabilityIntegrityReportReaderFactory factory;

		private Mock<IToggleProvider> toggleProviderMock;
		private Mock<IConnectionFactory> connectionFactoryMock;

		[SetUp]
		public void Setup()
		{
			this.toggleProviderMock = new Mock<IToggleProvider>();
			this.connectionFactoryMock = new Mock<IConnectionFactory>();
			this.factory = new RecoverabilityIntegrityReportReaderFactory(this.toggleProviderMock.Object, this.connectionFactoryMock.Object);
		}

		[Test]
		public void Get_ReturnRepository()
		{
			this.toggleProviderMock.Setup(m => m.IsEnabled<RecoverabilityIntegrityMetricSystemToggle>()).Returns(true);

			var repository = this.factory.Get();
			Assert.That(repository, Is.TypeOf<RecoverabilityIntegrityReportRepository>());
		}

		[Test]
		public void Get_ReturnLegacyRepository()
		{
			this.toggleProviderMock.Setup(m => m.IsEnabled<RecoverabilityIntegrityMetricSystemToggle>()).Returns(false);

			var repository = this.factory.Get();
			Assert.That(repository, Is.TypeOf<LegacyRecoverabilityIntegrityReportRepository>());
		}
	}
}
