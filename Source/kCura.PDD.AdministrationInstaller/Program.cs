namespace kCura.PDD.AdministrationInstaller
{
	using System;
	using System.Reflection;
	using CommandLine;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.DependencyInjection;
	using kCura.PDB.Service.Bindings;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Tests.Common;
	using kCura.PDD.AdministrationInstaller.Properties;
	using Ninject;

	class Program
	{
		static void Main(string[] args)
		{
			CommandLine.Parser.Default.ParseArguments<AdminScriptInstallOptions, InsertAuditOptions, TestingScriptInstallOptions, QoSDeploymentOptions>(args)
				.WithParsed<AdminScriptInstallOptions>(opt => RunUtilityService<ScriptInstaller, AdminScriptInstallOptions>(opt, RunAdminScriptInstall))
				.WithParsed<InsertAuditOptions>(opt => RunUtilityService<AuditInsertService, InsertAuditOptions>(opt, RunInsertAudits))
				.WithParsed<TestingScriptInstallOptions>(opt => RunUtilityService<ScriptInstaller, TestingScriptInstallOptions>(opt, RunTestingScriptInstall))
				.WithParsed<QoSDeploymentOptions>(opt => RunUtilityService<ScriptInstaller, QoSDeploymentOptions>(opt, RunQoSScriptInstall));
		}

		static void RunInsertAudits(AuditInsertService auditInsertService, InsertAuditOptions options)
		{
			var servicesManager = TestUtilities.GetKeplerServicesManager(options.ServiceUrl, options.RestUrl, options.Username, options.Password);
			var task = auditInsertService.SetupWorkspaceAudits(options.BaseTemplateName, options.PrimaryServerName, options.Username, options.Password, Resources.InjectAuditData, servicesManager);
			task.GetAwaiter().GetResult();
		}

		static void RunAdminScriptInstall(ScriptInstaller scriptInstaller, AdminScriptInstallOptions options) =>
			scriptInstaller.AdminScriptInstall(options.Username, options.Password);

		static void RunTestingScriptInstall(ScriptInstaller scriptInstaller, TestingScriptInstallOptions options)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var testingScripts = assembly.GetResourceBytes("kCura.PDD.AdministrationInstaller.MigrateTesting.zip");

			scriptInstaller.TestingScriptInstall(testingScripts);
		}

		static void RunQoSScriptInstall(ScriptInstaller scriptInstaller, QoSDeploymentOptions options) => 
			scriptInstaller.QoSDeployment();

		static void RunUtilityService<TSrv, TOpt>(TOpt options, Action<TSrv, TOpt> action)
			where TOpt : CommonOptions
		{
			var kernelFactory = new KernelFactory(new UtilityBindings(), new ProductionServiceBindings());
			using (var kernel = kernelFactory.GetKernel())
			using (var block = kernel.BeginBlock())
			{
				action(block.Get<TSrv>(), options);

				if (options.Verbose)
				{
					Console.WriteLine(block.Get<TextLogger>().Text);
				}
			}
		}
	}
}
