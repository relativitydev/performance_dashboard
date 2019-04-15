namespace kCura.PDB.Core.Interfaces.Audits
{
	using kCura.PDB.Core.Enumerations;

	public interface IAuditParsingService
	{
		string ParseRawQueryId(string details);

		string ParseQueryId(string parsedDetails);

		string ParseRawQueryText(string details);

		string ParseQueryText(string parsedDetails);

		QueryType? ParseRawQueryType(string details);

		QueryType? ParseQueryType(string parsedDetails);
	}
}