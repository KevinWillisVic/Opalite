using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace FishAndChips
{
    public class EntityService : Singleton<EntityService>, IInitializable, ICleanable
    {
		#region -- Supporting --
		private struct CreationData
		{
			public string EntityId;
			public string SaveId;
		}
		#endregion

		#region -- Protected Member Vars --
		protected Dictionary<string, IEntity> _cachedEntities = new();

		// Services
		protected DataService _dataService;
		#endregion

		#region -- Private Methods --
		private void CreateEntity<TData>(string instanceId, TData data, CreationData creationData, bool addToCache = true) where TData : class, IMetaData
		{
			var entity = data.CreateEntity(instanceId);
			if (entity == null)
			{
				return;
			}
			entity.Data = data;
			CreateSaveData(entity, creationData.SaveId);

			entity.Initialize();

			if (addToCache == true)
			{
				_cachedEntities.Add(creationData.EntityId, entity);
			}
		}
		#endregion

		#region -- Protected Methods --
		protected virtual void CreateEntity<TData>(string instanceId, string savedDataPrefix) where TData : class, IMetaData
		{
			var data = _dataService.Get<TData>(instanceId);
			if (data == null)
			{
				Debug.LogError($"instance with id {instanceId} doesn't exist in the metadata");
				return;
			}

			CreateEntity(instanceId, data, savedDataPrefix);
		}

		protected virtual void CreateEntity<TData>(string instanceId, string dataId, string savedDataPrefix) where TData : class, IMetaData
		{
			var data = _dataService.Get<TData>(dataId);
			if (data == null)
			{
				Debug.LogError($"instance with id {dataId} doesn't exist in the metadata");
				return;
			}
			CreateEntity(instanceId, data, savedDataPrefix);
		}

		protected virtual void CreateEntity<TData>(TData data, string savePrefix) where TData : class, IMetaData
		{
			var creationData = new CreationData()
			{
				EntityId = data.ID,
				SaveId = (savePrefix.IsNullOrEmpty() == false) ? $"{savePrefix}_{data.ID}" : data.ID
			};
			CreateEntity(data.ID, data, creationData);
		}

		protected virtual void CreateEntity<TData>(string instanceId, TData data, string savePrefix) where TData : class, IMetaData
		{
			var creationData = new CreationData()
			{
				EntityId = instanceId,
				SaveId = (savePrefix.IsNullOrEmpty() == false) ? $"{savePrefix}_{instanceId}" : instanceId
			};
			CreateEntity(instanceId, data, creationData);
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_cachedEntities = new();

			// Services.
			_dataService = DataService.Instance;
		}

		public override void Cleanup()
		{
			base.Cleanup();
			while (_cachedEntities.Count > 0)
			{
				UnloadEntity(_cachedEntities.FirstOrDefault().Value);
			}
			_cachedEntities.Clear();
		}

		public virtual void CreateSaveData(IEntity entity, string saveId)
		{
			// Handle save
			if (entity is IEntitySavedData entitySavedData && entity.Data is IMetaDataSavedData savedData)
			{
				entitySavedData.SavedData = savedData.CreateSavedData(saveId);
				entitySavedData.SavedData.Load();
			}
		}

		public virtual TReturn GetEntity<TReturn>(string instanceId) where TReturn : class, IEntity
		{
			_cachedEntities.TryGetValue(instanceId, out var returnVal);
			return returnVal as TReturn;
		}

		public virtual TReturn LoadEntity<TReturn, TData>(string instanceId, string savedDataPrefix = "") where TReturn : class, IEntity where TData : class, IMetaData
		{
			if (_cachedEntities.ContainsKey(instanceId) == false)
			{
				CreateEntity<TData>(instanceId, savedDataPrefix);
			}

			if (_cachedEntities.ContainsKey(instanceId) == false)
			{
				Debug.LogWarning($"Was not able to load or create entity with id {instanceId} it probably is missing from metadata");
				return null;
			}

			return _cachedEntities[instanceId] as TReturn;
		}

		public virtual TReturn LoadEntity<TReturn, TData>(TData data, string savedDataPrefix = "") where TReturn : class, IEntity where TData : class, IMetaData
		{
			if (_cachedEntities.ContainsKey(data.ID) == false)
			{
				CreateEntity(data, savedDataPrefix);
			}

			if (_cachedEntities.ContainsKey(data.ID) == false)
			{
				Debug.LogWarning($"Was not able to load or create entity with id {data.ID}, it probably is missing from metadata");
				return null;
			}

			return _cachedEntities[data.ID] as TReturn;
		}

		public virtual TReturn LoadEntity<TReturn, TData>(string instanceId, TData data, string savedDataPrefix = "") where TReturn : class, IEntity where TData : class, IMetaData
		{
			if (_cachedEntities.ContainsKey(instanceId) == false)
			{
				CreateEntity(instanceId, data, savedDataPrefix);
			}
			return _cachedEntities[instanceId] as TReturn;
		}

		public virtual TReturn LoadEntity<TReturn, TData>(string instanceId, string dataId, string savedDataPrefix) where TReturn : class, IEntity where TData : class, IMetaData
		{
			if (_cachedEntities.ContainsKey(instanceId) == false)
			{
				CreateEntity<TData>(instanceId, dataId, savedDataPrefix);
			}
			return _cachedEntities[instanceId] as TReturn;
		}

		public virtual TReturn[] LoadEntities<TReturn, TData>(string savedDataPrefix = "", bool ignoreInherited = true) where TReturn : class, IEntity where TData : class, IMetaData
		{
			var ids = _dataService.GetAllIds<TData>(ignoreInherited);
			return LoadEntities<TReturn, TData>(ids, savedDataPrefix);
		}

		public virtual TReturn[] LoadEntities<TReturn, TData>(string[] ids, string savedDataPrefix = "") where TReturn : class, IEntity where TData : class, IMetaData
		{
			var result = new TReturn[ids.Length];
			for (var i = 0; i < ids.Length; i++)
			{
				result[i] = LoadEntity<TReturn, TData>(ids[i], savedDataPrefix);
			}
			return result;
		}

		public virtual TReturn[] LoadEntities<TReturn, TData>(List<TData> datas) where TReturn : class, IEntity where TData : class, IMetaData
		{
			var result = new TReturn[datas.Count];
			for (var i = 0; i < datas.Count; i++)
			{
				result[i] = LoadEntity<TReturn, TData>(datas[i]);
			}
			return result;
		}

		public virtual TReturn[] LoadEntities<TReturn, TData>(TData[] data) where TReturn : class, IEntity where TData : class, IMetaData
		{
			var result = new TReturn[data.Length];
			for (var i = 0; i < data.Length; i++)
			{
				result[i] = LoadEntity<TReturn, TData>(data[i]);
			}
			return result;
		}

		public virtual void UnloadEntity(IEntity entity, bool nonPersistantOnly = false)
		{
			if (entity == null)
			{
				return;
			}
			UnloadEntity(entity.InstanceId, nonPersistantOnly);
		}

		public virtual void UnloadEntity(string instanceId, bool nonPersistantOnly = false)
		{
			if (_cachedEntities.ContainsKey(instanceId) == false)
			{
				Logger.LogWarning($"Entity {instanceId} not loaded");
				return;
			}

			var entity = _cachedEntities[instanceId];
			if (nonPersistantOnly == true && _dataService.IsPersistent(entity.Data) == true)
			{
				return;
			}
			entity.Data = null;
			if (entity is IEntitySavedData entitySaveData)
			{
				entitySaveData.SavedData = null;
			}
			entity.Cleanup();
			_cachedEntities.Remove(instanceId);
		}

		public virtual void UnloadEntities(IEnumerable<string> instanceIds, bool nonPersistentOnly = false)
		{
			foreach (var instanceId in instanceIds)
			{
				UnloadEntity(instanceId, nonPersistentOnly);
			}
		}

		public virtual void UnloadEntities(IEnumerable<IEntity> entities, bool nonPersistentOnly = false)
		{
			if (entities == null)
			{
				return;
			}
			var instanceIds = entities.Select(e => e.InstanceId);
			UnloadEntities(instanceIds, nonPersistentOnly);
		}
		#endregion
	}
}
