namespace kCura.PDB.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum MaintenanceWindowReason
    {
        [Display(Name = "Hardware Upgrade")]
        HardwareUpgrade = 1,

        [Display(Name = "Hardware Migration")]
        HardwareMigration = 2,

        [Display(Name = "Network Maintenance")]
        NetworkMaintenance = 3,

        [Display(Name = "Relativity Upgrade (major release)")]
        RelativityUpgradeRelease = 4,

        [Display(Name = "Relativity Upgrade (patch)")]
        RelativityUpgradePatch = 5,

        [Display(Name = "SQL Upgrade")]
        SQLUpgrade = 6,

        [Display(Name = "Operating System Updates")]
        OSUpdate = 7,

        [Display(Name = "Other (see comments)")]
        Other = 8,
    }
}
