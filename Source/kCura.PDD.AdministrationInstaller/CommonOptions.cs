namespace kCura.PDD.AdministrationInstaller
{
	using CommandLine;

	public abstract class CommonOptions
	{
		[Option('c', "connectionString", HelpText = "SQL connection string for Relativity instance", Default = "")]
		public string ConnectionString { get; set; }

		[Option('v', "verbose", HelpText = "Verbose logging", Default = false)]
		public bool Verbose { get; set; }
	}
}
