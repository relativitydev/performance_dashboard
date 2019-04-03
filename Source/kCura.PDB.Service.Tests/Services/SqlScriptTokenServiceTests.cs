namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class SqlScriptTokenServiceTests
	{
		[SetUp]
		public void Setup()
		{
			_tokenValueProvider = new Mock<ISqlScriptTokenValueProvider>();
			Console.WriteLine(Guid.NewGuid().ToString());
			var tokensText = @"abc {{blah}} something something {{blah}} {{stuff1}} and {{things}}";
			_tokensFilePath = Path.GetTempFileName();
			File.WriteAllText(_tokensFilePath, tokensText);

			var tokensText2 = @"{{blah}} {{blah}} {{asdf}}";
			_tokensFilePath2 = Path.GetTempFileName();
			File.WriteAllText(_tokensFilePath2, tokensText2);

			var noTokensText = @"abc something something 1234"; ;
			_noTokensFilePath = Path.GetTempFileName();
			File.WriteAllText(_noTokensFilePath, noTokensText);
		}

		private string _tokensFilePath;
		private string _tokensFilePath2;
		private string _noTokensFilePath;

		private Mock<ISqlScriptTokenValueProvider> _tokenValueProvider;

		[Test]
		public void Replace_NoFiles()
		{
			//Arrange

			//Act
			var srv = new SqlScriptTokenService(_tokenValueProvider.Object);
			Assert.Throws<ArgumentNullException>(() =>
				srv.Replace(new string[] { }));

		}

		[Test]
		public void Replace_Success()
		{
			//Arrange
			_tokenValueProvider.Setup(tvp => tvp.GetValue("blah")).Returns("123");
			_tokenValueProvider.Setup(tvp => tvp.GetValue("stuff1")).Returns("234");
			_tokenValueProvider.Setup(tvp => tvp.GetValue("things")).Returns("345");
			_tokenValueProvider.Setup(tvp => tvp.GetValue("asdf")).Returns("456");

			//Act
			var srv = new SqlScriptTokenService(_tokenValueProvider.Object);
			srv.Replace(new[] { _tokensFilePath, _tokensFilePath2, });

			//Assert
			Assert.Pass();
		}


		[Test]
		public void GetTokensFromFile_FileDoesntExist()
		{
			//Arrange

			//Act
			var srv = new SqlScriptTokenService(_tokenValueProvider.Object);
			Assert.Throws<FileNotFoundException>(() =>
				srv.GetTokensFromFile("FILE DOESNT EXIST"));
		}

		[Test]
		public void GetTokensFromFile_Success()
		{
			//Arrange

			//Act
			var srv = new SqlScriptTokenService(_tokenValueProvider.Object);
			var results = srv.GetTokensFromFile(_tokensFilePath);

			//Assert
			Assert.That(results.OrderBy(r => r), Is.EqualTo(new[] { "blah", "stuff1", "things" }));
		}

		[Test]
		public void GetTokensFromFile_NoTokens()
		{
			//Arrange
			var text = @"abc something something 1234";
			var tempFile = Path.GetTempFileName();
			File.WriteAllText(tempFile, text);

			//Act
			var srv = new SqlScriptTokenService(_tokenValueProvider.Object);
			var results = srv.GetTokensFromFile(tempFile);

			//Assert
			Assert.That(results, Is.Empty);
		}


		[Test]
		public void ReplaceFileTokenValues_Success()
		{
			//Arrange
			var tokens = new[] { "blah", "stuff1", "things" };
			var val1 = Guid.NewGuid().ToString();
			var val2 = Guid.NewGuid().ToString();
			var val3 = Guid.NewGuid().ToString();
			var tokenValues = new Dictionary<string, string>()
			{
				{ "blah", val1 },
				{ "stuff1", val2 },
				{ "things", val3 }
			};

			//Act
			var srv = new SqlScriptTokenService(_tokenValueProvider.Object);
			var result = srv.ReplaceFileTokenValues(_tokensFilePath, tokens, tokenValues);

			//Assert
			Assert.That(result.Split(new[] { val1 }, StringSplitOptions.None).Count(), Is.EqualTo(3)); // confirm there are 2 occurrences
			Assert.That(result.Contains(val2), Is.True);
			Assert.That(result.Contains(val3), Is.True);
			Assert.That(result.Contains("{"), Is.False);
			Assert.That(result.Contains("}"), Is.False);
		}

		[Test]
		public void ReplaceFileTokenValues_NoToken()
		{
			//Arrange
			var tokens = new string[] { };
			var tokenValues = new Dictionary<string, string>();
			var originalText = File.ReadAllText(_noTokensFilePath);

			//Act
			var srv = new SqlScriptTokenService(_tokenValueProvider.Object);
			var result = srv.ReplaceFileTokenValues(_noTokensFilePath, tokens, tokenValues);

			//Assert
			Assert.That(result, Is.EqualTo(originalText));
		}
	}
}

