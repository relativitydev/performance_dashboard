namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Net.Http;

	public interface IHttpClientFactory
	{
		HttpClient GetDefaultHttpClient();

		HttpClient GetDefaultWindowsAuthHttpClient();
	}
}
