namespace kCura.PDD.Web.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Http;
	using System.Web;

	public class RequestService : IRequestService
	{
		private List<KeyValuePair<string, string>> _queryParams = null;

		public List<KeyValuePair<string, string>> GetQueryParamsDecoded(HttpRequestMessage request)
		{
			return GetQueryParams(request, true);
		}

		public List<KeyValuePair<string, string>> GetQueryParams(HttpRequestMessage request)
		{
			return GetQueryParams(request, false);
		}

		private List<KeyValuePair<string, string>> GetQueryParams(HttpRequestMessage request, bool decode)
		{
			// Return cached value if we have one.
			if (null != _queryParams) return _queryParams;

			_queryParams = request.GetQueryNameValuePairs().ToList();

			if (request.Method != HttpMethod.Post) return _queryParams;

			var nvc = request.Content.ReadAsFormDataAsync().ConfigureAwait(true).GetAwaiter().GetResult();

			foreach (string key in nvc)
			{
				if (_queryParams.Any(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))) continue;
				var val = nvc[key];
				if (decode && !string.IsNullOrEmpty(val))
					val = HttpUtility.UrlDecode(val);
				_queryParams.Add(new KeyValuePair<string, string>(key, val));
			}

			return _queryParams;
		}

		public static int GetTimezoneOffset(HttpRequestMessage request)
		{
			try
			{
				var ckTimeZoneOffset = request.Headers.GetCookies("TimeZoneOffset").FirstOrDefault(); //.Cookies["TimeZoneOffset"];
				return ckTimeZoneOffset != null && ckTimeZoneOffset["TimeZoneOffset"].Value != string.Empty
					? Convert.ToInt32(ckTimeZoneOffset["TimeZoneOffset"].Value)
					: 0;
			}
			catch
			{
				return 0;
			}
		}
		public static int GetTimezoneOffset(HttpRequest request)
		{
			var ckTimeZoneOffset = request.Cookies["TimeZoneOffset"];
			return ckTimeZoneOffset != null && ckTimeZoneOffset.Value != string.Empty
				? Convert.ToInt32(ckTimeZoneOffset.Value)
				: 0;
		}
	}
}