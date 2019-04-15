namespace kCura.PDB.Service.Tests.Testing
{
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Testing;
	using Moq;
	using NUnit.Framework;
	using System;
	using System.Threading.Tasks;

	[TestFixture]
	[Category("Unit")]
	public class NoOpTaskTesting
	{
		[SetUp]
		public void Setup()
		{
			this.eventRepository = new Mock<IEventRepository>();
			this.noOpTask = new NoOpTask(this.eventRepository.Object);
		}

		private Mock<IEventRepository> eventRepository;
		private NoOpTask noOpTask;

		[Test]
		[TestCase(null)]
		[TestCase(1)]
		[TestCase(NoOpTask.NumberOfFailsExepected - 1)]
		public void FailsThenSucceeds_Fails(int? retryCount)
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadAsync(1234))
				.ReturnsAsync(new Event { Id = 1234, Retries = retryCount });

			// Act & Assert
			Assert.ThrowsAsync<Exception>(()=> this.noOpTask.FailsThenSucceeds(1234));
		}


		[Test]
		[TestCase(NoOpTask.NumberOfFailsExepected + 1)]
		[TestCase(NoOpTask.NumberOfFailsExepected + 1000)]
		public async Task FailsThenSucceeds_Succeeds(int? retryCount)
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadAsync(1234))
				.ReturnsAsync(new Event { Id = 1234, Retries = retryCount });

			// Act & Assert
			await this.noOpTask.FailsThenSucceeds(1234);
		}
	}
}
