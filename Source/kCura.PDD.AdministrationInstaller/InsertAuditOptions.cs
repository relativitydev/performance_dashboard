namespace kCura.PDD.AdministrationInstaller
{
	using CommandLine;

	[Verb("insertAudits", HelpText = "Create a workspace and insert audits")]
	public class InsertAuditOptions : CommonOptions
	{
		[Option('u', "username", HelpText = "Relativity login Username", Default = "relativity.admin@kcura.com")]
		public string Username { get; set; }

		[Option('p', "password", HelpText = "Relativity login Password", Required = true)]
		public string Password { get; set; }

		[Option('s', "serviceUrl", Required = true, HelpText = "Relativity instance service url")]
		public string ServiceUrl { get; set; }

		[Option('r', "restUrl", Required = true, HelpText = "Relativity instance rest url")]
		public string RestUrl { get; set; }

		[Option("primaryServerName", Required = true, HelpText = "Sql Server instance name where EDDS lives")]
		public string PrimaryServerName { get; set; }

		[Option('b', "baseTemplate", Required = true, HelpText = "Used with 'InjectAudits' to create a test workspace")]
		public string BaseTemplateName { get; set; }
	}
}
