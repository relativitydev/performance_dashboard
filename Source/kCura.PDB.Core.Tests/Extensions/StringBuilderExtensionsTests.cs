namespace kCura.PDB.Core.Tests.Extensions
{
	using System;
	using System.Text;
	using kCura.PDB.Core.Extensions;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class StringBuilderExtensionsTests
	{
		[Test]
		public void AppendLineWithDelimiter_AddsDelimiter()
		{
			//Arrange
			var builder = new StringBuilder();

			//Act
			builder.AppendLineWithDelimiter("Hello world");

			//Assert
			Assert.AreEqual(String.Format("Hello world{0} | ", Environment.NewLine), builder.ToString());
		}
	}
}
