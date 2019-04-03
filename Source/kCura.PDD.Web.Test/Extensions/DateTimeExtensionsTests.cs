using System;
using kCura.PDD.Web.Extensions;

namespace kCura.PDD.Web.Test.Extensions
{
    using NUnit.Framework;

    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void GetFirstDayOfWeek()
        {
            //Arrange
            var date = DateTime.Parse("3/31/2014");

            //Act
            var firstDay = date.GetFirstDayOfWeek(DayOfWeek.Friday);

            //Assert
            Assert.AreEqual("3/28/2014", firstDay.ToShortDateString());
        }

        [Test]
        public void GetLastDayOfWeek()
        {
            //Arrange
            var date = DateTime.Parse("3/31/2014");

            //Act
            var lastDay = date.GetLastDayOfWeek(DayOfWeek.Friday);

            //Assert
            Assert.AreEqual("4/4/2014", lastDay.ToShortDateString());
        }
    }
}
