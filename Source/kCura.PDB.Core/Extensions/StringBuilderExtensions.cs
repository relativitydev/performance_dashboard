namespace kCura.PDB.Core.Extensions
{
	using System.Text;

	public static class StringBuilderExtensions
	{
		public static void AppendLineWithDelimiter(this StringBuilder builder, string line)
		{
			builder.AppendLine(line);
			builder.Append(" | ");
		}
	}
}
