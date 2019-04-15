namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;

	using Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class AgentHistoryRepositoryTests
	{
		[OneTimeSetUp]
		public async Task SetUp()
		{
			var history = new AgentHistory
			{
				AgentArtifactId = 3,
				TimeStamp = DateTime.UtcNow,
				Successful = false
			};

			agentHistoryRepository = new AgentHistoryRepository(ConnectionFactorySetup.ConnectionFactory);
			agentHistory = await agentHistoryRepository.CreateAsync(history);
			agentHistory.Successful = true;
			await agentHistoryRepository.UpdateAsync(agentHistory);
			agentHistory = await agentHistoryRepository.ReadAsync(agentHistory.Id);
		}

		private AgentHistory agentHistory;
		private AgentHistoryRepository agentHistoryRepository;

		[Test]
		public void AgentHistory_CreateAsync_Success()
		{
			//Assert
			Assert.That(agentHistory, Is.Not.Null);
			Assert.That(agentHistory.Id, Is.GreaterThan(0));
			Assert.That(agentHistory.AgentArtifactId, Is.EqualTo(3));
			Assert.That(agentHistory.TimeStamp.Date, Is.Not.Null);
			Assert.That(agentHistory.Successful, Is.EqualTo(true));
		}

		[Test]
		public void AgentHistory_ReadAsync_ByID_Success()
		{
			//Assert
			Assert.That(agentHistory, Is.Not.Null);
			Assert.That(agentHistory.Id, Is.GreaterThan(0));
			Assert.That(agentHistory.AgentArtifactId, Is.EqualTo(3));
			Assert.That(agentHistory.TimeStamp.Date, Is.Not.Null);
			Assert.That(agentHistory.Successful, Is.EqualTo(true));
		}

		[Test]
		public async Task AgentHistory_ReadByHourAsync_Success()
		{
			//Arrange
			var hourRepo = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			var hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() });

			//Act
			var result = await agentHistoryRepository.ReadByHourAsync(hour);

			//Assert
			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public async Task AgentHistory_ReadEarliestAsync_Success()
		{
			//Act
			var result = await agentHistoryRepository.ReadEarliestAsync();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void AgentHistory_UpdateAsync_Success()
		{
			//Assert
			Assert.That(agentHistory, Is.Not.Null);
			Assert.That(agentHistory.Successful, Is.EqualTo(true));
		}

		[Test]
		public async Task AgentHistory_ZDeleteAsync_Success()
		{
			//Act
			await agentHistoryRepository.DeleteAsync(agentHistory);

			//Assert
			var result = await agentHistoryRepository.ReadAsync(agentHistory.Id);
			Assert.That(result, Is.Null);
		}
	}
}
