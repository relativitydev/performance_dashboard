namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Models;

	public interface IWMIHelper
	{
		List<KeyValuePair<string, string>> CreateDiagnostics(Server server, string wmiQuery, string category);

		/// <summary>
		/// Create diagnostic result
		/// </summary>
		/// <param name="server">Server from which to obtain diagnostic information</param>
		/// <param name="fields">Parameters for fields</param>
		/// <param name="table">Parameter for table name</param>
		/// <param name="where">Where condition</param>
		/// <param name="category">for logging</param>
		/// <returns>Return collection of diagnostic result</returns>
		List<KeyValuePair<string, string>> CreateDiagnostics(Server server, ManagementField fields, string table, string where, string category);

				/// <summary>
		/// Create diagnostic result
		/// </summary>
		/// <param name="server">Server from which to obtain diagnostic information</param>
		/// <param name="fields">Parameters for fields</param>
		/// <param name="table">Parameter for table name</param>
		/// /// <param name="category">for logging</param>
		/// <returns>Return collection of diagnostic result</returns>
		List<KeyValuePair<string, string>> CreateDiagnostics(Server server, ManagementField fields, string table, string category);
	}
}
