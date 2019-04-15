using System.IO;
using System.Linq;

namespace kCura.PDD.Web.Extensions
{
	public static class StringWriterExtensions
	{
		public static void WriteCsvSafeLine(this StringWriter sw, string[] inputs)
		{
			if (inputs != null && inputs.Any())
			{
				sw.Write("\"");
				sw.Write(string.Join("\",\"", inputs.Select(x =>
				{
					if (!string.IsNullOrEmpty(x))
					{
						var stringToJoin = x;
						if ("=-+@".IndexOf(x[0]) >= 0)
						{
							stringToJoin = "'" + x; // Prepend with single-quote to prevent formula injection
						}
						return stringToJoin.Replace("\"", "\"\"");
					}
					return x;
				})));
				sw.WriteLine("\"");
			}
		}
	}
}