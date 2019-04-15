namespace kCura.PDB.Core.Models.Testing
{
	using System.Collections.Generic;

	public class TestBackupDbccData
    {
        public IList<MockHour> Hours { get; set; }
        public IList<MockDatabaseChecked> DatabasesChecked { get; set; }
        public IList<MockServer> Servers { get; set; }
        public IList<MockBackupSet> Backups { get; set; }
        public IList<MockDbccServerResults> DbccResults { get; set; }
    }
}
