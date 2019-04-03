namespace kCura.PDB.Core.Interfaces.Services
{
	/// <summary>
	/// Service factory is a key based factory that given a key fetches a service
	/// </summary>
	/// <typeparam name="TService">The service type</typeparam>
	/// <typeparam name="TKey">The key type</typeparam>
	public interface IServiceFactory<out TService, in TKey>
		where TService : class
	{
		/// <summary>
		/// Fetches a service given a key
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The service</returns>
		TService GetService(TKey key);
	}
}
