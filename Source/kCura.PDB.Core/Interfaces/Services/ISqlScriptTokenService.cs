namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Collections.Generic;

	public interface ISqlScriptTokenService
	{
		void Replace(IEnumerable<string> files);
		IEnumerable<string> GetTokensFromFile(string file);
	}
}
