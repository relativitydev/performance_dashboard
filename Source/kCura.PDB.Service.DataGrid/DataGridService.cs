namespace kCura.PDB.Service.DataGrid
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using global::Relativity.Services.Objects.DataContracts;
	using global::Relativity.Services.Proxy.Async;
	using kCura.AuditUI2.Services.Interface.AuditLogManager.Models;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Exception;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.DataGrid.Interfaces;

	public class DataGridService : IDataGridService
	{
		private readonly IAuditLogObjectManagerFactory auditLogObjectManagerFactory;
		private readonly IDataGridConditionBuilder dataGridConditionBuilder;
		private readonly IDataGridResponseAuditMapper dataGridResponseAuditMapper;
		private readonly ILogger logger;

		public DataGridService(IAuditLogObjectManagerFactory auditLogObjectManagerFactory, IDataGridConditionBuilder dataGridConditionBuilder, IDataGridResponseAuditMapper dataGridResponseAuditMapper, ILogger logger)
		{
			this.auditLogObjectManagerFactory = auditLogObjectManagerFactory;
			this.dataGridConditionBuilder = dataGridConditionBuilder;
			this.dataGridResponseAuditMapper = dataGridResponseAuditMapper;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.DataGrid);
		}

		public async Task<IList<Audit>> ReadAuditsAsync(AuditQueryBatch queryBatch, int auditArtifactTypeId, IList<int> actionChoiceIds)
		{
			return await TimeSpan.FromSeconds(1).RetryCall(3, this.logger, async () =>
			{
				var queryStart = GetQueryStart(queryBatch);
				var queryRequest = new QueryRequest
				{
					Condition = this.dataGridConditionBuilder.BuildActionTimeframeCondition(actionChoiceIds.ToArray(),
						queryBatch.Query.StartTime, queryBatch.Query.EndTime),
					Fields = new List<FieldRef>
					{
						new FieldRef{Name = DataGridResourceConstants.DataGridFieldAuditId},
						new FieldRef{Name = DataGridResourceConstants.DataGridFieldUserId},
						new FieldRef{Name = DataGridResourceConstants.DataGridFieldAuditArtifactId},
						new FieldRef{Name = DataGridResourceConstants.DataGridFieldDetails},
						new FieldRef{Name = DataGridResourceConstants.DataGridFieldTimeStamp},
						new FieldRef{Name = DataGridResourceConstants.DataGridFieldAction},
						new FieldRef{Name = DataGridResourceConstants.DataGridFieldExecutionTime}
					},
					ObjectType = new ObjectTypeRef() { ArtifactTypeID = auditArtifactTypeId }
				};

				var auditQueryOptions = new AuditQueryOptions { ReturnRawDetails = true }; // Get Audit details

				using (var auditManagerProxy = this.auditLogObjectManagerFactory.GetManager())
				{
					try
					{
						var result = await
							auditManagerProxy.QueryAsync(
								queryBatch.Query.WorkspaceId,
								queryRequest,
								queryStart,
								queryBatch.Size,
								auditQueryOptions
							);


						return this.dataGridResponseAuditMapper.ResponseToAudits(result, queryBatch.Query.WorkspaceId);
					}
					catch (Exception e)
					{
						if (e.Message.Contains("StatusCode: 404"))
						{
							this.logger.LogWarning(
								$"Endpoint not found when reading audits {e.Message} {this.DataGridServiceDetails(queryBatch.Query)}, retrying...");
							throw new RetryException(e);
						}
						else
							throw this.DataGridServiceException(e, queryBatch.Query,
								$"Failed to read audits Size: {queryBatch.Size}, Start: {queryBatch.Start} || queryStart: {queryStart}, QueryParam: {queryRequest.ToJson()}");
					}
				}
			});
		}

		public async Task<long> ReadTotalAuditsForHourAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById)
		{
			var pivotSettings = new PivotSettings()
			{
				ArtifactTypeID = auditArtifactTypeId,
				GroupBy = new global::Relativity.Services.Field.FieldRef(userGroupById),
				ObjectSetQuery = new global::Relativity.Services.Query()
				{
					Condition =
						this.dataGridConditionBuilder.BuildActionTimeframeCondition(actionChoiceIds.ToArray(), query.StartTime,
							query.EndTime)
				}
			};

			using (var auditManagerProxy = this.auditLogObjectManagerFactory.GetManager())
			{
				try
				{
					var result = await
					auditManagerProxy.ExecuteAsync(
						query.WorkspaceId,
						pivotSettings,
						CancellationToken.None,
						new NonCapturingProgress<string>());
					if (!result.Success)
					{
						throw new Exception(
							$"Read TotalAudits returned unsuccessful with message: {result.Message}");
					}

					try
					{
						if (result.Results.Columns.Contains(DataGridResourceConstants.DataGridGrandTotal))
						{
							return result.Results.AsEnumerable().Sum(x => Convert.ToInt64(x[DataGridResourceConstants.DataGridGrandTotal]));
						}

						// Column doesn't exist, no values returned
						await this.logger.LogVerboseAsync($"No audit totals returned... {result.Results.ToJson()} -- {result.Results.Columns[DataGridResourceConstants.DataGridGrandTotal]} -- WorkspaceId {query.WorkspaceId}, StartTime {query.StartTime}, EndTime {query.EndTime}", Names.LogCategory.DataGrid);
						return 0;
					}
					catch (Exception e)
					{
						throw new Exception($"Failed to parse result.Results {result.Results.ToJson()} -- {result.Results.Columns[DataGridResourceConstants.DataGridGrandTotal]}", e);
					}
				}
				catch (Exception e)
				{
					throw this.DataGridServiceException(e, query, $"Failed to read TotalAudits, PivotSettings: {pivotSettings.ToJson()}");
				}
			}
		}

		public async Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById)
		{
			var pivotSettings = new PivotSettings
			{
				ArtifactTypeID = auditArtifactTypeId,
				GroupBy = new global::Relativity.Services.Field.FieldRef(userGroupById),
				ObjectSetQuery = new global::Relativity.Services.Query()
				{
					Condition =
						this.dataGridConditionBuilder.BuildActionTimeframeCondition(actionChoiceIds.ToArray(), query.StartTime,
							query.EndTime)
				}
			};

			using (var auditManagerProxy = this.auditLogObjectManagerFactory.GetManager())
			{
				try
				{
					var result = await
					auditManagerProxy.ExecuteAsync(
						query.WorkspaceId,
						pivotSettings,
						CancellationToken.None,
						new NonCapturingProgress<string>());
					if (!result.Success)
					{
						throw new Exception($"Read unique users returned unsuccessfully with message: {result.Message}");
					}

					try
					{
						if (result.Results.Columns.Contains(DataGridResourceConstants.DataGridUserName))
						{
							return result.Results.AsEnumerable().Select(x => Convert.ToInt32(x[DataGridResourceConstants.DataGridUserName])).ToList();
						}

						// Column doesn't exist, no values returned
						await this.logger.LogVerboseAsync($"No users returned... {result.Results.ToJson()} -- {result.Results.Columns[DataGridResourceConstants.DataGridUserName]} -- WorkspaceId {query.WorkspaceId}, StartTime {query.StartTime}, EndTime {query.EndTime}", Names.LogCategory.DataGrid);
						return new List<int>();
					}
					catch (Exception e)
					{
						throw new Exception($"Failed to parse result.Results {result.Results.ToJson()} -- {result.Results.Columns[DataGridResourceConstants.DataGridUserName]}", e);
					}
				}
				catch (Exception e)
				{
					throw this.DataGridServiceException(e, query, $"Failed to read unique users, PivotSettings {pivotSettings.ToJson()}");
				}
			}
		}

		public async Task<long> ReadTotalLongRunningQueriesForHourAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById)
		{
			var pivotSettings = new PivotSettings
			{
				ArtifactTypeID = auditArtifactTypeId,
				GroupBy = new global::Relativity.Services.Field.FieldRef(userGroupById),
				ObjectSetQuery = new global::Relativity.Services.Query()
				{
					Condition =
						this.dataGridConditionBuilder.BuildActionTimeframeLongRunningCondition(actionChoiceIds.ToArray(), query.StartTime,
							query.EndTime)
				}
			};

			using (var auditManagerProxy = this.auditLogObjectManagerFactory.GetManager())
			{
				try
				{
					var result = await
					auditManagerProxy.ExecuteAsync(
						query.WorkspaceId,
						pivotSettings,
						CancellationToken.None,
						new NonCapturingProgress<string>());
					if (!result.Success)
					{
						throw new Exception($"Read long running queries returned unsuccessfully with message: {result.Message}");
					}

					try
					{
						if (result.Results.Columns.Contains(DataGridResourceConstants.DataGridGrandTotal))
						{
							return result.Results.AsEnumerable().Sum(x => Convert.ToInt64(x[DataGridResourceConstants.DataGridGrandTotal]));
						}

						// Column doesn't exist, no values returned
						await this.logger.LogVerboseAsync($"No long running queries returned... {result.Results.ToJson()} -- {result.Results.Columns[DataGridResourceConstants.DataGridGrandTotal]} -- WorkspaceId {query.WorkspaceId}, StartTime {query.StartTime}, EndTime {query.EndTime}", Names.LogCategory.DataGrid);
						return 0;
					}
					catch (Exception e)
					{
						throw new Exception($"Failed to parse result.Results {result.Results.ToJson()} -- {result.Results.Columns[DataGridResourceConstants.DataGridGrandTotal]}", e);
					}
				}
				catch (Exception e)
				{
					throw this.DataGridServiceException(e, query, $"Failed to read long running queries, PivotSettings {pivotSettings.ToJson()}");
				}
			}
		}

		public Task<long> ReadTotalAuditExecutionTimeForHourAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById)
		{
			throw new NotImplementedException("TotalAuditExecutionTime not supported on Data Grid APIs");
		}

		private Exception DataGridServiceException(Exception innerException, AuditQuery query, string additionalDetails)
		{
			additionalDetails = string.IsNullOrEmpty(additionalDetails) ? "None" : additionalDetails;
			return new Exception($"DataGridService call failed for {this.DataGridServiceDetails(query)} -- Additional details: {additionalDetails}", innerException);
		}

		private string DataGridServiceDetails(AuditQuery query)
		{
			return $"AuditQuery params: {query.ToJson()}";
		}

		private int GetQueryStart(AuditQueryBatch queryBatch)
		{
			try
			{
				return Convert.ToInt32(queryBatch.Start + 1);
			}
			catch (OverflowException)
			{
				this.logger.LogWarning($"Overflow exception when trying to cast queryStart ({queryBatch.Start + 1}) to int");
				throw;
			}
		}
	}
}
