namespace kCura.PDB.Service.Services
{
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models;

	public static class SqlServerCredentialInfoBuilder
	{
		public static SqlServerCredentialInfo GetSqlServerCredentialInfo(string connectionString, string sqlServerInstance)
		{
			var dbConnectionSB = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
			var sqlCredInfo = new SqlServerCredentialInfo();
			sqlCredInfo.DatabaseName = Names.Database.EddsQoS;
			sqlCredInfo.Password = dbConnectionSB.Password;
			sqlCredInfo.UserName = dbConnectionSB.UserID;
			sqlCredInfo.UseWindowsAuthentication = dbConnectionSB.IntegratedSecurity;
			sqlCredInfo.SqlServerInstance = sqlServerInstance;
			return sqlCredInfo;
		}
	}
}
