namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class VersionCheckServiceTests
	{
		private VersionCheckService versionCheckService;
		private Mock<IPdbVersionRepository> pdbVersionRepositoryMock;
		private Mock<ILogger> loggerMock;

		[SetUp]
		public void SetUp()
		{
			this.pdbVersionRepositoryMock = new Mock<IPdbVersionRepository>(MockBehavior.Strict);
			this.loggerMock = TestUtilities.GetMockLogger();
			this.versionCheckService = new VersionCheckService(this.pdbVersionRepositoryMock.Object, this.loggerMock.Object);
		}

		[Test]
        [TestCase(null, null)]
        [TestCase("1.0.0.0", null)]
        [TestCase("1.0.0.0", "0.0.1.0")]

        public void UpdateLatestVersion_ThrowsException(string oldVersionString, string givenVersionString)
		{
		    // Arrange
		    var oldVersion = string.IsNullOrEmpty(oldVersionString) ? null : new Version(oldVersionString);
		    var givenVersion = string.IsNullOrEmpty(givenVersionString) ? null : new Version(givenVersionString);
		    this.pdbVersionRepositoryMock.Setup(m => m.GetLatestVersionAsync())
		        .ReturnsAsync(oldVersion);
		    if (givenVersion != null)
		    {
		        this.pdbVersionRepositoryMock.Setup(m => m.InitializeIfNotExists()).Returns(Task.Delay(1));
            }

            // Act
            // Assert
            Assert.ThrowsAsync<Exception>(() => this.versionCheckService.UpdateLatestVersion(givenVersion));
		}
		
		[Test]
		[TestCase(null, "1.0.1.0")]
		[TestCase("1.0.0.0", "1.0.1.0")]
		[TestCase("1.0.1.0", "1.0.1.0")]

		public async Task UpdateLatestVersion(string oldVersionString, string givenVersionString)
		{
			// Arrange
            var oldVersion = string.IsNullOrEmpty(oldVersionString) ? null : new Version(oldVersionString);
            var givenVersion = string.IsNullOrEmpty(givenVersionString) ? null : new Version(givenVersionString);
			this.pdbVersionRepositoryMock.Setup(m => m.GetLatestVersionAsync())
				.ReturnsAsync(oldVersion);
		    this.pdbVersionRepositoryMock.Setup(m => m.InitializeIfNotExists()).Returns(Task.Delay(1));
		    this.pdbVersionRepositoryMock.Setup(m => m.SetLatestVersionAsync(givenVersion)).Returns(Task.Delay(1));

			// Act
			await this.versionCheckService.UpdateLatestVersion(givenVersion);

			// Assert
            this.pdbVersionRepositoryMock.VerifyAll();
		}
        
        [Test]
        [TestCase(null, "1.0.0.0", false)]
        [TestCase("1.0.0.0", "1.0.0.0", true)]
        [TestCase("1.1.0.0", "1.0.0.0", true)]
        [TestCase("1.0.0.0", "1.1.0.0", false)]
        public async Task CurrentVersionIsInstalled(string oldVersionString, string givenVersionString, bool expectedResult)
	    {
	        // Arrange
	        var oldVersion = string.IsNullOrEmpty(oldVersionString) ? null : new Version(oldVersionString);
	        var givenVersion = string.IsNullOrEmpty(givenVersionString) ? null : new Version(givenVersionString);
	        this.pdbVersionRepositoryMock.Setup(m => m.GetLatestVersionAsync())
	            .ReturnsAsync(oldVersion);

	        var result = await this.versionCheckService.CurrentVersionIsInstalled(givenVersion);

            Assert.That(result, Is.EqualTo(expectedResult));
	    }

	    [Test]
	    [TestCase(null, null)]
	    public async Task CurrentVersionIsInstalled_Throws(string oldVersionString, string givenVersionString)
	    {
	        // Arrange
	        var oldVersion = string.IsNullOrEmpty(oldVersionString) ? null : new Version(oldVersionString);
	        var givenVersion = string.IsNullOrEmpty(givenVersionString) ? null : new Version(givenVersionString);
	        this.pdbVersionRepositoryMock.Setup(m => m.GetLatestVersionAsync())
	            .ReturnsAsync(oldVersion);

	        Assert.ThrowsAsync<ArgumentNullException>(() => this.versionCheckService.CurrentVersionIsInstalled(givenVersion));

	    }
    }
}
