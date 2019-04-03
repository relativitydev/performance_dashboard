namespace kCura.PDB.Data
{
	using System;
	using System.Data.SqlClient;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Services;

	partial class PDDModelDataContext : IPDDModelDataContext
	{
		public PDDModelDataContext()
				: base(GetEDDSPerformanceConnectionString(), mappingSource)
		{
			this.CommandTimeout = AgentConfiguration.CommandTimeout;
			OnCreated();
		}

		public PDDModelDataContext(String connectionString, Boolean b)
				: base(GetEDDSPerformanceConnectionString(connectionString), mappingSource)
		{
			this.CommandTimeout = AgentConfiguration.CommandTimeout;
			OnCreated();//
		}

		private static string GetEDDSPerformanceConnectionString(String connectionString = null)
		{
			// TODO -- Connection string refactor
			SqlConnectionStringBuilder csb;
			if (String.IsNullOrEmpty(connectionString))
			{
				csb = new SqlConnectionStringBuilder(GenericConnectionFactory.GetDefaultConnectionString());
			}
			else
			{
				csb = new SqlConnectionStringBuilder(connectionString);
			}
			csb.ConnectTimeout = AgentConfiguration.ConnectionTimeout;
			csb.InitialCatalog = "EDDSPerformance"; //the kCura connectionstring will point to EDDS, we need EDDSPerformance
			csb.PersistSecurityInfo = true; //the code is grabbing the connection string from the connection in other locations, so this must be true
			return csb.ConnectionString;
		}

		public IQueryable<Measure> ReadMeasures()
		{
			return this.Measures;
		}
	}

	public partial class ServerDiskDW : BaseDW
	{
	}

	/// <summary>
	/// ServerDW Extention 
	/// </summary>
	public partial class ServerDW : BaseDW
	{
		
	}

	/// <summary>
	/// ServerProcessorDW Extention 
	/// </summary>
	public partial class ServerProcessorDW : BaseDW
	{
		
	}

	public partial class SQLServerDW : BaseDW
	{
	}

}