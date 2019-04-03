namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web;
	using System.Xml;

	public static class StringExtensions
	{
		public static string Truncate(this string original, int limit = 15)
		{
			if (string.IsNullOrEmpty(original) || limit < 1)
				return original;

			return original.Length > limit
				? $"{original.Substring(0, limit - 1)}"
				: original;
		}

		public static T? TryParse<T>(this string s)
			where T : struct
		{
			var converter = TypeDescriptor.GetConverter(typeof(T));
			try
			{
				var value = (T)converter.ConvertFromString(s);
				return value;
			}
			catch
			{
				return null;
			}
		}

		public static int WordCount(this string s)
		{
			return Regex.Matches(s, @"[A-Za-z0-9]+").Count;
		}

		public static bool ComparisonContains(
			this string text,
			string value,
			StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
		{
			return text.IndexOf(value, stringComparison) >= 0;
		}

		public static T ToObject<T>(this string jsonObj)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonObj);
		}
	}
}