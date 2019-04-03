namespace kCura.PDB.Service.Services
{
	using System.Net.Http;
	using kCura.PDB.Core.Interfaces.Services;

	public class HttpClientFactory : IHttpClientFactory
	{
		public HttpClient GetDefaultHttpClient()
		{
			return new HttpClient(GetHttpClientHandler(false));
		}

		public HttpClient GetDefaultWindowsAuthHttpClient()
		{
			return new HttpClient(GetHttpClientHandler(true));
		}

		internal static HttpClientHandler GetHttpClientHandler(bool useDefaultCredentials)
		{
			return new HttpClientHandler { UseDefaultCredentials = useDefaultCredentials };
		}
	}
}
