namespace kCura.PDB.Core.Helpers
{
	using System.Data.SqlClient;

	public class SqlConnectionStringBuilderWrapper
	{
		private readonly SqlConnectionStringBuilder baseBuilder;

		public SqlConnectionStringBuilderWrapper()
		{
			this.baseBuilder = new SqlConnectionStringBuilder();
		}

		public SqlConnectionStringBuilderWrapper(string connectionString)
		{
			this.baseBuilder = new SqlConnectionStringBuilder(connectionString);
			this.CacheDomain();
		}

		public SqlConnectionStringBuilderWrapper(SqlConnectionStringBuilder sqlConnectionStringBuilder)
		{
			this.baseBuilder = sqlConnectionStringBuilder;
			this.CacheDomain();
		}

		public string DataSource
		{
			get
			{
				return this.baseBuilder.DataSource;
			}

			set
			{
				if (!string.IsNullOrEmpty(this.Domain))
				{
					this.baseBuilder.DataSource = value + "." + this.Domain;
				}
				else
				{
					this.baseBuilder.DataSource = value;
				}
			}
		}

		public string ConnectionString => this.baseBuilder.ConnectionString;

		public string InitialCatalog
		{
			get { return this.baseBuilder.InitialCatalog; }
			set { this.baseBuilder.InitialCatalog = value; }
		}

		public string Domain { get; private set; }

		private void CacheDomain()
		{
			var splitDataSource = this.baseBuilder.DataSource.Split(new[] { '.' }, 2);
			if (splitDataSource.Length > 1)
			{
				this.Domain = splitDataSource[1];
			}
		}
	}
}
