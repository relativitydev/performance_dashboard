namespace kCura.PDD.Web.Enum
{
    using System.ComponentModel.DataAnnotations;

    public enum MaintenanceWindowViewColumns
    {
        // Enum names should mimic the property names.
        // Display names are the columns exported to the CSV
        [Display(Name = "Start Period")]
        StartTime = 1,
        [Display(Name = "End Period")]
        EndTime = 2,
        [Display(Name = "Reason")]
        Reason = 3,
        [Display(Name = "Comments")]
        Comments = 4,
        [Display(Name = "Duration")]
        DurationHours = 5
    }
}