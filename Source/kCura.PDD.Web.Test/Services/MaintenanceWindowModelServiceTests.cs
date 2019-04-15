namespace kCura.PDD.Web.Test.Services
{
    using System;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Core.Constants;
    using kCura.PDD.Web.Services;
    using NUnit.Framework;

    [TestFixture, Category("Unit")]
    public class MaintenanceWindowModelServiceTests
    {
        private MaintenanceWindowModelService maintenanceWindowModelService;

        [Test]
        public void ConvertToJs()
        {
            maintenanceWindowModelService = new MaintenanceWindowModelService();
            var startTime = DateTime.UtcNow.AddDays(3);
            var endTime = DateTime.UtcNow.AddDays(4);

            var jsWindow = new JsMaintenanceWindowInput
            {
                StartTime = startTime.ToString(FormattingConstants.DateTimeFormat),
                EndTime = endTime.ToString(FormattingConstants.DateTimeFormat),
                Comments = "Test comments",
                Reason = MaintenanceWindowReason.HardwareMigration
            };

            var expectedWindow = new MaintenanceWindow
            {
                Comments = jsWindow.Comments,
                IsDeleted = false,
                Reason = jsWindow.Reason,
                StartTime = startTime,
                EndTime = endTime
            };

            var result = maintenanceWindowModelService.ConvertFromJS(jsWindow);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Comments, Is.EqualTo(expectedWindow.Comments));
            Assert.That(result.Reason, Is.EqualTo(expectedWindow.Reason));
            Assert.That(result.StartTime, Is.EqualTo(expectedWindow.StartTime).Within(TimeSpan.FromHours(1)));
            Assert.That(result.EndTime, Is.EqualTo(expectedWindow.EndTime).Within(TimeSpan.FromHours(1)));
        }
    }
}
