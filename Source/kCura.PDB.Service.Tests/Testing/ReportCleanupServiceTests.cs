namespace kCura.PDB.Service.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Service.Testing;
	using kCura.PDB.Tests.Common.Extensions;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class ReportCleanupServiceTests
	{
		private ReportCleanupService reportCleanupService;
		private Mock<IReportCleanupRepository> reportCleanupRepositoryMock;

		[SetUp]
		public void Setup()
		{
			this.reportCleanupRepositoryMock = new Mock<IReportCleanupRepository>();
			this.reportCleanupService = new ReportCleanupService(this.reportCleanupRepositoryMock.Object);
		}

		[Test]
		public async Task ClearReportData()
		{
			var testData = new List<DateTime>();
			this.reportCleanupRepositoryMock.Setup(m => m.ClearReportData(testData)).ReturnsAsyncDefault();
			this.reportCleanupRepositoryMock.Setup(m => m.ClearExistingBackupHistory()).ReturnsAsyncDefault();
			this.reportCleanupRepositoryMock.Setup(m => m.ClearExistingDbccHistory()).ReturnsAsyncDefault();

			await this.reportCleanupService.ClearReportDataAsync(testData);

			this.reportCleanupRepositoryMock.VerifyAll();
			this.reportCleanupRepositoryMock.Verify(m => m.ClearReportData(testData), Times.Once);
			this.reportCleanupRepositoryMock.Verify(m => m.ClearExistingBackupHistory(), Times.Once);
			this.reportCleanupRepositoryMock.Verify(m => m.ClearExistingDbccHistory(), Times.Once);
		}
	}
}
