namespace kCura.PDB.Service.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public abstract class GenericLogger
	{
		public void LogCritical(string message, List<string> categories)
		{
			this.Log(0, message, categories.ToArray());
		}

		public void LogCritical(string message, string category = null)
		{
			this.Log(0, message, category);
		}

		public void LogError(string message, List<string> categories)
		{
			this.Log(1, message, categories.ToArray());
		}

		public void LogError(string message, string category = null)
		{
			this.Log(1, message, category);
		}

		public void LogError(string message, Exception ex, string category = null)
		{
			this.LogError($"{message}. Details:\r\n{ex.ToString()}", category);
		}

		public void LogError(string message, Exception ex, List<string> categories)
		{
			this.LogError($"{message}. Details:\r\n{ex.ToString()}", categories);
		}

		public void LogWarning(string message, List<string> categories)
		{
			this.Log(5, message, categories.ToArray());
		}

		public void LogWarning(string message, string category = null)
		{
			this.Log(5, message, category);
		}

		public void LogInformation(string message, List<string> categories)
		{
			this.Log(9, message, categories.ToArray());
		}

		public void LogInformation(string message, string category = null)
		{
			this.Log(9, message, category);
		}

		public void LogVerbose(string message, List<string> categories)
		{
			this.Log(10, message, categories.ToArray());
		}

		public void LogVerbose(string message, string category = null)
		{
			this.Log(10, message, category);
		}

		public Task LogVerboseAsync(string message, List<string> categories)
		{
			return this.LogAsync(10, message, categories.ToArray());
		}

		public Task LogVerboseAsync(string message, string category = null)
		{
			return this.LogAsync(10, message, category);
		}

		public Task LogWarningAsync(string message, List<string> categories)
		{
			return this.LogAsync(5, message, categories.ToArray());
		}

		public Task LogWarningAsync(string message, string category = null)
		{
			return this.LogAsync(5, message, category);
		}

		public Task LogWarningAsync(string message, Exception ex, List<string> categories)
		{
			return this.LogAsync(5, $"{message}. Details:\r\n{ex.ToString()}", categories?.ToArray());
		}

		public Task LogWarningAsync(string message, Exception ex, string category = null)
		{
			return this.LogAsync(5, $"{message}. Details:\r\n{ex.ToString()}", category);
		}

		protected abstract void Log(int level, string message, params string[] categories);

		//protected abstract Task LogAsync(int level, string message, params string[] categories);

		public virtual Task LogAsync(int level, string message, params string[] categories) =>
			Task.Run(() => this.Log(level, message, categories));
	}
}
