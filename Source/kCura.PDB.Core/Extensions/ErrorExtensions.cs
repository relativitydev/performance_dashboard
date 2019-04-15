namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Linq;

	public static class ErrorExtensions
	{
		public static string GetExceptionDetails(this Exception exception)
		{
			var fields = exception
				.GetType()
				.GetProperties()
				.Select(property => new
				{
					property.Name,
					Value = property.GetValue(exception, null)
				})
				.Select(x => $"{x.Name} = {x.Value?.ToString() ?? string.Empty}");
			return string.Join("\n", fields);
		}
	}
}
