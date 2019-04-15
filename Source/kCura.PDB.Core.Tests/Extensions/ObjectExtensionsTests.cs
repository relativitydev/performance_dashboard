namespace kCura.PDB.Core.Tests.Extensions
{
	using System;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ObjectExtensionsTests
	{
		[Test]
		[TestCase(AuditActionId.DocumentQuery, "Document Query")]
		[TestCase(AuditActionId.UpdateMassEdit, "Update - Mass Edit")]
		public void Test(Enum val, string expectedOutput)
		{
			Assert.That(val.GetDisplayName(), Is.EqualTo(expectedOutput));
		}
	}
}
