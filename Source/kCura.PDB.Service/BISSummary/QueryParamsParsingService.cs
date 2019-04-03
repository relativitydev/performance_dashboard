namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public static class QueryParamsParsingService
	{
		public static int TimeZoneOffset(List<KeyValuePair<string, string>> queryParams)
		{
			var timezoneOffset = queryParams.FirstOrDefault(k => k.Key == "TimezoneOffset");
			return string.IsNullOrEmpty(timezoneOffset.Value)
				? 0
				: int.Parse(timezoneOffset.Value);
		}

		public static DateTime? StartDate(List<KeyValuePair<string, string>> queryParams)
		{
			var startDateParam = queryParams.FirstOrDefault(x => x.Key == "StartDate");
			var timezoneOffset = TimeZoneOffset(queryParams);
			DateTime sd;
			if (!DateTime.TryParse(startDateParam.Value, out sd))
				return DateTime.UtcNow.AddDays(-90).AddMinutes(timezoneOffset);
			else
				return sd.AddMinutes(-1 * timezoneOffset); //midnight local time this day, converted to UTC
		}

		public static DateTime? EndDate(List<KeyValuePair<string, string>> queryParams)
		{
			var endDateParam = queryParams.FirstOrDefault(x => x.Key == "EndDate");
			var timezoneOffset = TimeZoneOffset(queryParams);
			DateTime ed;
			if (!DateTime.TryParse(endDateParam.Value, out ed))
				return DateTime.UtcNow.AddMinutes(timezoneOffset);
			else
				return ed.AddDays(1).AddMinutes(-1 * timezoneOffset - 1); //23:59 local time the following day, converted to UTC
		}

		public static string SelectedServers(List<KeyValuePair<string, string>> queryParams)
		{
			return GetValue(queryParams, "ServerSelection");
		}

		public static int StartRow(List<KeyValuePair<string, string>> queryParams)
		{
			var iDisplayStart = queryParams.FirstOrDefault(k => k.Key == "iDisplayStart");
			int start;
			if (string.IsNullOrEmpty(iDisplayStart.Value) || !int.TryParse(iDisplayStart.Value, out start))
			{
				start = 1;
			}

			return start;
		}

		public static int EndRow(List<KeyValuePair<string, string>> queryParams)
		{
			var startRow = StartRow(queryParams);
			var iDisplayLength = queryParams.FirstOrDefault(k => k.Key == "iDisplayLength");
			int length;
			if (string.IsNullOrEmpty(iDisplayLength.Value) || !int.TryParse(iDisplayLength.Value, out length))
			{
				length = 25;
			}

			return length + startRow - 1;
		}

		public static int ServerArtifactId(List<KeyValuePair<string, string>> queryParams)
		{
			var serverArtifactIdParam = queryParams.FirstOrDefault(x => x.Key == "ServerArtifactId");
			int serverArtifactId;
			if (string.IsNullOrEmpty(serverArtifactIdParam.Value) ||
			    !int.TryParse(serverArtifactIdParam.Value, out serverArtifactId))
			{
				serverArtifactId = -1;
			}

			return serverArtifactId;
		}

		public static string SortIndex(List<KeyValuePair<string, string>> queryParams)
		{
			return GetValue(queryParams, "iSortCol_0");
		}

		public static string SortColumn<T>(List<KeyValuePair<string, string>> queryParams)
			where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			var iSortCol = queryParams.FirstOrDefault(k => k.Key == "iSortCol_0");
			var sortColumn = string.IsNullOrEmpty(iSortCol.Value)
				? null
				: (T?)System.Enum.Parse(typeof(T), iSortCol.Value);
			return sortColumn.HasValue
				? sortColumn.Value.ToString()
				: null;
		}

		public static string SortDirection(List<KeyValuePair<string, string>> queryParams)
		{
			return GetValue(queryParams, "sSortDir_0");
		}

		public static string GetValue(List<KeyValuePair<string, string>> queryParams, string key)
		{
			return queryParams.FirstOrDefault(k => k.Key == key).Value;
		}

		public static GridConditions PopulateCommonGridConditions(List<KeyValuePair<string, string>> queryParams)
		{
			return new GridConditions
			{
				sEcho = QueryParamsParsingService.GetValue(queryParams, "sEcho"),
				StartRow = QueryParamsParsingService.StartRow(queryParams),
				EndRow = QueryParamsParsingService.EndRow(queryParams),
				TimezoneOffset = QueryParamsParsingService.TimeZoneOffset(queryParams),
				StartDate = QueryParamsParsingService.StartDate(queryParams),
				EndDate = QueryParamsParsingService.EndDate(queryParams),
				ServerArtifactId = QueryParamsParsingService.ServerArtifactId(queryParams),
				SelectedServers = QueryParamsParsingService.SelectedServers(queryParams)
			};
		}
	}
}
