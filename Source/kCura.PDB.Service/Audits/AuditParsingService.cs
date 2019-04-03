namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Linq;
	using System.Xml.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Extensions;

	/// <inheritdoc />
	public class AuditParsingService : IAuditParsingService
	{
		private readonly ILogger logger;

		public AuditParsingService(ILogger logger)
		{
			this.logger = logger.WithCategory(Names.LogCategory.Audit);
		}

		public string ParseRawQueryId(string details)
		{
			var xmlSanitized = details.CleanInvalidXmlChars();

			// Parse the XML
			XDocument xmlDoc2;
			if(!this.TryParse(xmlSanitized, out xmlDoc2))
			{
				return null;
			}

			// Grab the QueryText value
			var queryTextNodes = xmlDoc2.Descendants("QueryText").ToList();
			if (!queryTextNodes.Any())
			{
				return null;
			}

			var parsedDetails = queryTextNodes.First().Value;
			return this.ParseQueryId(parsedDetails);
		}

		public string ParseQueryId(string parsedDetails)
		{
			var xmlSanitized = parsedDetails.CleanInvalidXmlChars();

			var commentsXmlText = ParseXmlComments(xmlSanitized);
			if (string.IsNullOrEmpty(commentsXmlText))
			{
				return null;
			}

			XDocument commentsXmlDoc;
			if (!this.TryParse(commentsXmlText, out commentsXmlDoc))
			{
				return null;
			}
			
			if (commentsXmlDoc.Root == null)
			{
				return null;
			}

			// Grab the QueryId
			var queryIdNodes = commentsXmlDoc.Root.Elements("QueryID").ToList();
			return queryIdNodes.Any() ? queryIdNodes.First().Value : null;
		}

		public QueryType? ParseRawQueryType(string details)
		{
			var xmlSanitized = details.CleanInvalidXmlChars();

			// Parse the XML
			XDocument xmlDoc2;
			if (!this.TryParse(xmlSanitized, out xmlDoc2))
			{
				return null;
			}

			// Grab the QueryText value
			var queryTextNodes = xmlDoc2.Descendants("QueryText").ToList();
			if (!queryTextNodes.Any())
			{
				return null;
			}

			var parsedDetails = queryTextNodes.First().Value;
			return this.ParseQueryType(parsedDetails);
		}

		public QueryType? ParseQueryType(string parsedDetails)
		{
			var xmlSanitized = parsedDetails.CleanInvalidXmlChars();

			var commentsXmlText = ParseXmlComments(xmlSanitized);
			if (string.IsNullOrEmpty(commentsXmlText))
			{
				return null;
			}

			XDocument commentsXmlDoc;
			if (!this.TryParse(commentsXmlText, out commentsXmlDoc))
			{
				return null;
			}

			if (commentsXmlDoc.Root == null)
			{
				return null;
			}

			// Grab the QueryType
			QueryType returnQueryType;
			var queryTypeNodes = commentsXmlDoc.Root.Elements("QueryType").ToList();
			return queryTypeNodes.Any() ?
				Enum.TryParse(queryTypeNodes.First().Value, out returnQueryType) ?
					returnQueryType
					: (QueryType?)null
				: null;
		}

		public string ParseRawQueryText(string details)
		{
			var xmlSanitized = details.CleanInvalidXmlChars();

			// Parse the XML
			XDocument xmlDoc2;
			if (!this.TryParse(xmlSanitized, out xmlDoc2))
			{
				return null;
			}

			// Grab the QueryText
			var queryTextNodes = xmlDoc2.Descendants("QueryText").ToList();
			if (!queryTextNodes.Any())
			{
				return null;
			}

			var parsedDetails = queryTextNodes.First().Value;
			return this.ParseQueryText(parsedDetails);
		}

		public string ParseQueryText(string parsedDetails)
		{
			var xmlSanitized = parsedDetails.CleanInvalidXmlChars();
			
			// Grab just the query text without comments
			var endCommentsXml = xmlSanitized.IndexOf("*/", StringComparison.Ordinal);
		    if (endCommentsXml < 0)
		    {
                return xmlSanitized;
            }

			return xmlSanitized.Substring(endCommentsXml + 2); // Comment characters have a length of 2
		}

		internal static string ParseXmlComments(string parsedDetails)
		{
			// Parse the XML in the comments of the QueryText
			var startCommentsXml = parsedDetails.IndexOf("<", StringComparison.Ordinal);
			var endCommentsXml = parsedDetails.IndexOf("*/", StringComparison.Ordinal);

			// Handle case when Expected XML doesn't exist
			if (startCommentsXml < 0 || endCommentsXml < 0)
			{
				return null;
			}

			return parsedDetails.Substring(startCommentsXml, endCommentsXml - startCommentsXml).CleanInvalidXmlChars();
		}

		internal bool TryParse(string details, out XDocument parsedXmlDocument)
		{
			try
			{
				parsedXmlDocument = XDocument.Parse(details);
				return true;
			}
			catch (Exception ex)
			{
				parsedXmlDocument = null;
				this.logger.LogError($"Failed to parse details: {{{details}}}", ex);
				return false;
			}
		}
	}
}
