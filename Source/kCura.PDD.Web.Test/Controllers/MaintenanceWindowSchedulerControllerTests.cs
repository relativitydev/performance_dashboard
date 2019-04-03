namespace kCura.PDD.Web.Test.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using System.Web.Http.Results;
	using FluentValidation;
	using FluentValidation.Results;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDD.Web.Controllers;
	using kCura.PDD.Web.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class MaintenanceWindowSchedulerControllerTests
	{
		private Mock<IMaintenanceWindowSchedulingService> maintenanceWindowSchedulingServiceMock;
		private Mock<IMaintenanceWindowModelService> maintenanceWindowModelServiceMock;
		private Mock<IValidator<MaintenanceWindow>> maintenanceWindowValidatorMock;
		private Mock<IValidator<MaintenanceWindow>> maintenanceWindowDeleteValidatorMock;
		private Mock<IRequestService> requestServiceMock;
		private IAuthenticationService authenticationService;
		private MaintenanceWindowSchedulerController maintenanceWindowSchedulerController;

		[SetUp]
		public void SetUp()
		{
			maintenanceWindowSchedulingServiceMock = new Mock<IMaintenanceWindowSchedulingService>(MockBehavior.Strict);
			maintenanceWindowModelServiceMock = new Mock<IMaintenanceWindowModelService>(MockBehavior.Strict);
			maintenanceWindowValidatorMock = new Mock<IValidator<MaintenanceWindow>>();
			maintenanceWindowDeleteValidatorMock = new Mock<IValidator<MaintenanceWindow>>();
			requestServiceMock = new Mock<IRequestService>(MockBehavior.Strict);
			authenticationService = Mock.Of<IAuthenticationService>(x => x.IsSystemAdministrator() == true);

			maintenanceWindowSchedulerController =
				new MaintenanceWindowSchedulerController(maintenanceWindowSchedulingServiceMock.Object,
					maintenanceWindowModelServiceMock.Object,
					maintenanceWindowValidatorMock.Object,
					maintenanceWindowDeleteValidatorMock.Object,
					requestServiceMock.Object,
					authenticationService);
		}

		[Test]
		public async Task CreateMaintenanceWindow_Valid()
		{
			var startTime = DateTime.UtcNow.AddDays(3);
			var endTime = DateTime.UtcNow.AddDays(4);

			var jsWindow = new JsMaintenanceWindowInput
			{
				StartTime = startTime.ToString(FormattingConstants.DateTimeFormat),
				EndTime = endTime.ToString(FormattingConstants.DateTimeFormat),
			};

			var window = new MaintenanceWindow
			{
				StartTime = startTime,
				EndTime = endTime
			};

			var validation = new ValidationResult();

			var createdWindow = new MaintenanceWindow
			{
				Id = 1,
				IsDeleted = false,
				StartTime = DateTime.ParseExact(jsWindow.StartTime, FormattingConstants.DateTimeFormat, CultureInfo.InvariantCulture),
				EndTime = DateTime.ParseExact(jsWindow.EndTime, FormattingConstants.DateTimeFormat, CultureInfo.InvariantCulture),
			};

			var expectedResult = new MaintenanceWindowValidationResult
			{
				Valid = true,
				Result = createdWindow
			};

			maintenanceWindowModelServiceMock.Setup(mock => mock.ConvertFromJS(jsWindow)).Returns(window);

			maintenanceWindowValidatorMock.Setup(mock => mock.Validate(window))
				.Returns(validation);

			maintenanceWindowSchedulingServiceMock.Setup(mock => mock.ScheduleMaintenanceWindowAsync(window))
				.ReturnsAsync(createdWindow);

			var result = await maintenanceWindowSchedulerController.CreateMaintenanceWindow(jsWindow);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Valid, Is.EqualTo(expectedResult.Valid));
			Assert.That(result.Result, Is.EqualTo(expectedResult.Result));
			Assert.That(result.Errors, Is.EqualTo(expectedResult.Errors));
		}

		[Test]
		public async Task CreateMaintenanceWindow_Invalid()
		{
			var startTime = DateTime.UtcNow.AddDays(4);
			var endTime = DateTime.UtcNow.AddDays(3);

			var jsWindow = new JsMaintenanceWindowInput
			{
				StartTime = startTime.ToString(FormattingConstants.DateTimeFormat),
				EndTime = endTime.ToString(FormattingConstants.DateTimeFormat),
			};

			var window = new MaintenanceWindow
			{
				StartTime = startTime,
				EndTime = endTime
			};

			var validation = new ValidationResult
			{
				Errors = { new ValidationFailure("StartTime", "Start Time must be after End Time.") }
			};

			var expectedValidation = new MaintenanceWindowValidationResult
			{
				Valid = false,
				Result = window,
				Errors = new List<string>
				{
				  "Start Time must be after End Time."
				}
			};

			maintenanceWindowModelServiceMock.Setup(mock => mock.ConvertFromJS(jsWindow)).Returns(window);
			maintenanceWindowValidatorMock.Setup(mock => mock.Validate(window))
				.Returns(validation);

			var result = await maintenanceWindowSchedulerController.CreateMaintenanceWindow(jsWindow);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Valid, Is.EqualTo(expectedValidation.Valid));
			Assert.That(result.Valid, Is.False);
			Assert.That(result.Result, Is.EqualTo(expectedValidation.Result));
		}

		[Test]
		public async Task GetFilteredMaintenanceWindows_Success()
		{
			//Arrange
			var queryParams = new List<KeyValuePair<string, string>>();
			var windows = new List<MaintenanceWindow>
			{
				new MaintenanceWindow
				{
					StartTime = DateTime.UtcNow,
					EndTime = DateTime.UtcNow.AddHours(1),
					Reason = MaintenanceWindowReason.Other,
					Comments = "Unit Test"
				}
			};
			var grid = new GeneralCheckGrid<MaintenanceWindow>
			{
				Data = windows.AsQueryable()
			};

			this.requestServiceMock.Setup(mock => mock.GetQueryParams(this.maintenanceWindowSchedulerController.Request))
				.Returns(queryParams);
			this.maintenanceWindowSchedulingServiceMock.Setup(mock => mock.GetFilteredMaintenanceWindowsAsync(It.IsAny<MaintenanceWindowDataTableQuery>()))
				.ReturnsAsync(grid);

			//Act
			var result = await this.maintenanceWindowSchedulerController.GetListOfSchedules();

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task GenerateCSV_Success()
		{
			//Arrange
			var queryParams = new List<KeyValuePair<string, string>>();
			this.requestServiceMock.Setup(mock => mock.GetQueryParams(this.maintenanceWindowSchedulerController.Request))
				.Returns(queryParams);

			var windows = new List<MaintenanceWindow>
			{

			};
			var grid = new GeneralCheckGrid<MaintenanceWindow>
			{
				Data = windows.AsQueryable()
			};

			this.maintenanceWindowSchedulingServiceMock.Setup(mock => mock.GetFilteredMaintenanceWindowsAsync(It.IsAny<MaintenanceWindowDataTableQuery>()))
				.ReturnsAsync(grid);

			//Act
			var result = await this.maintenanceWindowSchedulerController.GenerateCSV();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}

		[Test]
		public async Task DeleteMaintenanceWindow_Success()
		{
			// Arrange
			var windowId = 1;
			var window = new MaintenanceWindow();
			var validationResult = new ValidationResult();

			this.maintenanceWindowSchedulingServiceMock.Setup(m => m.ReadMaintenanceWindowAsync(windowId))
				.ReturnsAsync(window);
			this.maintenanceWindowDeleteValidatorMock.Setup(m => m.Validate(window)).Returns(validationResult);
			this.maintenanceWindowSchedulingServiceMock.Setup(m => m.DeleteMaintenanceWindowAsync(window))
				.Returns(Task.Delay(1));

			// Act
			var response = await this.maintenanceWindowSchedulerController.DeleteMaintenanceWindow(windowId);

			// Assert
			Assert.That(response, Is.TypeOf(typeof(OkResult)));
		}

		[Test]
		public async Task DeleteMaintenanceWindow_Failure()
		{
			// Arrange
			var windowId = 1;
			var window = new MaintenanceWindow();
			var validationResult = new ValidationResult(new List<ValidationFailure>
				{
					new ValidationFailure("StartTime", "UnitTestError")
				});

			this.maintenanceWindowSchedulingServiceMock.Setup(m => m.ReadMaintenanceWindowAsync(windowId))
				.ReturnsAsync(window);
			this.maintenanceWindowDeleteValidatorMock.Setup(m => m.Validate(window)).Returns(validationResult);

			// Act
			var response = await this.maintenanceWindowSchedulerController.DeleteMaintenanceWindow(windowId);

			// Assert
			Assert.That(response, Is.TypeOf(typeof(BadRequestErrorMessageResult)));
		}

		[Test]
		public void GetMaintenanceWindowReasons()
		{
			var response = this.maintenanceWindowSchedulerController.GetMaintenanceWindowReasons();
			var jsonResponse = response as JsonResult<IOrderedEnumerable<JSReason>>;
			Assert.That(jsonResponse, Is.Not.Null);
			Assert.That(jsonResponse.Content.Count(), Is.EqualTo(Enum.GetValues(typeof(MaintenanceWindowReason)).Length));
		}
	}
}
