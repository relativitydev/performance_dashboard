namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;

	public class SqlScriptTokenService : ISqlScriptTokenService
	{
		public SqlScriptTokenService(ISqlScriptTokenValueProvider tokenValueProvider)
		{
			_tokenValueProvider = tokenValueProvider;
		}

		private readonly ISqlScriptTokenValueProvider _tokenValueProvider;

		public void Replace(IEnumerable<string> files)
		{
			if (files == null || files.Any() == false) throw new ArgumentNullException("files");

			var fileTokens = files.Select(f => new { File = f, Tokens = GetTokensFromFile(f) }).ToList();

			var tokenValues = fileTokens
				.SelectMany(ft => ft.Tokens)
				.Distinct()
				.Select(ft => new { Token = ft, Value = _tokenValueProvider.GetValue(ft) })
				.ToDictionary(ft => ft.Token, ft => ft.Value);

			fileTokens.ForEach(ft => ReplaceFileTokenValues(ft.File, ft.Tokens, tokenValues));
		}

		public string ReplaceFileTokenValues(string file, IEnumerable<string> fileTokens, IDictionary<string, string> tokenValues)
		{
			var fileText = File.ReadAllText(file);
			if (fileTokens.Any() == false) return fileText;
			var newFileText = fileTokens.Aggregate(fileText, (s, s1) => s.Replace(String.Format("{{{{{0}}}}}", s1), tokenValues[s1]));
			File.WriteAllText(file, newFileText);
			return newFileText;
		}

		public IEnumerable<string> GetTokensFromFile(string file)
		{
			if (File.Exists(file) == false) throw new FileNotFoundException(String.Format("Cannot read tokens from: {0}", file));

			var fileText = File.ReadAllText(file);
			return Regex.Matches(fileText, @"\{\{(\w+)\}\}", RegexOptions.IgnoreCase).GetCaptures().Distinct();
		}
	}
}
