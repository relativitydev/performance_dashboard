namespace kCura.PDD.Web.Services
{
    using System;
    using System.Globalization;
    using kCura.PDB.Core.Constants;
    using kCura.PDB.Core.Interfaces.Services;
    using kCura.PDB.Core.Models;

    public class MaintenanceWindowModelService : IMaintenanceWindowModelService
    {
        public MaintenanceWindow ConvertFromJS(JsMaintenanceWindowInput input)
        {
            var result = new MaintenanceWindow
            {
                StartTime = string.IsNullOrEmpty(input.StartTime) ? DateTime.MinValue : DateTime.ParseExact(input.StartTime, FormattingConstants.DateTimeFormat, CultureInfo.InvariantCulture),
                EndTime = string.IsNullOrEmpty(input.EndTime) ? DateTime.MinValue : DateTime.ParseExact(input.EndTime, FormattingConstants.DateTimeFormat, CultureInfo.InvariantCulture),
                Comments = input.Comments,
                Reason = input.Reason
            };
            return result;
        }
    }
}