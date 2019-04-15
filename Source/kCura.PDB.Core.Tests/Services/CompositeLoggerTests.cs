namespace kCura.PDB.Core.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class CompositeLoggerTests
	{
		[SetUp]
		public void SetUp()
		{
			loggers = Enumerable.Range(0, 3)
				.Select(i => new Mock<ILogger>())
				.ForEach(ml => ml.Setup(l => l.LogCritical("", "")))
				.ForEach(ml => ml.Setup(l => l.LogCritical("", new List<string>())))
				.ForEach(ml => ml.Setup(l => l.LogError("", "")))
				.ForEach(ml => ml.Setup(l => l.LogError("", It.IsAny<Exception>(), "")))
				.ForEach(ml => ml.Setup(l => l.LogError("", It.IsAny<Exception>(), new List<string>())))
				.ForEach(ml => ml.Setup(l => l.LogError("", new List<string>())))
				.ForEach(ml => ml.Setup(l => l.LogInformation("", "")))
				.ForEach(ml => ml.Setup(l => l.LogInformation("", new List<string>())))
				.ForEach(ml => ml.Setup(l => l.LogVerbose("", "")))
				.ForEach(ml => ml.Setup(l => l.LogVerbose("", new List<string>())))
				.ForEach(ml => ml.Setup(l => l.LogWarning("", "")))
				.ForEach(ml => ml.Setup(l => l.LogWarning("", new List<string>())))
				.ToList();
			this.compositeLogger = new CompositeLogger(loggers.Select(ml => ml.Object).ToList());
		}

		private IEnumerable<Mock<ILogger>> loggers;
		private ILogger compositeLogger;

		[Test]
		public void CompositeLogger_LogAll()
		{
			// Arrange

			// Act
			this.compositeLogger.LogCritical("", "");
			this.compositeLogger.LogCritical("", new List<string>());
			this.compositeLogger.LogError("", "");
			this.compositeLogger.LogError("", new Exception(), "");
			this.compositeLogger.LogError("", new Exception(), new List<string>());
			this.compositeLogger.LogError("", new List<string>());
			this.compositeLogger.LogInformation("", "");
			this.compositeLogger.LogInformation("", new List<string>());
			this.compositeLogger.LogVerbose("", "");
			this.compositeLogger.LogVerbose("", new List<string>());
			this.compositeLogger.LogWarning("", "");
			this.compositeLogger.LogWarning("", new List<string>());

			// Assert
			loggers
				.ForEach(ml => ml.Verify(l => l.LogCritical("", ""), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogCritical("", new List<string>()), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogError("", ""), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogError("", It.IsAny<Exception>(), ""), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogError("", It.IsAny<Exception>(), new List<string>()), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogError("", new List<string>()), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogInformation("", ""), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogInformation("", new List<string>()), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogVerbose("", ""), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogVerbose("", new List<string>()), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogWarning("", ""), Times.Once()))
				.ForEach(ml => ml.Verify(l => l.LogWarning("", new List<string>()), Times.Once()));
		}
	}
}
