namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Collections.Generic;

	public static class ChartColorExtensions
	{
		private static readonly List<string> Colors = new List<string>
		{
			"#3471B7", // Blue
			"#F59D1F", // Orange
			"#010000", // Black
			"#60BD68", // Green
			"#F15854", // Red
			"#B276B2", // Purple
			"#B4674D", // Brown
			"#F17CB0", // Pink
			"#DECF3F", // Yellow
			"#0D98BA", // Blue Green
			"#F59D1F", // Brown 2
			"#DB2126", // Red 2
			"#C1C1C0", // Gray
			"#0AAA4B", // Green
		};

		public static string ChartColorHex(int index)
		{
			if (index < Colors.Count && index >= 0)
			{
				return Colors[index];
			}

			//More than 14 SQL servers? Try a random color...
			var rand = new Random(index);
			return $"rgb({rand.Next(254)},{rand.Next(254)},{rand.Next(254)})";
		}
	}
}
