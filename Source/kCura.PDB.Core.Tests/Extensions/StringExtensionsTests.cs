namespace kCura.PDB.Core.Tests.Extensions
{
	using System;
	using System.Xml.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Service.Extensions;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class StringExtensionsTests
	{
		[Test]
		[TestCase("This is a bunch of words", 6)]
		[TestCase("WORDS ARE STUff [And] things? !?", 5)]
		[TestCase("Word", 1)]
		[TestCase("A", 1)]
		[TestCase("", 0)]
		public void WordCount(string s, int expectedCount)
		{
			// Arrange
			// Act
			var wordCount = s.WordCount();

			// Assert
			Assert.That(wordCount, Is.EqualTo(expectedCount));
		}

		public const string SampleInput = "Burninating the countryside";

		[Test]
		public void Truncate_HandlesNullInputs()
		{
			//Arrange
			string input = null;

			//Act
			var result = input.Truncate(10);

			//Assert
			Assert.IsNull(result);
		}

		[Test]
		public void Truncate_HandlesEmptyInputs()
		{
			//Arrange
			var input = String.Empty;

			//Act
			var result = input.Truncate(10);

			//Assert
			Assert.AreEqual(String.Empty, result);
		}

		[Test]
		public void Truncate_HandlesInvalidLength()
		{
			//Arrange
			var input = SampleInput;

			//Act
			var result = input.Truncate(0);

			//Assert
			Assert.AreEqual(SampleInput, result);
		}

		[Test]
		public void Truncate_DoesNotModifyOnExactLength()
		{
			//Arrange
			var input = SampleInput;

			//Act
			var result = input.Truncate(SampleInput.Length);

			//Assert
			Assert.AreEqual(SampleInput, result);
		}

		[Test]
		public void Truncate_ShortensLongInput()
		{
			//Arrange
			var input = SampleInput;

			//Act
			var result = input.Truncate(4);

			//Assert
			Assert.AreEqual("Bur", result);
		}

		[Test]
		[TestCase("<key>my&#xB;tag</key>", "<key>mytag</key>")]
		[TestCase("<a>This has valid characters</a>", "<a>This has valid characters</a>")]
		[TestCase("<a>a\u0013d</a>", "<a>ad</a>")]
		[TestCase("<a>a\x0B d</a>", "<a>a d</a>")]
		[TestCase("<a>a\\x0B d</a>", "<a>a\\x0B d</a>")]
		[TestCase("<a>a\vd</a>", "<a>ad</a>")]
		public void InvalidXmlChars_Test(string testCase, string expectedResult)
		{
			// Arrange
			// Act
			var result = testCase.CleanInvalidXmlChars();

			// Assert
			var xmlDoc = XDocument.Parse(result); // Xml Parse doesn't throw exception
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
