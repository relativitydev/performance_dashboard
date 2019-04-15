namespace kCura.PDD.Web.Test.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDD.Web.Enum;
	using kCura.PDD.Web.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class MaintenanceWindowSchedulingServiceTests
	{
		[SetUp]
		public void SetUp()
		{
			this.maintenanceWindowRepositoryMock = new Mock<IMaintenanceWindowRepository>(MockBehavior.Strict);
			this.maintenanceWindowSchedulingService = new MaintenanceWindowSchedulingService(this.maintenanceWindowRepositoryMock.Object);
		}

		private Mock<IMaintenanceWindowRepository> maintenanceWindowRepositoryMock;
		private MaintenanceWindowSchedulingService maintenanceWindowSchedulingService;

		[Test]
		public async Task ScheduleMaintenanceWindowAsync_Success()
		{
			//Arrange
			var window = new MaintenanceWindow();
			var createdWindow = new MaintenanceWindow();
			this.maintenanceWindowRepositoryMock.Setup(m => m.CreateAsync(window)).ReturnsAsync(createdWindow);

			//Act
			var result = await this.maintenanceWindowSchedulingService.ScheduleMaintenanceWindowAsync(window);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.EqualTo(createdWindow));
		}

		[Test]
		public async Task GetFilteredMaintenanceWindowsAsync_Success()
		{
			//Arrange
			var query = new MaintenanceWindowDataTableQuery();
			var windows = new List<MaintenanceWindow>
			{
				new MaintenanceWindow()
			};

			this.maintenanceWindowRepositoryMock.Setup(mock => mock.ReadSortedAsync(query)).ReturnsAsync(windows);

			//Act
			var results = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);

			//Assert
			Assert.That(results.Data, Is.Not.Empty);
		}

		[Test]
		public async Task GetFilteredMaintenanceWindowsAsync_SortByStartDate_Success()
		{
			//Arrange
			var query = new MaintenanceWindowDataTableQuery
			{
				SortColumn = MaintenanceWindowViewColumns.StartTime.ToString()
			};
			var aWindow = new MaintenanceWindow
			{
				StartTime = DateTime.UtcNow
			};
			var bWindow = new MaintenanceWindow
			{
				StartTime = DateTime.UtcNow.AddHours(1)
			};
			var windows = new List<MaintenanceWindow>
			{
				aWindow, bWindow
			};
			this.maintenanceWindowRepositoryMock.Setup(mock => mock.ReadSortedAsync(query)).ReturnsAsync(windows);

			//Act
			var results = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);

			//Assert
			Assert.That(results.Data, Is.Not.Empty);
			Assert.That(results.Count, Is.EqualTo(windows.Count));
			Assert.That(results.Data.First(), Is.EqualTo(aWindow));
		}

		[Test]
		public async Task GetFilteredMaintenanceWindowsAsync_SortByEndDate_Success()
		{
			//Arrange
			var query = new MaintenanceWindowDataTableQuery
			{
				SortColumn = MaintenanceWindowViewColumns.EndTime.ToString()
			};
			var aWindow = new MaintenanceWindow
			{
				EndTime = DateTime.UtcNow
			};
			var bWindow = new MaintenanceWindow
			{
				EndTime = DateTime.UtcNow.AddHours(1)
			};
			var windows = new List<MaintenanceWindow>
			{
				aWindow, bWindow
			};
			this.maintenanceWindowRepositoryMock.Setup(mock => mock.ReadSortedAsync(query)).ReturnsAsync(windows);

			//Act
			var results = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);

			//Assert
			Assert.That(results.Data, Is.Not.Empty);
			Assert.That(results.Count, Is.EqualTo(windows.Count));
			Assert.That(results.Data.First(), Is.EqualTo(aWindow));
		}

		[Test]
		public async Task GetFilteredMaintenanceWindowsAsync_SortByComments_Success()
		{
			//Arrange
			var query = new MaintenanceWindowDataTableQuery
			{
				SortColumn = MaintenanceWindowViewColumns.Comments.ToString()
			};
			var aWindow = new MaintenanceWindow
			{
				Comments = "ABCD"
			};
			var bWindow = new MaintenanceWindow
			{
				Comments = "AABC"
			};
			var windows = new List<MaintenanceWindow>
			{
				aWindow, bWindow
			};
			this.maintenanceWindowRepositoryMock.Setup(mock => mock.ReadSortedAsync(query)).ReturnsAsync(windows);

			//Act
			var results = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);

			//Assert
			Assert.That(results.Data, Is.Not.Empty);
			Assert.That(results.Count, Is.EqualTo(windows.Count));
			Assert.That(results.Data.First(), Is.EqualTo(bWindow));
		}

		[Test]
		public async Task GetFilteredMaintenanceWindowsAsync_SortByReason_Success()
		{
			//Arrange
			var query = new MaintenanceWindowDataTableQuery
			{
				SortColumn = MaintenanceWindowViewColumns.Reason.ToString()
			};
			var aWindow = new MaintenanceWindow
			{
				Reason = MaintenanceWindowReason.RelativityUpgradeRelease
			};
			var bWindow = new MaintenanceWindow
			{
				Reason = MaintenanceWindowReason.SQLUpgrade
			};
			var windows = new List<MaintenanceWindow>
			{
				aWindow, bWindow
			};
			this.maintenanceWindowRepositoryMock.Setup(mock => mock.ReadSortedAsync(query)).ReturnsAsync(windows);

			//Act
			var results = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);

			//Assert
			Assert.That(results.Data, Is.Not.Empty);
			Assert.That(results.Count, Is.EqualTo(windows.Count));
			Assert.That(results.Data.First(), Is.EqualTo(aWindow));
		}

		[Test]
		public async Task GetFilteredMaintenanceWindowsAsync_SortByReasonDesc_Success()
		{
			//Arrange
			var query = new MaintenanceWindowDataTableQuery
			{
				SortColumn = MaintenanceWindowViewColumns.Reason.ToString(),
				SortDirectionDesc = true
			};
			var aWindow = new MaintenanceWindow
			{
				Reason = MaintenanceWindowReason.RelativityUpgradeRelease
			};
			var bWindow = new MaintenanceWindow
			{
				Reason = MaintenanceWindowReason.SQLUpgrade
			};
			var windows = new List<MaintenanceWindow>
			{
				aWindow, bWindow
			};
			this.maintenanceWindowRepositoryMock.Setup(mock => mock.ReadSortedAsync(query)).ReturnsAsync(windows);

			//Act
			var results = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);

			//Assert
			Assert.That(results.Data, Is.Not.Empty);
			Assert.That(results.Count, Is.EqualTo(windows.Count));
			Assert.That(results.Data.First(), Is.EqualTo(bWindow));
		}



		[Test]
		public async Task ReadMaintenanceWindowAsync_Success()
		{
			// Arrange
			var windowId = 1;
			var window = new MaintenanceWindow();
			this.maintenanceWindowRepositoryMock.Setup(m => m.ReadAsync(windowId)).ReturnsAsync(window);

			// Act
			var response = await this.maintenanceWindowSchedulingService.ReadMaintenanceWindowAsync(windowId);

			// Assert
			Assert.That(response, Is.Not.Null);
		}

		[Test]
		public async Task DeleteMaintenanceWindowAsync_Success()
		{
			// Arrange
			var window = new MaintenanceWindow();
			this.maintenanceWindowRepositoryMock.Setup(m => m.DeleteAsync(window)).Returns(Task.Delay(1));

			// Act
			await this.maintenanceWindowSchedulingService.DeleteMaintenanceWindowAsync(window);

			// Assert
			// No exceptions were thrown and it was deleted so, yay?
		}
	}
}