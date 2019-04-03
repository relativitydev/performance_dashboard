namespace kCura.PDD.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;
	using System.Web.Http;
	using FluentValidation;
	using FluentValidation.WebApi;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDD.Web.Enum;
	using kCura.PDD.Web.Extensions;
	using kCura.PDD.Web.Factories;
	using kCura.PDD.Web.Filters;
	using kCura.PDD.Web.Models;
	using kCura.PDD.Web.Services;
	using kCura.PDD.Web.Validators;

	public class MaintenanceWindowSchedulerController : BaseApiController
	{
		public MaintenanceWindowSchedulerController(
			IMaintenanceWindowSchedulingService maintenanceWindowSchedulingService,
			IMaintenanceWindowModelService maintenanceWindowModelService,
			IValidator<MaintenanceWindow> maintenanceWindowValidator,
			IValidator<MaintenanceWindow> maintenanceWindowDeleteValidator,
			IRequestService requestService,
			IAuthenticationService authenticationService)
		{
			this.maintenanceWindowSchedulingService = maintenanceWindowSchedulingService;
			this.maintenanceWindowModelService = maintenanceWindowModelService;
			this.maintenanceWindowValidator = maintenanceWindowValidator;
			this.maintenanceWindowDeleteValidator = maintenanceWindowDeleteValidator;
			this.requestService = requestService;
			this.authenticationService = authenticationService;
		}

		private readonly IMaintenanceWindowSchedulingService maintenanceWindowSchedulingService;
		private readonly IMaintenanceWindowModelService maintenanceWindowModelService;
		private readonly IValidator<MaintenanceWindow> maintenanceWindowValidator;
		private readonly IValidator<MaintenanceWindow> maintenanceWindowDeleteValidator;
		private readonly IRequestService requestService;
		private readonly IAuthenticationService authenticationService;

		public MaintenanceWindowSchedulerController()
		{
			this.maintenanceWindowSchedulingService = new MaintenanceWindowSchedulingService(
				new MaintenanceWindowRepository(
					new HelperConnectionFactory(
						ConnectionHelper.Helper())));
			this.maintenanceWindowModelService = new MaintenanceWindowModelService();
			this.maintenanceWindowValidator = new MaintenanceWindowValidator();
			this.maintenanceWindowDeleteValidator = new MaintenanceWindowDeleteValidator();
			this.requestService = new RequestService();
			this.authenticationService = new AuthenticationServiceFactory().GetService();
		}

		[HttpGet]
		public async Task<DataTableResponse> GetListOfSchedules()
		{
			var query = PopulateMaintenanceWindowQuery();

			var schedules = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);

			var aaDataRel = schedules.Data.Select(d => new[] {
				GetDeleteIcon(d),
				d.StartTime.ToString(FormattingConstants.DateTimeFormat),
				d.EndTime.ToString(FormattingConstants.DateTimeFormat),
				d.Reason.GetDisplayName(),
				d.Comments,
				d.DurationHours == 1 ? $"{d.DurationHours.ToString()} hour" : $"{d.DurationHours.ToString()} hours"
			}).ToArray();

			var dtResponse = new DataTableResponse()
			{
				sEcho = string.IsNullOrEmpty(query.sEcho)
					? "1"
					: query.sEcho,
				aaData = aaDataRel,
				recordsTotal = schedules.Data.Count(),
				recordsFiltered = schedules.Count
			};
			return dtResponse;
		}

		private string GetDeleteIcon(MaintenanceWindow d)
		{
			if (!this.authenticationService.IsSystemAdministrator())
				return "";

			if ((DateTime.UtcNow >= d.EndTime) || (DateTime.UtcNow >= d.StartTime))
			{
				return "<span class=\"atm-glyph permissions-delete icon-trash disabled\" " +
					"title=\"Cannot delete window in the past.\" data-placement=\"right\"></span>";
			}

			var windowDeletionClass = d.StartTime.Subtract(DateTime.UtcNow).TotalHours <= 48
				? "expiring-maintenance-window-deletion"
				: "maintenance-window-deletion";

			return $"<a class=\"atm-glyph {windowDeletionClass}\" href=\"#\">" +
						$"<input type=\"hidden\" value=\"{d.Id}\"/>" +
						"<span class=\"atm-glyph permissions-delete icon-trash\"></span>" +
					"</a>";
		}

		[HttpGet]
		public IHttpActionResult GetMaintenanceWindowReasons()
		{
			var reasonDisplayNames = Enum.GetValues(typeof(MaintenanceWindowReason)).Cast<MaintenanceWindowReason>().Select(v => new JSReason { Text = v.GetDisplayName(), Value = (int)v }).OrderBy(v => v.Text);
			return Json(reasonDisplayNames);
		}

		[HttpPost]
		public async Task<HttpResponseMessage> GenerateCSV()
		{
			var query = PopulateMaintenanceWindowQuery();
			query.StartRow = 1;
			query.EndRow = int.MaxValue;

			var fetchedData = await FetchFileData(query);

			//Serialize response
			HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
			message.Content = new StringContent(fetchedData);
			message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
			message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			message.Content.Headers.ContentDisposition.FileName = string.Format("MaintenanceWindows-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
			return message;
		}

		private async Task<string> FetchFileData(MaintenanceWindowDataTableQuery query)
		{
			var grid = await this.maintenanceWindowSchedulingService.GetFilteredMaintenanceWindowsAsync(query);
			var aaData = grid.Data.Select(d => new string[]
			{
				d.StartTime.ToString(FormattingConstants.DateTimeFormat),
				d.EndTime.ToString(FormattingConstants.DateTimeFormat),
				d.Reason.GetDisplayName(),
				WebUtility.HtmlDecode(d.Comments),
				d.DurationHours.ToString()
			}).ToArray();
			using (var sw = new StringWriter())
			{
				var headerArr =
					Enum.GetValues(typeof(MaintenanceWindowViewColumns))
						.Cast<MaintenanceWindowViewColumns>()
						.Select(v => v.GetDisplayName());
				sw.WriteLine(string.Join(",", headerArr));
				foreach (var row in aaData)
					sw.WriteCsvSafeLine(row);
				return sw.ToString();
			}
		}

		private MaintenanceWindowDataTableQuery PopulateMaintenanceWindowQuery()
		{
			var query = new MaintenanceWindowDataTableQuery();

			var queryParams = requestService.GetQueryParams(Request);
			query.StartTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1").Value;
			query.EndTimeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2").Value;
			query.ReasonFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3").Value;
			query.CommentFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4").Value;

			var startTimeOperator = queryParams.FirstOrDefault(k => k.Key == "sOperand_1");
			query.StartTimeOperator = string.IsNullOrEmpty(startTimeOperator.Value) ? FilterOperand.Equals : (FilterOperand)System.Enum.Parse(typeof(FilterOperand), startTimeOperator.Value);
			var endTimeOperator = queryParams.FirstOrDefault(k => k.Key == "sOperand_2");
			query.EndTimeOperator = string.IsNullOrEmpty(endTimeOperator.Value) ? FilterOperand.Equals : (FilterOperand)System.Enum.Parse(typeof(FilterOperand), endTimeOperator.Value);
			var reasonOperator = queryParams.FirstOrDefault(k => k.Key == "sOperand_3");
			query.ReasonOperator = string.IsNullOrEmpty(reasonOperator.Value) ? FilterOperand.Equals : (FilterOperand)System.Enum.Parse(typeof(FilterOperand), reasonOperator.Value);

			var sEcho = queryParams.FirstOrDefault(k => k.Key == "sEcho");
			query.sEcho = sEcho.Value;

			var iDisplayStart = queryParams.FirstOrDefault(k => k.Key == "iDisplayStart");
			query.StartRow = string.IsNullOrEmpty(iDisplayStart.Value) ? 1 : int.Parse(iDisplayStart.Value) + 1;
			var iDisplayLength = queryParams.FirstOrDefault(k => k.Key == "iDisplayLength");
			query.EndRow = (string.IsNullOrEmpty(iDisplayLength.Value) ? 1 : int.Parse(iDisplayLength.Value)) + query.StartRow - 1;

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sSortDir = queryParams.FirstOrDefault(k => k.Key == "sSortDir_0");
			query.SortDirectionDesc = string.IsNullOrEmpty(sSortDir.Value) || sSortDir.Value == "desc";
			MaintenanceWindowViewColumns sortColumn;
			if (Enum.TryParse(iSortCol.Value, out sortColumn) && Enum.IsDefined(typeof(MaintenanceWindowViewColumns), sortColumn))
			{
				query.SortColumn = sortColumn.ToString();
			}

			return query;
		}

		[HttpPost]
		[ValidateAjaxAntiForgeryToken]
		[AuthenticateSystemAdministrator]
		public async Task<MaintenanceWindowValidationResult> CreateMaintenanceWindow(JsMaintenanceWindowInput window)
		{
			var thisWindow = maintenanceWindowModelService.ConvertFromJS(window);
			var oldValidationResult = new MaintenanceWindowValidationResult();
			var validationResult = this.maintenanceWindowValidator.Validate(thisWindow);
			oldValidationResult.Valid = validationResult.IsValid;
			if (validationResult.IsValid)
			{
				// Create MaintenanceWindow
				oldValidationResult.Result = await this.maintenanceWindowSchedulingService.ScheduleMaintenanceWindowAsync(thisWindow);
				return oldValidationResult;
			}
			else
			{
				// Return error details
				validationResult.AddToModelState(this.ModelState, null);
				oldValidationResult.Errors = validationResult.Errors.Select(x => x.ToString()).ToList();
				oldValidationResult.Result = thisWindow;
				return oldValidationResult; 
			}
		}

		[HttpDelete]
		[ValidateAjaxAntiForgeryToken]
		[AuthenticateSystemAdministrator]
		public async Task<IHttpActionResult> DeleteMaintenanceWindow(int id)
		{
			// Read Maintenance window
			var windowToDelete = await maintenanceWindowSchedulingService.ReadMaintenanceWindowAsync(id);
			var deleteValidation = this.maintenanceWindowDeleteValidator.Validate(windowToDelete);
			if (!deleteValidation.IsValid)
			{
				return BadRequest(
					deleteValidation.Errors.Aggregate(new StringBuilder(), (a, b) => a.Append(", " + b.ToString()), (a) => a.Remove(0, 2)).ToString());
			}
			await this.maintenanceWindowSchedulingService.DeleteMaintenanceWindowAsync(windowToDelete);
			return Ok();
		}
	}

	public class MaintenanceWindowValidationResult
	{
		public bool Valid { get; set; }

		public List<string> Errors { get; set; }

		public MaintenanceWindow Result { get; set; }
	}

	public class JSReason
	{
		public string Text { get; set; }

		public int Value { get; set; }
	}
}