using kCura.PDB.Core.Models;
using kCura.PDB.Core.Services;
using kCura.PDB.Data.Repositories;
using kCura.PDB.Data.Services;
using kCura.PDB.Service.Services;
using kCura.PDD.Web.Filters;
using kCura.PDD.Web.Mapper;
using kCura.PDD.Web.Models.BISSummary;
using System.Threading.Tasks;
using System.Web.Http;

namespace kCura.PDD.Web.Controllers
{
	using System;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Notifications;
	using kCura.PDD.Web.Services;

	[AuthenticateUser]
	public class ServiceQualityController : ApiController
	{
		private BestInServiceReportingService ReportingService;
		private IConnectionFactory connectionFactory;
		private ISqlServerRepository SqlRepo;
		private PdbNotificationService NotificationService;

		public ServiceQualityController()
		{
			this.connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			this.SqlRepo = new SqlServerRepository(connectionFactory);
			var toggleProvider = new PdbSqlToggleProvider(connectionFactory);
			var reportRepositoryFactory = new RecoverabilityIntegrityReportReaderFactory(toggleProvider, this.connectionFactory);
			var reportRepository = reportRepositoryFactory.Get();

			this.ReportingService = new BackupDbccService(SqlRepo, reportRepository);
		}

		[HttpGet]
		public QualityOfServiceViewModel GetServiceQualityIndicators()
		{
			var model = BestInServiceMapper.ToQualityOfServiceReportModel(ReportingService, RequestService.GetTimezoneOffset(this.Request));
			var service = new BestInServiceReportingService(SqlRepo);
			model.SampleRange = new LookingGlassInformation(service.GetSampleRange(RequestService.GetTimezoneOffset(this.Request))).SampleRange;
			return model;
		}

		[HttpGet]
		public async Task<bool> GetFraudDetection()
		{
			//spinning up the SQL Repo takes awhile, making this separate should allow the user to see overall indicator levels much quicker
			return await Task.FromResult(false);
		}

		[HttpGet]
		public QualityIndicatorLevel GetIndicatorLevels()
		{
			var qualityService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(this.connectionFactory)));
			return qualityService.GetIndicatorLevels();
		}

		[HttpGet]
		public PDBNotification GetNotifications()
		{
			NotificationService = new PdbNotificationService(this.SqlRepo);
			return NotificationService.GetNext();
		}
	}
}
