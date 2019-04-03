namespace kCura.PDD.Web.Validators
{
    using System;
    using FluentValidation;
    using kCura.PDB.Core.Models;

    public class MaintenanceWindowDeleteValidator : AbstractValidator<MaintenanceWindow>
    {
        public MaintenanceWindowDeleteValidator()
        {
            RuleFor(window => window.StartTime)
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage(MaintenanceWindowValidationMessages.Delete_StartTime_Invalid);
        }
    }
}