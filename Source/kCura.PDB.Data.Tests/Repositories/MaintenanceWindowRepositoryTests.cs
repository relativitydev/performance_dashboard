namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using kCura.PDB.Core.Extensions;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class MaintenanceWindowRepositoryTests
	{
		[SetUp]
		public void SetUp()
		{
			this.hourRepository = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			this.maintenanceWindowRepository = new MaintenanceWindowRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private MaintenanceWindowRepository maintenanceWindowRepository;
		private HourRepository hourRepository;

		[Test]
		public async Task CreateAsync_Success()
		{
			//Arrange
			var window = new MaintenanceWindow
			{
				StartTime = DateTime.UtcNow.AddYears(-1),
				EndTime = DateTime.UtcNow.AddYears(-1).AddDays(2),
				Comments = "Test Comments",
				Reason = MaintenanceWindowReason.HardwareMigration
			};

			//Act
			var result = await this.maintenanceWindowRepository.CreateAsync(window);
			
			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StartTime, Is.EqualTo(window.StartTime).Within(5).Milliseconds);
			Assert.That(result.EndTime, Is.EqualTo(window.EndTime).Within(5).Milliseconds);
			Assert.That(result.Comments, Is.EqualTo(window.Comments));
			Assert.That(result.Reason, Is.EqualTo(window.Reason));
		}

		[Test]
		public async Task ReadAsync_ByID_Success()
		{
			//Arrange
			var window = new MaintenanceWindow
			{
				StartTime = DateTime.UtcNow.AddYears(-1),
				EndTime = DateTime.UtcNow.AddYears(-1).AddDays(2),
				Comments = "Test Comments",
				Reason = MaintenanceWindowReason.HardwareMigration
			};


			var createResult = await this.maintenanceWindowRepository.CreateAsync(window);
			var readId = createResult.Id;
			
			//Act
			var result = await this.maintenanceWindowRepository.ReadAsync(readId);
			
			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(createResult.Id));
			Assert.That(result.StartTime, Is.EqualTo(window.StartTime).Within(5).Milliseconds);
			Assert.That(result.EndTime, Is.EqualTo(window.EndTime).Within(5).Milliseconds);
			Assert.That(result.Comments, Is.EqualTo(window.Comments));
			Assert.That(result.Reason, Is.EqualTo(window.Reason));
		}

		[Test]
		public async Task ReadSortedAsync_Success()
		{
			//Arrange
			var startTime = DateTime.UtcNow.AddYears(-1);
			var endTime = DateTime.UtcNow.AddYears(-1).AddDays(2);
			var window = new MaintenanceWindow
			{
				StartTime = startTime,
				EndTime = endTime,
				Comments = "Test Comments",
				Reason = MaintenanceWindowReason.HardwareMigration
			};
			var query = new MaintenanceWindowDataTableQuery
			{
				CommentFilter = "Test Comments"
			};

			var createResult = await this.maintenanceWindowRepository.CreateAsync(window);

			// Act
			var result = await this.maintenanceWindowRepository.ReadSortedAsync(query);

			//Assert
			Assert.That(result.Any(r => r.Id == createResult.Id), Is.True);
		}

		[Test]
		public async Task UpdateAsync_Success()
		{
			//Arrange
			var window = new MaintenanceWindow
			{
				StartTime = DateTime.UtcNow.AddYears(-1),
				EndTime = DateTime.UtcNow.AddYears(-1).AddDays(2),
				Comments = "Test Comments - Update Test 1",
				Reason = MaintenanceWindowReason.HardwareMigration
			};

			var createResult = await this.maintenanceWindowRepository.CreateAsync(window);

			var updateWindow = new MaintenanceWindow
			{
				Id = createResult.Id,
				StartTime = createResult.StartTime.AddDays(-5),
				EndTime = createResult.EndTime.AddDays(-5),
				Comments = "Test Comments - Update Test 2",
				Reason = MaintenanceWindowReason.HardwareUpgrade
			};

			//Act
			await this.maintenanceWindowRepository.UpdateAsync(updateWindow);

			//Assert
			var readResult = await this.maintenanceWindowRepository.ReadAsync(updateWindow.Id);
			
			Assert.That(readResult, Is.Not.Null);
			Assert.That(readResult.Id, Is.EqualTo(updateWindow.Id));
			Assert.That(readResult.StartTime, Is.EqualTo(updateWindow.StartTime).Within(5).Milliseconds);
			Assert.That(readResult.EndTime, Is.EqualTo(updateWindow.EndTime).Within(5).Milliseconds);
			Assert.That(readResult.Reason, Is.EqualTo(updateWindow.Reason));
			Assert.That(readResult.Comments, Is.EqualTo(updateWindow.Comments));

			Assert.That(readResult.Id, Is.EqualTo(createResult.Id));
			Assert.That(readResult.StartTime, Is.Not.EqualTo(createResult.StartTime).Within(5).Milliseconds);
			Assert.That(readResult.EndTime, Is.Not.EqualTo(createResult.EndTime).Within(5).Milliseconds);
			Assert.That(readResult.Reason, Is.Not.EqualTo(createResult.Reason));
			Assert.That(readResult.Comments, Is.Not.EqualTo(createResult.Comments));
		}

		[Test]
		public async Task DeleteAsync_Success()
		{
			//Arrange
			var window = new MaintenanceWindow
			{
				StartTime = DateTime.UtcNow.AddYears(-1),
				EndTime = DateTime.UtcNow.AddYears(-1).AddDays(2),
				Comments = "Test Comments - Delete Test",
				Reason = MaintenanceWindowReason.HardwareMigration
			};

			var createResult = await this.maintenanceWindowRepository.CreateAsync(window);
			Assert.That(createResult, Is.Not.Null);
			Assert.That(createResult.IsDeleted, Is.False);

			// Act
			await this.maintenanceWindowRepository.DeleteAsync(createResult);
			
			//Assert
			var readResult = await this.maintenanceWindowRepository.ReadAsync(createResult.Id);

			Assert.That(readResult.IsDeleted, Is.True);
		}

		[Test]
		public async Task IsHourScheduledAsync_True()
		{
			//Arrange
			var startTime = DateTime.UtcNow.NormilizeToHour();
			var window = new MaintenanceWindow
			{
				StartTime = startTime,
				EndTime = startTime.AddHours(1),
				Comments = "Test Comments - Delete Test",
				Reason = MaintenanceWindowReason.HardwareUpgrade
			};

			var hour = new Hour
			{
				HourTimeStamp = startTime
			};
			hour = await this.hourRepository.CreateAsync(hour);

			await this.maintenanceWindowRepository.CreateAsync(window);

			// Act
			var result = await this.maintenanceWindowRepository.HourIsInMaintenanceWindowAsync(hour);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task IsHourScheduledAsync_False()
		{
			//Arrange
			var startTime = DateTime.UtcNow.AddYears(-100).NormilizeToHour();

			// Act
			var result = await this.maintenanceWindowRepository.HourIsInMaintenanceWindowAsync(new Hour { HourTimeStamp = startTime });

			// Assert
			Assert.That(result, Is.False);
		}
	}
}
