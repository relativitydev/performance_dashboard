namespace kCura.PDB.Data
{
	using Dapper;
	using kCura.PDB.Core.Constants;

	public static class DataSetup
	{
		public static void Setup()
		{
			SqlMapper.Settings.CommandTimeout = Defaults.Database.ConnectionTimeout;
		}
	}
}
