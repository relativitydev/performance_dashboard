namespace kCura.PDD.Web.Test.Controllers
{
	using System;
	using global::Relativity.API;
	using global::Relativity.Services.Credential;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Tests.Common;
	using kCura.PDD.Web.Controllers;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class ConfigurationControllerTests
	{
		[SetUp]
		public void SetUp()
		{
			this.pdbConfigurationService = new Mock<IPdbConfigurationService>();
			this.configurationController = new ConfigurationController(this.pdbConfigurationService.Object);
		}

		private Mock<IPdbConfigurationService> pdbConfigurationService;
		private ConfigurationController configurationController;

		[Test]
		public void GetSettings()
		{
			// Arrange
			this.pdbConfigurationService.Setup(s => s.GetConfiguration())
				.Returns(new PerformanceDashboardConfigurationSettings());

			// Act
			var result = this.configurationController.GetSettings();

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		// TODO Add other tests
	}
}
