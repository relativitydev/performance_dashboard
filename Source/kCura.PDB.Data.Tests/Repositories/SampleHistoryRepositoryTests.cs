namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class SampleHistoryRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetUp()
		{
			this.sampleHistoryRepository = new SampleHistoryRepository(ConnectionFactorySetup.ConnectionFactory);
			serverId = 0;
			startSampleHistories = Enumerable.Range(0, 16).Select(i =>
				new SampleHistory
				{
					ServerId = serverId,
					HourId = i,
					IsActiveArrivalRateSample = true,
					IsActiveConcurrencySample = true
				}).ToList();
			await this.sampleHistoryRepository.AddToCurrentSampleAsync(startSampleHistories); // Ensure that we add something?
		}

		private SampleHistoryRepository sampleHistoryRepository;
		private int serverId;
		private IList<SampleHistory> startSampleHistories;
		private IList<SampleHistory> resetArrivalRateSampleHistories;
		private IList<SampleHistory> resetConcurrencySampleHistories;

		[Test]
		public async Task ResetCurrentSampleAsync()
		{
			// Arrange
			// Act
			await this.sampleHistoryRepository.ResetCurrentSampleAsync(serverId);

			// Assert
			resetArrivalRateSampleHistories = await this.sampleHistoryRepository.ReadCurrentArrivalRateSampleAsync(serverId);
			resetConcurrencySampleHistories = await this.sampleHistoryRepository.ReadCurrentConcurrencySampleAsync(serverId);

			Assert.That(resetArrivalRateSampleHistories, Is.Empty);
			Assert.That(resetConcurrencySampleHistories, Is.Empty);
		}

		[Test]
		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		[TestCase(true, true)]
		public async Task AddToCurrentSampleAsync(bool isActiveArrivalRateSample, bool isActiveConcurrencySample)
		{
			// Arrange
			await this.sampleHistoryRepository.ResetCurrentSampleAsync(serverId); // Reset so our data is consistent
			var sampleHistory = new SampleHistory
			{
				ServerId = serverId,
				HourId = startSampleHistories[0].HourId,
				IsActiveArrivalRateSample = isActiveArrivalRateSample,
				IsActiveConcurrencySample = isActiveConcurrencySample
			};

			// Act
			await this.sampleHistoryRepository.AddToCurrentSampleAsync(sampleHistory);

			// Assert
			var resultList = await this.sampleHistoryRepository.ReadCurrentArrivalRateSampleAsync(serverId);
			if (isActiveArrivalRateSample)
			{
				Assert.That(resultList, Is.Not.Empty);
				AssertCompareResult(resultList.First(), sampleHistory);
			}
			else
			{
				Assert.That(resultList, Is.Empty);
			}

			resultList = await this.sampleHistoryRepository.ReadCurrentConcurrencySampleAsync(serverId);
			if (isActiveConcurrencySample)
			{
				Assert.That(resultList, Is.Not.Empty);
				AssertCompareResult(resultList.First(), sampleHistory);
			}
			else
			{
				Assert.That(resultList, Is.Empty);
			}
		}

		[Test]
		public async Task AddToCurrentSampleAsync_List()
		{
			// Arrange
			await this.sampleHistoryRepository.ResetCurrentSampleAsync(serverId); // Reset so our data is consistent
			var sampleHistory = startSampleHistories[0];
			var sampleHistories = new List<SampleHistory> {sampleHistory};

			// Act
			await this.sampleHistoryRepository.AddToCurrentSampleAsync(sampleHistories);

			// Assert
			var resultList = await this.sampleHistoryRepository.ReadCurrentArrivalRateSampleAsync(serverId);
			var result = resultList.First();
			Assert.That(resultList, Is.Not.Empty);
			AssertCompareResult(result, sampleHistory);
		}

		[Test]
		public async Task ReadCurrentArrivalRateSampleAsync()
		{
			// Arrange
			// Act
			var result = await this.sampleHistoryRepository.ReadCurrentArrivalRateSampleAsync(serverId);

			// Assert
			Assert.Pass($"Results not guaranteed, Result count: {result.Count}, {result}");
		}

		[Test]
		public async Task ReadCurrentConcurrencySampleAsync()
		{
			// Arrange
			// Act
			var result = await this.sampleHistoryRepository.ReadCurrentConcurrencySampleAsync(serverId);

			// Assert
			Assert.Pass($"Results not guaranteed, Result count: {result.Count}, {result}");
		}

		[Test]
		public async Task SampleHistory_RemoveHourFromSampleAsync()
		{
			// Arrange
			// Act
			await this.sampleHistoryRepository.RemoveHourFromSampleAsync(4);

			// Assert
			Assert.Pass($"Results not guaranteed");
		}

		private static void AssertCompareResult(SampleHistory result, SampleHistory expectedResult)
		{
			Assert.That(result.ServerId, Is.EqualTo(expectedResult.ServerId));
			Assert.That(result.IsActiveArrivalRateSample, Is.EqualTo(expectedResult.IsActiveArrivalRateSample));
			Assert.That(result.IsActiveConcurrencySample, Is.EqualTo(expectedResult.IsActiveConcurrencySample));
			Assert.That(result.HourId, Is.EqualTo(expectedResult.HourId));
		}
	}
}
