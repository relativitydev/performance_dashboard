namespace kCura.PDB.Core.Extensions
{
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public static class RegexExtensions
	{
		public static IEnumerable<string> GetCaptures(this MatchCollection matches)
		{
			foreach (Match match in matches)
			{
				for (var g = 1; g < match.Groups.Count; g++)
				{
					yield return match.Groups[g].Captures[0].Value;
				}
			}
		}
	}
}
