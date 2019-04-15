namespace kCura.PDB.Tests.Common.Extensions
{
	using System;
	using global::Relativity.API;
	using global::Relativity.Services.ServiceProxy;

	public static class ServiceManagerExtension
	{
		public static T GetProxy<T>(this IServicesMgr svcmgr, string username, string password) where T : IDisposable
		{
			return new global::Relativity.Services.ServiceProxy.ServiceFactory(new ServiceFactorySettings(svcmgr.GetServicesURL(), svcmgr.GetKeplerUrl(), (Credentials)new global::Relativity.Services.ServiceProxy.UsernamePasswordCredentials(username, password))).CreateProxy<T>();
		}

		public static Uri GetKeplerUrl(this IServicesMgr svcmgr)
		{
			return new Uri($"{svcmgr.GetRESTServiceUrl()}/api");
		}
	}
}
