namespace kCura.PDB.Service.Extensions
{
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web;
	using System.Xml;

	public static class StringExtensions
	{
		public static string CleanInvalidXmlChars(this string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			// Check for invalid chars in string
			if (HasInvalidXmlChars(text))
			{
				// Remove with regex and check again
				text = text.RegexRemoveInvalidXmlChars();
				if (HasInvalidXmlChars(text))
				{
					// If still invalid, try slower method
					return text.RemoveInvalidXmlChars();
				}
			}
			else
			{
				// Decode the details as HTML and double check
				var htmlDecoded = HttpUtility.HtmlDecode(text);
				if (!HasInvalidXmlChars(htmlDecoded))
				{
					return text;
				}

				// Remove with regex and check again
				htmlDecoded = htmlDecoded.RegexRemoveInvalidXmlChars();
				if (HasInvalidXmlChars(htmlDecoded))
				{
					// If still invalid, try slower method
					return htmlDecoded.RemoveInvalidXmlChars();
				}

				return htmlDecoded;
			}

			return text;
		}

		internal static bool HtmlContainsInvalidXmlChars(string text)
		{
			var decoded = HttpUtility.HtmlDecode(text);
			return HasInvalidXmlChars(decoded);
		}

		/// <summary>
		/// Encapsulate XmlConvert.VerifyXmlChars into a try/catch and return bool result
		/// </summary>
		/// <param name="text">Text that may have invalid xml characters</param>
		/// <returns>True if invalid characters exist in the string</returns>
		internal static bool HasInvalidXmlChars(string text)
		{
			try
			{
				XmlConvert.VerifyXmlChars(text);
				return false;
			}
			catch (XmlException ex)
			{
				return true;
			}
		}

		/// <summary>
		/// Removes xml characters from a string by iterating over each character and omitting invalid characters from the new stringbuilder
		/// </summary>
		/// <param name="inString">String that may have invalid xml characters</param>
		/// <returns>String with no invalid xml characters</returns>
		internal static string RemoveInvalidXmlChars(this string inString)
		{
			if (string.IsNullOrEmpty(inString))
				return inString;

			int length = inString.Length;
			StringBuilder newString = new StringBuilder();

			for (int i = 0; i < length; ++i)
			{
				if (XmlConvert.IsXmlChar(inString[i]))
				{
					newString.Append(inString[i]);
				}
				else if (i + 1 < length && XmlConvert.IsXmlSurrogatePair(inString[i + 1], inString[i]))
				{
					newString.Append(inString[i]);
					newString.Append(inString[i + 1]);
					++i;
				}
			}

			return newString.ToString();
		}

		/// <summary>
		/// Removes invalid XML characters via Regex.Replace().  See: https://stackoverflow.com/questions/29301248/hexadecimal-value-0x0b-is-an-invalid-character-issue-in-xml
		/// </summary>
		/// <param name="inString">String that may have invalid xml characters</param>
		/// <returns>String with no invalid xml characters</returns>
		internal static string RegexRemoveInvalidXmlChars(this string inString)
		{
			string regex = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
			var result1 = Regex.Replace(inString, regex, string.Empty);
			var htmlDecoded = HttpUtility.HtmlDecode(inString);
			var result2 = Regex.Replace(htmlDecoded, regex, string.Empty);
			return Regex.Replace(inString, regex, string.Empty);
		}
	}
}
