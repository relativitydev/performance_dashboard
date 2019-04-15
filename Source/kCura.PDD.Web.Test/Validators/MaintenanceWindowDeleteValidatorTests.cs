namespace kCura.PDD.Web.Test.Validators
{
    using System;
    using FluentValidation.TestHelper;
    using kCura.PDB.Core.Models;
    using kCura.PDD.Web.Validators;
    using NUnit.Framework;

    [TestFixture, Category("Unit")]
    public class MaintenanceWindowDeleteValidatorTests
    {
        [SetUp]
        public void SetUp()
        {
            this.validator = new MaintenanceWindowDeleteValidator();
        }

        private MaintenanceWindowDeleteValidator validator;

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
            var results = this.validator.Validate(testWindow);

            Assert.That(results.IsValid, Is.True);
        }

        [Test]
        public void Validate_StartTimePast_Failure()
        {
            var testWindow = new MaintenanceWindow
            {
                StartTime = DateTime.UtcNow.AddHours(-1)
            };
            validator.ShouldHaveValidationErrorFor(window => window.StartTime, testWindow);
            var results = this.validator.Validate(testWindow);

            Assert.That(results.IsValid, Is.False);
        }
    }
}
