using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using kCura.PDD.Web.Extensions;

namespace kCura.PDD.Web.Test.Extensions
{
    using NUnit.Framework;

    [TestFixture, Category("Unit")]
	public class StringWriterExtensionsTests
    {
	    [Test]
		[TestCase(new [] { "Hello", "World" }, "\"Hello\",\"World\"\r\n", TestName = "UsesCsvFormatting")]
		[TestCase(new [] { "Hello", "", "World" }, "\"Hello\",\"\",\"World\"\r\n", TestName = "HandlesEmptyStrings")]
		[TestCase(new [] { "Hel,lo", "World" }, "\"Hel,lo\",\"World\"\r\n", TestName = "HandlesCommas")]
		[TestCase(new [] { "This is a \"cool\" unit test", "Isn't it?" }, "\"This is a \"\"cool\"\" unit test\",\"Isn't it?\"\r\n", TestName = "HandlesQuotes")]
		[TestCase(new [] { "This is a \"cool\" unit test", null }, "\"This is a \"\"cool\"\" unit test\",\"\"\r\n", TestName = "HandlesNullValues")]
		[TestCase(new [] { "=SUM(potentialInjectionCode)" }, "\"'=SUM(potentialInjectionCode)\"\r\n", TestName = "HandlesFormulaInjection1")]
		[TestCase(new [] { "+SUM(potentialInjectionCode)" }, "\"'+SUM(potentialInjectionCode)\"\r\n", TestName = "HandlesFormulaInjection2")]
		[TestCase(new [] { "-SUM(potentialInjectionCode)" }, "\"'-SUM(potentialInjectionCode)\"\r\n", TestName = "HandlesFormulaInjection3")]
		[TestCase(new [] { "@SUM(potentialInjectionCode)" }, "\"'@SUM(potentialInjectionCode)\"\r\n", TestName = "HandlesFormulaInjection4")]
		[TestCase(null, "", TestName = "HandlesNullInput")]
	    public void WriteCsvSafeLine_AllCases(string[] inputs, string expectedOutput)
	    {
			var sw = new StringWriter();

			//Act
			sw.WriteCsvSafeLine(inputs);
			var output = sw.ToString();

			//Assert
			Assert.AreEqual(expectedOutput, output);
		}
	}
}
