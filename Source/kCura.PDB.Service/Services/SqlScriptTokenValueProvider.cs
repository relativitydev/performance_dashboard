namespace kCura.PDB.Service.Services
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class SqlScriptTokenValueProvider : ISqlScriptTokenValueProvider
	{
		public SqlScriptTokenValueProvider(IDeploymentRepository deploymentRepo, string serverName)
		{
			_deploymentRepo = deploymentRepo;
			_serverName = serverName;
			InitConstants();
		}

		private readonly IDeploymentRepository _deploymentRepo;
		private readonly string _serverName;
		private readonly IDictionary<string, string> _valueCache = new Dictionary<string, string>();

		public string GetValue(string key)
		{
			switch (key)
			{
				case SqlScriptTokens.MdfDir:
					LoadMdfLdfDirectories();
					break;
				case SqlScriptTokens.LdfDir:
					LoadMdfLdfDirectories();
					break;
				case SqlScriptTokens.Collation:
					LoadCollationSettings();
					break;
			}
			return ReadFromCache(key);
		}

		public void LoadMdfLdfDirectories()
		{
			if (CacheContainsKey(SqlScriptTokens.MdfDir) && CacheContainsKey(SqlScriptTokens.LdfDir))
				return;
			var directoryInfo = _deploymentRepo.ReadMdfLdfDirectories(_serverName);
			AddToCache(SqlScriptTokens.MdfDir, directoryInfo.MdfPath);
			AddToCache(SqlScriptTokens.LdfDir, directoryInfo.LdfPath);
		}

		public void LoadCollationSettings()
		{
			if (CacheContainsKey(SqlScriptTokens.Collation))
				return;
			var collationSettings = _deploymentRepo.ReadCollationSettings();
			AddToCache(SqlScriptTokens.Collation, collationSettings);
		}

		private void InitConstants()
		{
			AddToCache(SqlScriptTokens.ResourceDbName, Names.Database.PdbResource);
			AddToCache(SqlScriptTokens.BackfillDays, Defaults.BackfillDays.ToString());
		}

		private void AddToCache(string key, string value)
		{
			if (CacheContainsKey(key) == false)
				_valueCache.Add(key.ToLower(), value);
		}

		private bool CacheContainsKey(string key)
		{
			return _valueCache.ContainsKey(key.ToLower());
		}

		private string ReadFromCache(string key)
		{
			return _valueCache[key.ToLower()];
		}
	}
}
