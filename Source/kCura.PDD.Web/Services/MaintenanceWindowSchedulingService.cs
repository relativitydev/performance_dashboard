namespace kCura.PDD.Web.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using DevExpress.Data.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDD.Web.Controllers;
	using kCura.PDD.Web.Enum;
	using kCura.PDD.Web.Extensions;

	public class MaintenanceWindowSchedulingService : IMaintenanceWindowSchedulingService
	{
		public MaintenanceWindowSchedulingService(IMaintenanceWindowRepository maintenanceWindowRepository)
		{
			this.maintenanceWindowRepository = maintenanceWindowRepository;
		}

		private readonly IMaintenanceWindowRepository maintenanceWindowRepository;

		public async Task<MaintenanceWindow> ScheduleMaintenanceWindowAsync(MaintenanceWindow window)
		{
			// Validation has happened at this point
			window.StartTime = window.StartTime.NormalizeHour();
			window.EndTime = window.EndTime.NormalizeHour();

			// Sanitize comments string before inputting into database?
			window.Comments = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(window.Comments, false);

			// Create window
			return await maintenanceWindowRepository.CreateAsync(window);
		}

		public async Task<GeneralCheckGrid<MaintenanceWindow>> GetFilteredMaintenanceWindowsAsync(MaintenanceWindowDataTableQuery query)
		{
			var grid = new GeneralCheckGrid<MaintenanceWindow>();
			var results = await this.maintenanceWindowRepository.ReadSortedAsync(query);
			var resultList = results as IList<MaintenanceWindow> ?? results.ToList();

			// Sort
			resultList = SortResults(resultList, query);

			// paging (start row/end row)
			// Add sort/ start row/end row to MWDataTableQuery
			var gridCount = resultList.Count;
			grid.Data = resultList.AsQueryable();

			if (0 < query.StartRow)
				grid.Data = grid.Data.Skip(query.StartRow - 1);
			if (0 < query.EndRow)
				grid.Data = grid.Data.Take(query.EndRow - query.StartRow + 1);

			grid.Count = gridCount;

			return grid;
		}

		private IList<MaintenanceWindow> SortResults(IList<MaintenanceWindow> resultList,
			MaintenanceWindowDataTableQuery query)
		{
			var sortColumn = string.IsNullOrEmpty(query.SortColumn) == false
				? query.SortColumn
				: MaintenanceWindowViewColumns.StartTime.ToString();

			return query.SortDirectionDesc
				? resultList.OrderByDescending(mw => SortBy(mw, sortColumn)).ToList()
				: resultList.OrderBy(mw => SortBy(mw, sortColumn)).ToList();
		}

		private object SortBy(MaintenanceWindow mw, string sortColumn)
		{
			var propertyInfo = typeof(MaintenanceWindow).GetProperty(sortColumn);
			if (propertyInfo.PropertyType.IsEnum)
			{
				return propertyInfo.GetValue(mw, null).ToString();
			}
			if (propertyInfo.Name == MaintenanceWindowViewColumns.Comments.ToString())
			{
				var v = propertyInfo.GetValue(mw, null);
				return DecodeHTMLComments(v);
			}
			return propertyInfo.GetValue(mw, null);
		}

		private string DecodeHTMLComments(object comments)
		{
			var commentString = comments as string;
			return string.IsNullOrEmpty(commentString) ? null : WebUtility.HtmlDecode(commentString);
		}

		public async Task<MaintenanceWindow> ReadMaintenanceWindowAsync(int id)
		{
			return await this.maintenanceWindowRepository.ReadAsync(id);
		}

		public async Task DeleteMaintenanceWindowAsync(MaintenanceWindow window)
		{
			await this.maintenanceWindowRepository.DeleteAsync(window);
		}

	}
}