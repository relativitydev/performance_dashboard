namespace kCura.PDB.Tests.Common
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;
	using Data.Services;
	using global::Relativity.API;
	using global::Relativity.Services.Pipeline;
	using global::Relativity.Services.ServiceProxy;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.Relativity.Client;
	using Moq;
	using Service.Services;

	public static class TestUtilities
	{
		public static IConnectionFactory GetIntegrationConnectionFactory()
		{
			return new ConfiguredConnectionFactory(new AppSettingsConfigurationService());
		}

		public static GenericCredentialInfo GetSACredentialInfo()
		{
			return new GenericCredentialInfo
			{
				UserName = Config.SAUserName,
				Password = Config.SAPassword,
				UseWindowsAuthentication = false
			};
		}

		// This doesn't seem to work from Integration tests.  Needs to be running from within Relativity?
		public static GenericCredentialInfo GetIntegratedCredentialInfo()
		{
			return new GenericCredentialInfo { UseWindowsAuthentication = true };
		}

		public static DeploymentSettings GetDeploymentSettings()
		{
			return new DeploymentSettings
			{
				Server = Config.Server,
				DatabaseName = Names.Database.EddsPerformance,
				CredentialInfo = GetSACredentialInfo()
			};
		}

		public static Mock<ILogger> GetMockLogger()
		{
			var logger = new Mock<ILogger>();
			logger.Setup(l => l.LogCritical(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((m, c) => Console.WriteLine($"{c}: {m}"));
			logger.Setup(l => l.LogCritical(It.IsAny<string>(), It.IsAny<List<string>>())).Callback<string, List<string>>((m, c) => Console.WriteLine($"{string.Join(",", c)}: {m}"));
			logger.Setup(l => l.LogError(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((m, c) => Console.WriteLine($"{c}: {m}"));
			logger.Setup(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>())).Callback<string, Exception, string>((m, e, c) => Console.WriteLine($"{c}: {m}. {e.ToString()}"));
			logger.Setup(l => l.LogError(It.IsAny<string>(), It.IsAny<List<string>>())).Callback<string, List<string>>((m, c) => Console.WriteLine($"{string.Join(",", c)}: {m}"));
			logger.Setup(l => l.LogInformation(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((m, c) => Console.WriteLine($"{c}: {m}"));
			logger.Setup(l => l.LogInformation(It.IsAny<string>(), It.IsAny<List<string>>())).Callback<string, List<string>>((m, c) => Console.WriteLine($"{string.Join(",", c)}: {m}"));
			logger.Setup(l => l.LogVerbose(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((m, c) => Console.WriteLine($"{c}: {m}"));
			logger.Setup(l => l.LogVerbose(It.IsAny<string>(), It.IsAny<List<string>>())).Callback<string, List<string>>((m, c) => Console.WriteLine($"{string.Join(",", c)}: {m}"));
			logger.Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((m, c) => Console.WriteLine($"{c}: {m}"));
			logger.Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<List<string>>())).Callback<string, List<string>>((m, c) => Console.WriteLine($"{string.Join(",", c)}: {m}"));
			logger.Setup(l => l.LogVerboseAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Delay(1)).Callback<string, string>((m, c) => Console.WriteLine($"{c}: {m}"));
			logger.Setup(l => l.LogVerboseAsync(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(Task.Delay(1)).Callback<string, List<string>>((m, c) => Console.WriteLine($"{string.Join(",", c)}: {m}"));
			logger.Setup(l => l.LogWarningAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Delay(1)).Callback<string, string>((m, c) => Console.WriteLine($"{c}: {m}"));
			logger.Setup(l => l.LogWarningAsync(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(Task.Delay(1)).Callback<string, List<string>>((m, c) => Console.WriteLine($"{string.Join(",", c)}: {m}"));
			logger.Setup(l => l.LogWarningAsync(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>())).Returns(Task.Delay(1)).Callback<string, Exception, string>((m, ex, c) => Console.WriteLine($"{c}: {m}. {ex.ToString()}"));
			logger.Setup(l => l.LogWarningAsync(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<List<string>>())).Returns(Task.Delay(1)).Callback<string, Exception, List<string>>((m, ex, c) => Console.WriteLine($"{string.Join(",", c)}: {m}. {ex.ToString()}"));
			return logger;
		}



		public static ITimeService GetTimeService(double multiplier, DateTime? startTime = null)
		{
			return new MockTimeService(multiplier, startTime);
		}

		public static IServicesMgr GetServicesManager(string serviceUrl = null, string restUrl = null, string userName = null, string password = null)
		{
			var settings = GetServiceFactorySettings(serviceUrl, restUrl, userName, password);
			return new TestServicesManager(settings);
		}

		public static IServicesMgr GetKeplerServicesManager(string serviceUrl = null, string restUrl = null, string userName = null, string password = null)
		{
			var settings = GetServiceFactorySettings(serviceUrl, restUrl, userName, password);
			return new TestKeplerServicesManager(settings);
		}

		static ServiceFactorySettings GetServiceFactorySettings(string serviceUrl = null, string restUrl = null, string userName = null,
			string password = null)
		{
			return new ServiceFactorySettings(
				new Uri(serviceUrl ?? Config.RelativityServiceUrl),
				new Uri(restUrl ?? Config.RelativityRestUrl), //needed for e.g. Pivot API
				new global::Relativity.Services.ServiceProxy.UsernamePasswordCredentials(userName ?? Config.RSAPIUsername, password ?? Config.RSAPIPassword)
			);
		}
		public static Mock<IHelper> GetMockHelper(string serviceUrl = null, string restUrl = null, string userName = null,
			string password = null)
		{
			var helper = new Mock<IHelper>();
			helper.Setup(m => m.GetServicesManager()).Returns(GetServicesManager(serviceUrl, restUrl, userName, password));
			return helper;
		}
	}

	public class TestServicesManager : IServicesMgr
	{
		private readonly ServiceFactorySettings settings;

		public TestServicesManager(ServiceFactorySettings settings)
		{
			this.settings = settings;
		}

		public Uri GetServicesURL()
		{
			return this.settings.RelativityServicesUri;
		}

		public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
		{
			return new ServiceFactory(settings).CreateProxy<T>();
		}


		public Uri GetRESTServiceUrl()
		{
			return this.settings.RelativityRestUri;
		}
	}

	public class TestKeplerServicesManager : IServicesMgr
	{
		private readonly KeplerServiceFactorySettings keplerSettings;
		private readonly ServiceFactorySettings settings;

		public TestKeplerServicesManager(KeplerServiceFactorySettings keplerSettings)
		{
			this.keplerSettings = keplerSettings;
		}

		public TestKeplerServicesManager(ServiceFactorySettings settings)
		{
			this.settings = settings;
			this.keplerSettings = new KeplerServiceFactorySettings(new Uri(settings.RelativityRestUri, "/API"),
				settings.Credentials.GetAuthenticationHeaderValue(), WireProtocolVersion.V2);
		}

		public Uri GetServicesURL()
		{
			return this.settings.RelativityServicesUri;
		}

		public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
		{
			if (typeof(T) == typeof(IRSAPIClient))
			{
				return new ServiceFactory(settings).CreateProxy<T>();
			}

			return new KeplerServiceFactory(keplerSettings).GetClient<T>();
		}

		public Uri GetRESTServiceUrl()
		{
			return this.settings.RelativityRestUri;
		}
	}

	public sealed class ApiClientHelper
	{
		public Lazy<T> GetKeplerServiceReference<T>() where T : IDisposable
		{
			return new Lazy<T>(GetKeplerServiceReferenceX<T>);
		}

		internal T GetKeplerServiceReferenceX<T>() where T : IDisposable
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			string basicAuthHeader = GetAuthenticationHeader();
			var sfs = new KeplerServiceFactorySettings(new Uri(Config.RelativityRestUrl + "/API"), basicAuthHeader, WireProtocolVersion.V2);
			var ksf = new KeplerServiceFactory(sfs);

			return ksf.GetClient<T>();
		}

		public string GetAuthenticationHeader()
		{
			return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Config.RSAPIUsername}:{Config.RSAPIPassword}"))}";
		}
	}
}
