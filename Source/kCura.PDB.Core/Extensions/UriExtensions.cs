namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Linq;

	public static class UriExtensions
	{
		public static Uri Append(this Uri uri, params string[] paths)
		{
			return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
		}
	}
}
