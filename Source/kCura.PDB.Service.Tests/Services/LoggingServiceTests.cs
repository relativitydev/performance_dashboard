namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Reflection;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Logging;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class LoggingServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.configurationRepository = new Mock<IConfigurationRepository>();
			this.logRepository = new Mock<ILogRepository>();
		}

		private Mock<IConfigurationRepository> configurationRepository;
		private Mock<ILogRepository> logRepository;

		[TestCase(null, LogLevel.Warnings),
			TestCase("", LogLevel.Warnings),
			TestCase("errors", LogLevel.Errors),
			TestCase("error", LogLevel.Errors),
			TestCase("asdfasdfasf", LogLevel.Warnings),
			TestCase("warn", LogLevel.Warnings),
			TestCase("warning", LogLevel.Warnings),
			TestCase("warnings", LogLevel.Warnings),
			TestCase("info", LogLevel.Verbose),
			TestCase("information", LogLevel.Verbose),
			TestCase("verbose", LogLevel.Verbose),
			TestCase("neverlog", LogLevel.NeverLog),
			TestCase("none", LogLevel.NeverLog)]
		public void GetLogLevel(string logLevelConfigValue, LogLevel expectedResult)
		{
			// Arrange
			this.configurationRepository
				.SetupSequence(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LogLevel))
				.Returns(logLevelConfigValue) // used for when to constructor calls GetLogLevel
				.Returns(logLevelConfigValue)
				.Returns(logLevelConfigValue?.ToUpper());

			// Act
			var loggingService = new LogService(this.configurationRepository.Object, this.logRepository.Object);
			var resultLower = loggingService.GetLogLevel();
			var resultUpper = loggingService.GetLogLevel();

			// Assert
			Assert.That(resultLower, Is.EqualTo(expectedResult));
			Assert.That(resultUpper, Is.EqualTo(expectedResult));
		}

		[Test]
		public void GetLogLevel_Error()
		{
			// Arrange
			this.configurationRepository
				.SetupSequence(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LogLevel))
				.Returns("errors") // used for when to constructor calls GetLogLevel
				.Throws(CreateSqlException(1));

			// Act
			var loggingService = new LogService(this.configurationRepository.Object, this.logRepository.Object);
			var result = loggingService.GetLogLevel();

			// Assert
			Assert.That(result, Is.EqualTo(LogLevel.NeverLog));
		}

		[Test,
			TestCase(5, LogLevel.NeverLog, false),
			TestCase(5, LogLevel.Errors, false),
			TestCase(5, LogLevel.Warnings, true),
			TestCase(5, LogLevel.Verbose, true),

			TestCase(1, LogLevel.NeverLog, false),
			TestCase(1, LogLevel.Errors, true),
			TestCase(1, LogLevel.Warnings, true),
			TestCase(1, LogLevel.Verbose, true)]
		public void ShouldLogFromLogLevel(int level, LogLevel configuredLogLevel, bool expectedResult)
		{
			// Arrange
			this.logRepository.Setup(r => r.ReadCategories()).Returns(new[] { new LogCategory { Name = "abc", LogLevel = 10 } });
			this.configurationRepository
				.Setup(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LogLevel))
				.Returns(configuredLogLevel.ToString());

			// Act
			var loggingService = new LogService(this.configurationRepository.Object, this.logRepository.Object);
			var result = loggingService.ShouldLogFromLogLevel(level);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test,
			TestCase(5, new[] { "abc" }, true),
			TestCase(5, new[] { "xyz" }, false),
			TestCase(7, new[] { "123" }, false),
			TestCase(6, new string[] { }, false)]
		public void ShouldLogFromLogCategories(int level, string[] logCategories, bool expectedResult)
		{
			// Arrange
			this.logRepository.Setup(r => r.ReadCategories()).Returns(new[]
			{
				new LogCategory { Name = "abc", LogLevel = 10 },
				new LogCategory { Name = "xyz", LogLevel = 2 }
			});

			// Act
			var loggingService = new LogService(this.configurationRepository.Object, this.logRepository.Object);
			var result = loggingService.ShouldLogFromLogCategories(level, logCategories);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}


		private SqlException CreateSqlException(int number)
		{
			var collectionConstructor = typeof(SqlErrorCollection)
			.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
			var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
			var errorCollection = (SqlErrorCollection)collectionConstructor.Invoke(null);
			var errorConstructor = typeof(SqlError).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
			new[] { typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string), typeof(string), typeof(int), typeof(uint) }, null);
			var error =
			errorConstructor.Invoke(new object[] { number, (byte)0, (byte)0, "server", "errMsg", "proccedure", 100, (uint)0 });
			addMethod.Invoke(errorCollection, new[] { error });
			var constructor = typeof(SqlException)
			.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string), typeof(SqlErrorCollection), typeof(Exception), typeof(Guid) }, null);
			return (SqlException)constructor.Invoke(new object[] { "Error message", errorCollection, new DataException(), Guid.NewGuid() });
		}
	}
}
