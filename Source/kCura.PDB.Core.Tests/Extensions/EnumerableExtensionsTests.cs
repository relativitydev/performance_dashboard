namespace kCura.PDB.Core.Tests.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class EnumerableExtensionsTests
	{
		[Test]
		public void DistinctBy()
		{
			var list = new List<Event>
			{
				new Event
				{
					SourceId = null,
					SourceType = EventSourceType.ScoreCategoryScore,
					Id = 1
				},
				new Event
				{
					SourceId = 1,
					SourceType = EventSourceType.ScoreCategoryScore,
					Id = 2
				},
				new Event
				{
					SourceId = 1,
					SourceType = EventSourceType.ScoreCategoryScore,
					Id = 3
				},
				new Event
				{
					SourceId = 2,
					SourceType = EventSourceType.ScoreCategoryScore,
					Id = 4
				},
				new Event
				{
					SourceId = 3,
					SourceType = EventSourceType.ScoreCategoryScore,
					Id = 5
				},
				new Event
				{
					SourceId = null,
					SourceType = EventSourceType.ScoreCategoryScore,
					Id = 6
				},
			};

			var results = list.DistinctBy(e => new { e.SourceType, e.SourceId }).ToList();
			var excludedResults = list.Except(results);

			Assert.That(list.Count, Is.EqualTo(6));
			Assert.That(results.Count, Is.EqualTo(4));
			Assert.That(excludedResults.Count(), Is.EqualTo(2));
		}

		[Test]
		[TestCase(8, new[] { 1, 2, 3, 4, 5, 6, 7, 8 })]
		[TestCase(2, new[] { 1, 2 })]
		[TestCase(0, new int[0])]
		public async Task WhenAllStreamed(int items, int[] expected)
		{
			var nums = await Enumerable.Range(0, items)
				.Select(async i => await GetNum(i + 1))
				.WhenAllStreamed(4);

			Assert.That(nums.Count(), Is.EqualTo(items));
			Assert.That(nums.All(n => expected.Contains(n)), Is.True);
			Assert.That(expected.All(n => nums.Contains(n)), Is.True);

		}

		[Test]
		public async Task WhereAsync()
		{
			// Arrange
			// Make a list of numbers
			var listOfNums = new[] { 1, 2, 3 };

			// Act
			// Get the even numbers
			var result = await listOfNums.WhereAsync(async a => await GetNum(a) % 2 == 0 == true);

			// Assert
			Assert.That(result.Count(), Is.EqualTo(1));
			Assert.That(result, Contains.Item(2));
		}

		[Test]
		public async Task EnumerableExtensions_SelectManyAsync()
		{
			// Arrange
			// Make a list of numbers
			var listOfNums = Task.FromResult(Enumerable.Range(0, 10).Select(x => new[] { x, x * 2, x * 3 }));

			// Act
			// Get the even numbers
			var result = await listOfNums
					.SelectManyAsync(a => a.Select(GetNum));

			// Assert
			Assert.That(result.Count, Is.EqualTo(30));
		}


		[Test]
		public async Task EnumerableExtensions_ToListAsync()
		{
			// Arrange
			// Make a list of numbers
			var listOfNums = Task.FromResult(Enumerable.Range(0, 10).Select(x => new[] { x, x * 2, x * 3 }));

			// Act
			// Get the even numbers
			var result = await listOfNums
				.SelectManyAsync(a => a.Select(GetNum))
				.ToListAsync();

			// Assert
			Assert.That(result.Count, Is.EqualTo(30));
		}

		[Test]
		public void EnumerableExtensions_AsBatches()
		{
			// Arrange
			// Make a list of numbers
			var listOfNums = Enumerable.Range(0, 35).ToList();

			// Act
			var result = listOfNums.AsBatches(10).ToList();

			// Assert
			Assert.That(result.Count, Is.EqualTo(4));
			Assert.That(result[0], Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
			Assert.That(result[1], Is.EqualTo(new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }));
			Assert.That(result[2], Is.EqualTo(new[] { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 }));
			Assert.That(result[3], Is.EqualTo(new[] { 30, 31, 32, 33, 34 }));
		}

		public async Task<int> GetNum(int i)
		{
			await Task.Delay(TimeSpan.FromMilliseconds(i));
			return i;
		}
	}
}
