namespace kCura.PDB.Core.Models
{
    public class JsMaintenanceWindowInput
    {
        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public MaintenanceWindowReason Reason { get; set; }

        public string Comments { get; set; }
    }
}
