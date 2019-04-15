namespace kCura.PDD.Web.Test.Validators
{
    using System;
    using FluentValidation.TestHelper;
    using kCura.PDB.Core.Models;
    using kCura.PDD.Web.Validators;
    using NUnit.Framework;

    [TestFixture, Category("Unit")]
    public class MaintenanceWindowValidatorTests
    {
        [SetUp]
        public void SetUp()
        {
            this.validator = new MaintenanceWindowValidator();
        }

        private MaintenanceWindowValidator validator;

        [Test]
        public void Validate_Success()
        {
            var testWindow = new MaintenanceWindow
            {
                StartTime = DateTime.UtcNow.AddDays(3),
                EndTime = DateTime.UtcNow.AddDays(4),
                Reason = MaintenanceWindowReason.SQLUpgrade
            };
            validator.ShouldNotHaveValidationErrorFor(window => window.StartTime, testWindow);
            validator.ShouldNotHaveValidationErrorFor(window => window.EndTime, testWindow);
            var results = this.validator.Validate(testWindow);

            Assert.That(results.IsValid, Is.True);
        }

        [Test]
        public void Validate_StartDate_Past_Failure()
        {
            var testWindow = new MaintenanceWindow
            {
                StartTime = DateTime.UtcNow.AddDays(-2),
                EndTime = DateTime.UtcNow.AddDays(4),
                Reason = MaintenanceWindowReason.SQLUpgrade
            };
            validator.ShouldHaveValidationErrorFor(window => window.StartTime, testWindow);
            var results = this.validator.Validate(testWindow);

            Assert.That(results.IsValid, Is.False);
        }

        [Test]
        public void Validate_StartDate_Invalid_Failure()
        {
            var testWindow = new MaintenanceWindow
            {
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(4),
                Reason = MaintenanceWindowReason.SQLUpgrade
            };
            validator.ShouldHaveValidationErrorFor(window => window.StartTime, testWindow);
            var results = this.validator.Validate(testWindow);

            Assert.That(results.IsValid, Is.False);
        }

        [Test]
        public void Validate_EndDate_Invalid_Failure()
        {
            var testWindow = new MaintenanceWindow
            {
                StartTime = DateTime.UtcNow.AddDays(3),
                EndTime = DateTime.UtcNow.AddDays(2),
                Reason = MaintenanceWindowReason.SQLUpgrade
            };
            validator.ShouldHaveValidationErrorFor(window => window.EndTime, testWindow);
            var results = this.validator.Validate(testWindow);

            Assert.That(results.IsValid, Is.False);
        }

        [Test]
        public void Validate_Reason_Invalid_Failure()
        {
            var testWindow = new MaintenanceWindow
            {
                StartTime = DateTime.UtcNow.AddDays(3),
                EndTime = DateTime.UtcNow.AddDays(2),
            };
            validator.ShouldHaveValidationErrorFor(window => window.Reason, testWindow);
            var results = this.validator.Validate(testWindow);

            Assert.That(results.IsValid, Is.False);
        }
    }
}
