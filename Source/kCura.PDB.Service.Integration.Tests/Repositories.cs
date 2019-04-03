namespace kCura.PDB.Service.Integration.Tests
{
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using Ninject;
	using PDB.Tests.Common;

	public static class Repositories
	{
		public static void Init(IKernel kernel)
		{
			kernel.Bind<IConnectionFactory>().ToConstant(TestUtilities.GetIntegrationConnectionFactory()).InSingletonScope();
			HourRepository = kernel.Get<IHourRepository>();
			MetricRepository = kernel.Get<IMetricRepository>();
			MetricDataRepository = kernel.Get<IMetricDataRepository>();
			CategoryRepository = kernel.Get<ICategoryRepository>();
			CategoryScoreRepository = kernel.Get<ICategoryScoreRepository>();
			ServerRepository = kernel.Get<IServerRepository>();
		}

		public static IConnectionFactory ConnectionFactory { get; private set; }

		public static IHourRepository HourRepository { get; private set; }

		public static IMetricRepository MetricRepository { get; private set; }

		public static IMetricDataRepository MetricDataRepository { get; private set; }

		public static ICategoryRepository CategoryRepository { get; private set; }

		public static ICategoryScoreRepository CategoryScoreRepository { get; private set; }

		public static IServerRepository ServerRepository { get; private set; }

	}
}
