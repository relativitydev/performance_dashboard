namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class HourRepositoryTests
	{
		[OneTimeSetUp]
		public async Task SetUp()
		{
			var nowHour = DateTime.UtcNow.NormilizeToHour();
			repo = new HourRepository(ConnectionFactorySetup.ConnectionFactory);

			hour = await repo.CreateAsync(new Hour
			{
				HourTimeStamp = nowHour,
				Score = 100.0m,
				InSample = true
			});


			hour.Score = 75.0m;
			await repo.UpdateAsync(hour);
			hour = await repo.ReadAsync(hour.Id);
		}

		private Hour hour;
		private HourRepository repo;

		[Test]
		public void Hour_CreateAsync_Success()
		{
			//Assert
			Assert.That(hour, Is.Not.Null);
			Assert.That(hour.Id, Is.GreaterThan(0));
		}

		[Test]
		public void Hour_CreateList_Success()
		{
			// Act
			var hours = repo.Create(new List<Hour>()
			{
				new Hour { HourTimeStamp = DateTime.UtcNow.AddYears(-2).AddHours(-1).NormilizeToHour() },
				new Hour { HourTimeStamp = DateTime.UtcNow.AddYears(-2).AddHours(-2).NormilizeToHour() }
			});

			// Assert
			Assert.That(hours.Count, Is.EqualTo(2));
		}

		[Test]
		public void Hour_CreateList_EmptyList()
		{
			// Act
			var hours = repo.Create(new List<Hour>());

			// Assert
			Assert.That(hours, Is.Empty);
		}

		[Test]
		public void Hour_ReadAsync_ByID_Success()
		{
			//Assert
			Assert.That(hour, Is.Not.Null);
			Assert.That(hour.Id, Is.GreaterThan(0));
			Assert.That(hour.HourTimeStamp.Date, Is.GreaterThan(DateTime.MinValue));
			Assert.That(hour.InSample, Is.EqualTo(true));
			Assert.That(hour.Score, Is.EqualTo(75.0m));
		}

		[Test]
		public async Task Hour_ReadLastAsync()
		{
			//Act
			var result = await repo.ReadLastAsync();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task Hour_ReadBackFillHoursAsync()
		{
			//Act
			var result = await repo.ReadBackFillHoursAsync();

			//Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Any(r => r.Id == this.hour.Id), Is.True);
		}

		[Test]
		public async Task Hour_ReadPastWeekHoursAsync()
		{
			//Act
			var result = await repo.ReadPastWeekHoursAsync(hour);

			//Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Any(r => r.Id == this.hour.Id), Is.True);
		}

		[Test]
		public void Hour_UpdateAsync_Success()
		{
			//Assert
			Assert.That(hour.Score, Is.EqualTo(75.0m));
		}

		[Test]
		public async Task Hour_ZDeleteAsync_Success()
		{
			// Act
			await repo.DeleteAsync(hour);

			//Assert
			var readResult = await repo.ReadAsync(hour.Id);

			Assert.That(readResult, Is.Null);
		}

		[Test]
		public async Task Hour_ReadHighestHourAfterMinHour()
		{
			// Act
			var result = await repo.ReadHighestHourAfterMinHour();

			//Assert
			Assert.Pass("Could return result or null depending on if there is an hour at sql datetime min date.");
		}

		[Test]
		public async Task Hour_ReadCompletedBackFillHoursAsync()
		{
			// Act
			var result = await repo.ReadCompletedBackFillHoursAsync();

			//Assert
			Assert.Pass("Could return result or empty depending on if there is a complete hour.");
		}

		[Test]
		public async Task Hour_ReadNextHourWithoutRatings()
		{
			// Act
			var result = await repo.ReadNextHourWithoutRatings();

			//Assert
			Assert.Pass("Could return result or null depending on if there is a qos_ratings for all hours.");
		}
	}
}
