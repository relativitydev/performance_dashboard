namespace kCura.PDB.Data.Extensions
{
	using System;
	using System.Text;

	using global::Relativity.Services.Agent;

	public static class ResultSetExtensions
	{
		public static void ThrowIfUnsuccessful(this AgentQueryResultSet results, string initialMessage = null)
		{
			if (!results.Success)
			{
				var sb = new StringBuilder();
				if (!string.IsNullOrEmpty(initialMessage))
				{
					sb.AppendLine(initialMessage);
				}
				sb.Append(results.Message);
				foreach (var r in results.Results)
				{
					if (!r.Success)
					{
						sb.AppendLine(r.Message);
					}
				}
				throw new Exception(sb.ToString());
			}
		}
	}
}
