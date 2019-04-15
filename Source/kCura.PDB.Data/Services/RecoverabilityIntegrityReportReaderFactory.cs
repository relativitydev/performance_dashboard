namespace kCura.PDB.Data.Services
{
	using global::Relativity.Toggles;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Data.Repositories.BISSummary;

	public class RecoverabilityIntegrityReportReaderFactory : IRecoverabilityIntegrityReportReaderFactory
	{
		private readonly IToggleProvider toggleProvider;
		private readonly IConnectionFactory connectionFactory;

		public RecoverabilityIntegrityReportReaderFactory(
			IToggleProvider toggleProvider,
			IConnectionFactory connectionFactory)
		{
			this.toggleProvider = toggleProvider;
			this.connectionFactory = connectionFactory;
		}

		public IRecoverabilityIntegrityReportReader Get()
		{
			return this.toggleProvider.IsEnabled<RecoverabilityIntegrityMetricSystemToggle>()
				       ? (IRecoverabilityIntegrityReportReader)new RecoverabilityIntegrityReportRepository(this.connectionFactory)
				       : (IRecoverabilityIntegrityReportReader)new LegacyRecoverabilityIntegrityReportRepository(this.connectionFactory);
		}
	}
}
