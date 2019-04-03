namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Models;
	using Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class LogRepositoryTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			this.logRepository = new LogRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private LogRepository logRepository;

		[Test]
		public void LogRepository_Create()
		{
			// Arrange
			var logEntry = new LogEntry { Module = "Log Te'st", TaskCompleted = @"Cre""ate log entry", AgentId = 3, NextTask = "Ne'xt Log Test", LogLevel = 2 };

			// Act
			this.logRepository.Create(logEntry);

			//Assert
			Assert.Pass();
		}

		[Test]
		public void LogRepository_Create_LargeMessage()
		{
			// Arrange
			var module = new string(Enumerable.Range(0, 10 * 1000 * 1000).Select(i => 'a').ToArray());
			var taskCompleted = new string(Enumerable.Range(0, 10 * 1000 * 1000).Select(i => 'b').ToArray());
			var otherVars = new string(Enumerable.Range(0, 10 * 1000 * 1000).Select(i => 'c').ToArray());
			var nextTask = new string(Enumerable.Range(0, 10 * 1000 * 1000).Select(i => 'd').ToArray());
			var logEntry = new LogEntry { Module = module, TaskCompleted = taskCompleted, OtherVars = otherVars, AgentId = 3, NextTask = nextTask, LogLevel = 2 };

			// Act
			this.logRepository.Create(logEntry);

			//Assert
			Assert.Pass();
		}

		[Test]
		public async Task LogRepository_ReadLast()
		{
			// Act
			var result = await this.logRepository.ReadLastAsync();

			//Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
