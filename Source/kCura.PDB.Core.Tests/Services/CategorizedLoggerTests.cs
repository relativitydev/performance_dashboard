namespace kCura.PDB.Core.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class CategorizedLoggerTests
	{
		[SetUp]
		public void Setup()
		{
			mockLogger = TestUtilities.GetMockLogger();
		}

		private Mock<ILogger> mockLogger;

		[Test]
		public void CategorizedLogger_LogAll()
		{
			// Arrange
			var loggerCategory = "test category";
			var category = "abc";
			var logger = new CategorizedLogger(mockLogger.Object, loggerCategory);

			// Act
			logger.LogCritical("", category);
			logger.LogCritical("", new List<string>() { category });
			logger.LogError("", category);
			logger.LogError("", new Exception(), category);
			logger.LogError("", new Exception(), new List<string>() { category });
			logger.LogError("", new List<string>() { category });
			logger.LogInformation("", category);
			logger.LogInformation("", new List<string>() { category });
			logger.LogVerbose("", category);
			logger.LogVerbose("", new List<string>() { category });
			logger.LogWarning("", category);
			logger.LogWarning("", new List<string>() { category });

			// Assert
			Expression<Func<List<string>, bool>> expectedList = lst => lst[0] == loggerCategory && lst[1] == category && lst.Count == 2;
			mockLogger.Verify(l => l.LogCritical(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(2));
			mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(2));
			mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.Is(expectedList)), Times.Exactly(2));
			mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(2));
			mockLogger.Verify(l => l.LogVerbose(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(2));
			mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(2));
		}

		[Test]
		public void CategorizedLogger_LogAll_WithNullCategories()
		{
			// Arrange
			var loggerCategory = "test category";
			var logger = new CategorizedLogger(mockLogger.Object, loggerCategory);

			// Act
			logger.LogCritical("");
			logger.LogError("");
			logger.LogError("", new Exception());
			logger.LogInformation("");
			logger.LogVerbose("");
			logger.LogWarning("");

			// Assert
			Expression<Func<List<string>, bool>> expectedList = lst => lst[0] == loggerCategory && lst.Count == 1;
			mockLogger.Verify(l => l.LogCritical(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(1));
			mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(1));
			mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.Is(expectedList)), Times.Exactly(1));
			mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(1));
			mockLogger.Verify(l => l.LogVerbose(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(1));
			mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.Is(expectedList)), Times.Exactly(1));
		}
	}
}
