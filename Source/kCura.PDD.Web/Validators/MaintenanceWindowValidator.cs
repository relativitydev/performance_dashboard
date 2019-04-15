namespace kCura.PDD.Web.Validators
{
    using System;
    using FluentValidation;
    using kCura.PDB.Core.Models;

    public class MaintenanceWindowValidator : AbstractValidator<MaintenanceWindow>
    {
        public MaintenanceWindowValidator()
        {
            RuleFor(window => window.StartTime)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddHours(48))
                .WithMessage(MaintenanceWindowValidationMessages.StartTime_Invalid);

            RuleFor(window => window.EndTime)
                .GreaterThan(window => window.StartTime)
                .WithMessage(MaintenanceWindowValidationMessages.EndTime_Invalid);

            RuleFor(window => window.Reason)
                .IsInEnum()
                .WithMessage(MaintenanceWindowValidationMessages.Reason_Invalid);
        }
    }
}
