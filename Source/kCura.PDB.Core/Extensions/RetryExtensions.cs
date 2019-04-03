namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Exception;
	using kCura.PDB.Core.Interfaces.Services;

	public static class RetryExtensions
	{
		public static async Task<T> RetryCall<T>(this TimeSpan delay, int times, ILogger logger, Func<Task<T>> operation)
		{
			var attempts = 0;
			do
			{
				try
				{
					attempts++;
					return await operation();
				}
				catch (RetryException ex)
				{
					if (attempts == times)
					{
						throw ex.InnerException;
					}

					logger.WithClassName().LogVerbose($"Exception caught on attempt {attempts} - will retry after delay {delay} -- Exception Details: {ex}");

					await Task.Delay(delay);
				}
			}
			while (true);
		}
	}
}
