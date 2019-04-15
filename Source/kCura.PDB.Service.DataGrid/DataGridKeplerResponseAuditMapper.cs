namespace kCura.PDB.Service.DataGrid
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using global::Relativity.Services.Objects.DataContracts;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.Audits.DataGrid;
	using kCura.PDB.Service.DataGrid.Interfaces;

	public class DataGridResponseAuditMapper : IDataGridResponseAuditMapper
	{
		private readonly ILogger logger;

		public DataGridResponseAuditMapper(ILogger logger)
		{
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.DataGrid);
		}

		public IList<Audit> ResponseToAudits(QueryResult response, int workspaceId)
		{
			// DEBUG -- What are the details?
			//await this.logger.LogVerboseAsync($"{response.ToJson()}");

			if (!response.Objects.Any())
			{
				return new Audit[0].ToList();
			}

			return response.Objects.Select(r => this.ParseObjectToAudit(r, workspaceId)).ToList();
		}

		internal Audit ParseObjectToAudit(global::Relativity.Services.Objects.DataContracts.RelativityObject obj, int workspaceId)
		{
			return new Audit
			{
				AuditID = this.GetAuditId(obj),
				UserID = this.GetUserId(obj.FieldValues),
				ArtifactID = obj.ArtifactID,
				ParsedDetails = this.GetDetails(obj.FieldValues),
				TimeStamp = this.GetTimeStamp(obj.FieldValues),
				Action = this.GetAction(obj.FieldValues),
				ExecutionTime = this.GetExecutionTime(obj.FieldValues),
				WorkspaceId = workspaceId
			};
		}

		internal int GetAuditId(RelativityObject obj)
		{
			return int.Parse(obj.Name);
		}

		internal DateTime GetTimeStamp(IEnumerable<FieldValuePair> fieldValuePairs)
		{
			return DateTime.Parse(
					fieldValuePairs.First(fv => fv.Field.Name == DataGridResourceConstants.DataGridFieldTimeStamp).Value.ToString());
		}

		internal AuditActionId GetAction(IEnumerable<FieldValuePair> fieldValuePairs)
		{
			var choiceFieldValue = fieldValuePairs.First(fv => fv.Field.Name == DataGridResourceConstants.DataGridFieldAction).Value;
			var choice = choiceFieldValue as Choice ?? choiceFieldValue.ToString().ToObject<Choice>();
			if (choice == null) throw new Exception("Could not map audit action type to known action type.");

			return AuditConstants.RelevantAuditActionIds.FirstOrDefault(a => a.GetDisplayName() == choice.Name);
		}

		internal long? GetExecutionTime(IEnumerable<FieldValuePair> fieldValuePairs)
		{
			return fieldValuePairs.All(fv => fv.Field.Name != DataGridResourceConstants.DataGridFieldExecutionTime)
				? (long?)null
				: Convert.ToInt64(
					fieldValuePairs.First(fv => fv.Field.Name == DataGridResourceConstants.DataGridFieldExecutionTime).Value.ToString());
		}

		internal int GetUserId(IEnumerable<FieldValuePair> fieldValuePairs)
		{
			return Convert.ToInt32(
				fieldValuePairs.First(fv => fv.Field.Name == DataGridResourceConstants.DataGridFieldUserId).Value);
		}

		internal string GetDetails(IEnumerable<FieldValuePair> fieldValuePairs)
		{
			var details = fieldValuePairs.First(fv => fv.Field.Name == DataGridResourceConstants.DataGridFieldDetails).Value.ToString();
			var detailsObj = details?.ToObject<DataGridAuditDetails>();
			return detailsObj?.AuditElement.QueryText ?? string.Empty;
		}
	}
}
