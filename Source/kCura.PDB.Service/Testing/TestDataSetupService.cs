namespace kCura.PDB.Service.Testing
{
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Interfaces.Testing.Services;
	using kCura.PDB.Core.Models.Testing;

	public class TestDataSetupService : ITestDataSetupService
	{
		private readonly ITestDataExcelParser testDataExcelParser;
        private readonly IBackupTestDataRepository backupTestDataRepository;
        private readonly IHourTestDataRepository hourTestDataRepository;
        private readonly IDbccTestDataRepository dbccTestDataRepository;
        private readonly IDatabasesCheckedTestDataRepository databasesCheckedTestDataRepository;
        private readonly IServerTestDataRepository serverTestDataRepository;

        public TestDataSetupService(
			ITestDataExcelParser testDataExcelParser,
            IBackupTestDataRepository backupTestDataRepository,
            IHourTestDataRepository hourTestDataRepository,
            IDbccTestDataRepository dbccTestDataRepository,
            IDatabasesCheckedTestDataRepository databasesCheckedTestDataRepository,
            IServerTestDataRepository serverTestDataRepository)
        {
	        this.testDataExcelParser = testDataExcelParser;
            this.backupTestDataRepository = backupTestDataRepository;
            this.hourTestDataRepository = hourTestDataRepository;
            this.dbccTestDataRepository = dbccTestDataRepository;
            this.databasesCheckedTestDataRepository = databasesCheckedTestDataRepository;
            this.serverTestDataRepository = serverTestDataRepository;
        }

		public async Task<TestBackupDbccData> SetupBackupDbccDataAsync(byte[] excelData, bool throwOnNullData = false)
		{
			var data = this.testDataExcelParser.ParseExcelBackupDbccData(excelData, throwOnNullData);
			await this.SetupBackupDbccDataAsync(data);
			return data;
		}

        public async Task SetupBackupDbccDataAsync(TestBackupDbccData data)
        {
            // Clear Data
            await this.CleanupBackupDbccTestDataAsync();

            // Setup Data
            await new[]
                      {
                          this.backupTestDataRepository.CreateAsync(data.Backups),
                          this.hourTestDataRepository.CreateAsync(data.Hours),
                          this.dbccTestDataRepository.CreateAsync(data.DbccResults),
                          this.databasesCheckedTestDataRepository.CreateAsync(data.DatabasesChecked),
                          this.serverTestDataRepository.CreateAsync(data.Servers)
                      }.WhenAllStreamed();
        }

        public void SetupBackupDbccReportData(byte[] backupDbccData)
        {
            throw new System.NotImplementedException();
        }

        public void SetupQoSReportData(byte[] qosReportData)
        {
            throw new System.NotImplementedException();
        }

        public Task CleanupBackupDbccTestDataAsync()
        {
            return Task.WhenAll(
                this.backupTestDataRepository.ClearAsync(),
                this.hourTestDataRepository.ClearAsync(),
                this.dbccTestDataRepository.ClearAsync(),
                this.databasesCheckedTestDataRepository.ClearAsync(),
                this.serverTestDataRepository.ClearAsync());
        }
    }
}
