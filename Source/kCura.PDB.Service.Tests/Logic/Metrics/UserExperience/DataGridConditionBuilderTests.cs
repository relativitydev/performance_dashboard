namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System;
	using kCura.PDB.Service.Audits;
	using NUnit.Framework;

	[TestFixture]
	public class DataGridConditionBuilderTests
	{
		private DataGridConditionBuilder dataGridConditionBuilder;

		[SetUp]
		public void SetUp()
		{
			this.dataGridConditionBuilder = new DataGridConditionBuilder();
		}

		[Test]
		[TestCase(
			new[] {1, 3, 4},
			"8/1/2017 7:00:00 PM",
			"8/1/2017 8:00:00 PM",
			"((\'Action\' IN CHOICE [1, 3, 4])) AND ((\'Timestamp\' >= 2017-08-01T19:00:00.00Z)) AND ((\'Timestamp\' <= 2017-08-01T20:00:00.00Z))")]
		public void BuildActionTimeframeCondition(int[] choiceIds, DateTime start, DateTime end, string expectedResult)
		{
			var result = this.dataGridConditionBuilder.BuildActionTimeframeCondition(choiceIds, start, end);

			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(
			new[] { 1, 3, 4 },
			"8/1/2017 7:00:00 PM",
			"8/1/2017 8:00:00 PM",
			"((\'Action\' IN CHOICE [1, 3, 4])) AND ((\'Timestamp\' >= 2017-08-01T19:00:00.00Z)) AND ((\'Timestamp\' <= 2017-08-01T20:00:00.00Z)) AND ((\'Execution Time (ms)\' > 2000))")]
		public void BuildActionTimeframeLongRunningCondition(int[] choiceIds, DateTime start, DateTime end, string expectedResult)
		{
			var result = this.dataGridConditionBuilder.BuildActionTimeframeLongRunningCondition(choiceIds, start, end);

			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
