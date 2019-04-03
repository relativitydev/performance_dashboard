namespace kCura.PDB.Service.Tests.Testing
{
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Service.Testing;
	using kCura.PDB.Tests.Common.Extensions;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class TestDataSetupServiceTests
	{
		private TestDataSetupService service;
		private Mock<ITestDataExcelParser> testDataExcelParser;

		private Mock<IBackupTestDataRepository> backupTestDataRepositoryMock;

		private Mock<IHourTestDataRepository> hourTestDataRepositoryMock;

		private Mock<IDbccTestDataRepository> dbccTestDataRepositoryMock;

		private Mock<IDatabasesCheckedTestDataRepository> databasesCheckedTestDataRepositoryMock;

		private Mock<IServerTestDataRepository> serverTestDataRepositoryMock;

		[SetUp]
		public void Setup()
		{
			this.testDataExcelParser = new Mock<ITestDataExcelParser>();
			this.backupTestDataRepositoryMock = new Mock<IBackupTestDataRepository>();
			this.hourTestDataRepositoryMock = new Mock<IHourTestDataRepository>();
			this.dbccTestDataRepositoryMock = new Mock<IDbccTestDataRepository>();
			this.databasesCheckedTestDataRepositoryMock = new Mock<IDatabasesCheckedTestDataRepository>();
			this.serverTestDataRepositoryMock = new Mock<IServerTestDataRepository>();
			this.service = new TestDataSetupService(
				this.testDataExcelParser.Object,
				this.backupTestDataRepositoryMock.Object,
				this.hourTestDataRepositoryMock.Object,
				this.dbccTestDataRepositoryMock.Object,
				this.databasesCheckedTestDataRepositoryMock.Object,
				this.serverTestDataRepositoryMock.Object);
		}

		[Test]
		public async Task SetupBackupDbccDataAsync()
		{
			// Arrange
			var testExcelData = new byte[0];
			var testData = new TestBackupDbccData();
			this.testDataExcelParser.Setup(m => m.ParseExcelBackupDbccData(testExcelData, false)).Returns(testData);

			this.backupTestDataRepositoryMock.Setup(m => m.ClearAsync()).ReturnsAsyncDefault();
			this.hourTestDataRepositoryMock.Setup(m => m.ClearAsync()).ReturnsAsyncDefault();
			this.dbccTestDataRepositoryMock.Setup(m => m.ClearAsync()).ReturnsAsyncDefault();
			this.databasesCheckedTestDataRepositoryMock.Setup(m => m.ClearAsync()).ReturnsAsyncDefault();
			this.serverTestDataRepositoryMock.Setup(m => m.ClearAsync()).ReturnsAsyncDefault();

			this.backupTestDataRepositoryMock.Setup(m => m.CreateAsync(testData.Backups)).ReturnsAsyncDefault();
			this.hourTestDataRepositoryMock.Setup(m => m.CreateAsync(testData.Hours)).ReturnsAsyncDefault();
			this.dbccTestDataRepositoryMock.Setup(m => m.CreateAsync(testData.DbccResults)).ReturnsAsyncDefault();
			this.databasesCheckedTestDataRepositoryMock.Setup(m => m.CreateAsync(testData.DatabasesChecked)).ReturnsAsyncDefault();
			this.serverTestDataRepositoryMock.Setup(m => m.CreateAsync(testData.Servers)).ReturnsAsyncDefault();

			// Act
			var result = await this.service.SetupBackupDbccDataAsync(testExcelData);

			Assert.That(result, Is.EqualTo(testData));
			this.backupTestDataRepositoryMock.VerifyAll();
			this.hourTestDataRepositoryMock.VerifyAll();
			this.dbccTestDataRepositoryMock.VerifyAll();
			this.databasesCheckedTestDataRepositoryMock.VerifyAll();
			this.serverTestDataRepositoryMock.VerifyAll();
		}
	}
}
