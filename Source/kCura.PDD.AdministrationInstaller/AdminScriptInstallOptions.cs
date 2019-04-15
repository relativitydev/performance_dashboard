namespace kCura.PDD.AdministrationInstaller
{
	using CommandLine;

	[Verb("adminInstall", HelpText = "Run admin install step")]
	public class AdminScriptInstallOptions : CommonOptions
	{
		[Option('u', "username", HelpText = "Database login Username", Default = "sa")]
		public string Username { get; set; }

		[Option('p', "password", HelpText = "Database login Password", Required = true)]
		public string Password { get; set; }
	}
}
