namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using kCura.PDB.Service.Services;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class TimeoutTests
	{
		[Test,
			TestCase(10, true),
			TestCase(500, false)]
		public async Task IsAfterTimedOut(int delay, bool expectedResult)
		{
			// Arrange & Act
			var timeout = new Timeout(TimeSpan.FromMilliseconds(delay));
			
			// wait 100 milliseconds
			await Task.Delay(TimeSpan.FromMilliseconds(100));

			// is it AFTER the timeout given the delay?
			var result = timeout.IsAfterTimedOut;

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test,
			TestCase(10, false),
			TestCase(500, true)]
		public async Task TimeRemaining(int delay, bool hasTimeRemaining)
		{
			// Arrange & Act
			var timeout = new Timeout(TimeSpan.FromMilliseconds(delay));

			// wait 100 milliseconds
			await Task.Delay(TimeSpan.FromMilliseconds(100));

			// get the time remaining
			var result = timeout.TimeRemaining.TotalMilliseconds;

			// Assert
			Assert.That(result > 0, Is.EqualTo(hasTimeRemaining));
		}
	}
}
