namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.DatabaseDeployment;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class RoundhouseLoggerTests
	{
		private RelativityRoundHouseLogger logger;

		public const string Message = "Party like it's {0}";

		[SetUp]
		public void BeforeRun()
		{
			logger = new RelativityRoundHouseLogger();
		}

		[Test]
		public void LogEvent_Debug()
		{
			//Arrange

			//Act
			logger.log_a_debug_event_containing(Message, 2001);

			//Assert
			Assert.AreEqual(1, logger.Messages.Count);
			Assert.AreEqual(LogSeverity.Debug, logger.Messages.First().Severity);
			Assert.AreEqual(String.Format(Message, 2001), logger.Messages.First().Message);
			Assert.AreEqual("DEBUG: " + String.Format(Message, 2001), logger.Messages.First().ToString());
		}

		[Test]
		public void LogEvent_Fatal()
		{
			//Arrange

			//Act
			logger.log_a_fatal_event_containing(Message, 1984);

			//Assert
			Assert.AreEqual(1, logger.Messages.Count);
			Assert.AreEqual(LogSeverity.Fatal, logger.Messages.First().Severity);
			Assert.AreEqual(String.Format(Message, 1984), logger.Messages.First().Message);
			Assert.AreEqual("FATAL: " + String.Format(Message, 1984), logger.Messages.First().ToString());
		}

		[Test]
		public void LogEvent_Warning()
		{
			//Arrange

			//Act
			logger.log_a_warning_event_containing(Message, 1776);

			//Assert
			Assert.AreEqual(1, logger.Messages.Count);
			Assert.AreEqual(LogSeverity.Warning, logger.Messages.First().Severity);
			Assert.AreEqual(String.Format(Message, 1776), logger.Messages.First().Message);
			Assert.AreEqual("WARNING: " + String.Format(Message, 1776), logger.Messages.First().ToString());
		}

		[Test]
		public void LogEvent_Error()
		{
			//Arrange

			//Act
			logger.log_an_error_event_containing(Message, "going out of style");

			//Assert
			Assert.AreEqual(1, logger.Messages.Count);
			Assert.AreEqual(LogSeverity.Error, logger.Messages.First().Severity);
			Assert.AreEqual(String.Format(Message, "going out of style"), logger.Messages.First().Message);
			Assert.AreEqual("ERROR: " + String.Format(Message, "going out of style"), logger.Messages.First().ToString());
		}

		[Test]
		public void LogEvent_Info()
		{
			//Arrange

			//Act
			logger.log_an_info_event_containing(Message, 12345);

			//Assert
			Assert.AreEqual(1, logger.Messages.Count);
			Assert.AreEqual(LogSeverity.Info, logger.Messages.First().Severity);
			Assert.AreEqual(String.Format(Message, 12345), logger.Messages.First().Message);
			Assert.AreEqual("INFO: " + String.Format(Message, 12345), logger.Messages.First().ToString());
		}

		[Test]
		public void Logger_NoEventsCaptured()
		{
			//Arrange

			//Act

			//Assert
			Assert.AreEqual(0, logger.Messages.Count);
		}

		[Test]
		public void Logger_MultipleEventsCaptured()
		{
			//Arrange

			//Act
			logger.log_an_info_event_containing("Doing some stuff");
			logger.log_an_info_event_containing("Doing more stuff");
			logger.log_a_fatal_event_containing("Couldn't keep doing stuff because: {0}", "Y2K happened");

			//Assert
			Assert.AreEqual(3, logger.Messages.Count);
		}
	}
}
