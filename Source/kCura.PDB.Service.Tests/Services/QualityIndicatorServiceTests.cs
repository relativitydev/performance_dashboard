namespace kCura.PDB.Service.Tests.Services
{
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class QualityIndicatorServiceTests
	{
		private Mock<IConfigurationRepository> configurationRepository;
		private IQualityIndicatorConfigurationService indicatorConfigurationService;
		private Mock<QualityIndicatorService> qualityIndicatorService;

		[SetUp]
		public void Setup()
		{
			this.configurationRepository = new Mock<IConfigurationRepository>();
			this.indicatorConfigurationService = new QualityIndicatorConfigurationService(this.configurationRepository.Object);
			this.qualityIndicatorService = new Mock<QualityIndicatorService>(this.indicatorConfigurationService);
		}

	
		[TestCase(105, null, null, QualityIndicator.Pass),
		TestCase(91, null, null, QualityIndicator.Pass),
		TestCase(90, null, null, QualityIndicator.Pass),
		TestCase(89, null, null, QualityIndicator.Warn),
		TestCase(80, null, null, QualityIndicator.Warn),
		TestCase(79, null, null, QualityIndicator.Fail),
		TestCase(20, null, null, QualityIndicator.Fail),
		TestCase(0, null, null, QualityIndicator.Fail),
		TestCase(-90, null, null, QualityIndicator.None)]
		public void GetIndicatorForScore_WithDefaultValues(int score, int? passScore, int? warnScore, QualityIndicator expectedResult)
		{
			// Arrange
			this.configurationRepository.Setup(r => r.ReadValue<int>(ConfigurationKeys.PassScore)).Returns(passScore);
			this.configurationRepository.Setup(r => r.ReadValue<int>(ConfigurationKeys.WarnScore)).Returns(warnScore);
			// Act
			var result = this.qualityIndicatorService.Object.GetIndicatorForScore(score);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));

		}

		[TestCase(105, 50, 50, QualityIndicator.Pass),
		TestCase(91, 50, 50, QualityIndicator.Pass),
		TestCase(90, 50, 50, QualityIndicator.Pass),
		TestCase(89, 50, 40, QualityIndicator.Pass),
		TestCase(45, 50, 40, QualityIndicator.Warn),
		TestCase(40, 50, 40, QualityIndicator.Warn),
		TestCase(50, 50, 49, QualityIndicator.Pass),
		TestCase(20, 50, 50, QualityIndicator.Fail),
		TestCase(0, 50, 50, QualityIndicator.Fail),
		TestCase(-90, 50, 50, QualityIndicator.None)]
		public void GetIndicatorForScore_WithCustomValues(int score, int? passScore, int? warnScore, QualityIndicator expectedResult)
		{

			// Arrange
			this.configurationRepository.Setup(r => r.ReadValue<int>(ConfigurationKeys.PassScore)).Returns(passScore);
			this.configurationRepository.Setup(r => r.ReadValue<int>(ConfigurationKeys.WarnScore)).Returns(warnScore);

			//Act
			var result = this.qualityIndicatorService.Object.GetIndicatorForScore(score);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));

		}
		
		[TestCase(10, true, "failBackground"),
		TestCase(10, false, "failText"),
		TestCase(80, true, "warnBackground"),
		TestCase(80, false, "warnText"),
		TestCase(100, true, "passBackground"),
		TestCase(100, false, "passText"),
		TestCase(-1, true, "noneBackground"),
		TestCase(-1, false, "noneText"),
		TestCase(0, true, "failBackground"),
		TestCase(0, true, "failBackground")]
		public void GetCSSClassForScore_WithDefaultValues(int score, bool isBackground, string expectedResult)
		{

			var result = this.qualityIndicatorService.Object.GetCssClassForScore(score, isBackground);

			Assert.That(result, Is.EqualTo(expectedResult));
		}

	}
}
