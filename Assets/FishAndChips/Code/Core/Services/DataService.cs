using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace FishAndChips
{
    public class DataService : Singleton<DataService>, IInitializable, ICleanable
	{
		#region -- Properties --
		public bool IsPersistentDataLoaded { get; set; }
		#endregion

		#region -- Private Member Vars --
		private Dictionary<string, IMetaDataStaticData> _persistentData = new();
		private Dictionary<string, IMetaDataStaticData> _temporaryData = new();

		private readonly Dictionary<string, List<string>> _eventReferences = new();

		private Action _onGlobalDatabaseLoaded;
		private Action _onGlobablDatabaseUnloaded;

		private Action _onContentDatabaseLoaded;
		private Action _onContentDatabaseUnlaoded;
		#endregion

		#region -- Private Methods --
		private async Task<MetadataDatabase> LoadDatabaseFromResources(string name)
		{
			var databaseRequest = Resources.LoadAsync<MetadataDatabase>(name);
			while (databaseRequest.isDone == false)
			{
				await Awaitable.EndOfFrameAsync();
			}
			var database = databaseRequest.asset as MetadataDatabase;
			return database;
		}

		private bool OnListenerAdded(string eventType, Action action)
		{
			if (action.Method.Name.Contains('<') && action.Method.Name.Contains('>'))
			{
				Logger.LogWarning($"Avoid using inside methods on a listener.\nEventType: {eventType} - Method Name: {action.Method.ReflectedType}.{action.Method.Name}");
			}
			if (_eventReferences.ContainsKey(eventType) == false)
			{
				_eventReferences.Add(eventType, new List<string>());
			}
			var methodName = action.UniqueMethodName();
			if (_eventReferences[eventType].Contains(methodName))
			{
				Logger.LogWarning($"The listener {eventType} has more than one subscriber to the class {action.Method.ReflectedType} and method {action.Method.Name}");
				return false;
			}
			_eventReferences[eventType].Add(methodName);
			return true;
		}

		private void OnListenerRemoved(string eventType, Action action)
		{
			if (_eventReferences.ContainsKey(eventType) == false)
			{
				return;
			}
			var methodName = action.UniqueMethodName();
			_eventReferences[eventType].Remove(methodName);
		}
		#endregion

		#region -- Protected Methods --
		protected virtual async Task LoadPersistentData()
		{
			await LoadDatabase(CoreConstants.GlobalDatabase, isPersistent: true);
			IsPersistentDataLoaded = true;
		}

		protected void FillDatabaseFromStaticData(IMetaDataStaticData[] staticDatas,
			Dictionary<string, IMetaDataStaticData> database)
		{
			for (int i = 0; i < staticDatas.Length; i++)
			{
				var data = staticDatas[i];
				try
				{
					database.Add(data.ID, data);
				}
				catch (Exception e)
				{
					Logger.LogException(e);
				}
			}
		}

		protected void FillDatabaseFromOtherDatabase(Dictionary<string, IMetaDataStaticData> from,
			Dictionary<string, IMetaDataStaticData> to)
		{
			to.Clear();
			foreach (var data in from.Values)
			{
				to.Add(data.ID, data);
			}
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_persistentData = new();
			_temporaryData = new();
		}

		public override void Cleanup()
		{
			base.Cleanup();
			_persistentData.Clear();
			_temporaryData.Clear();

			_onGlobalDatabaseLoaded = null;
			_onGlobablDatabaseUnloaded = null;
			_onContentDatabaseLoaded = null;
			_onContentDatabaseUnlaoded = null;

			_eventReferences.Clear();
		}

		public virtual async Task Load()
		{
			await LoadPersistentData();
		}

		public void SubscribeToOnGlobalDatabaseLoaded(Action action)
		{
			if (!OnListenerAdded("OnGlobalDatabaseLoaded", action))
			{
				return;
			}
			_onGlobalDatabaseLoaded += action;
		}

		public void UnsubscribeToOnGlobalDatabaseLoaded(Action action)
		{
			OnListenerRemoved("OnGlobalDatabaseLoaded", action);
			_onGlobalDatabaseLoaded -= action;
		}

		public void SubscribeToOnGlobalDatabaseUnloaded(Action action)
		{
			if (!OnListenerAdded("OnGlobalDatabaseUnloaded", action))
			{
				return;
			}
			_onGlobablDatabaseUnloaded += action;
		}

		public void UnsubscribeToOnGlobalDatabaseUnloaded(Action action)
		{
			OnListenerRemoved("OnGlobalDatabaseUnloaded", action);
			_onGlobablDatabaseUnloaded -= action;
		}

		public void SubscribeToOnContentDatabaseLoaded(Action action)
		{
			if (!OnListenerAdded("OnContentDatabaseLoaded", action))
			{
				return;
			}
			_onContentDatabaseLoaded += action;
		}

		public void UnsubscribeToOnContentDatabaseLoaded(Action action)
		{
			OnListenerRemoved("OnContentDatabaseLoaded", action);
			_onContentDatabaseLoaded -= action;
		}

		public void SubscribeToOnContentDatabaseUnloaded(Action action)
		{
			if (!OnListenerAdded("OnContentDatabaseUnloaded", action))
			{
				return;
			}
			_onContentDatabaseUnlaoded += action;
		}

		public void UnsubscribeToOnContentDatabaseUnloaded(Action action)
		{
			OnListenerRemoved("OnContentDatabaseUnloaded", action);
			_onContentDatabaseUnlaoded -= action;
		}

		public virtual async Task LoadDatabase(string name, bool isPersistent)
		{
			MetadataDatabase  database = await LoadDatabaseFromResources(name);
			if (database == null)
			{
				return;
			}
			var data = database.StaticData.ToArray();
			var dataDict = new Dictionary<string, IMetaDataStaticData>();
			FillDatabaseFromStaticData(data, dataDict);

			if (isPersistent == true)
			{
				FillDatabaseFromOtherDatabase(dataDict, _persistentData);
				_onGlobalDatabaseLoaded.FireSafe();
			}
			else
			{
				FillDatabaseFromOtherDatabase(dataDict, _temporaryData);
				_onContentDatabaseLoaded.FireSafe();
			}
		}

		public virtual void UnloadDatabase(string name)
		{
			if (name == CoreConstants.GlobalDatabase)
			{
				_persistentData.Clear();
				_onGlobablDatabaseUnloaded.FireSafe();
			}
			else
			{
				_temporaryData.Clear();
				_onContentDatabaseUnlaoded.FireSafe();
			}
		}

		public bool IsPersistent(IMetaData data)
		{
			return _persistentData.TryGetValue(data.ID, out _);
		}

		public bool ExistsInActiveContent(string dataId)
		{
			return _persistentData.ContainsKey(dataId) || _temporaryData.ContainsKey(dataId);
		}

		public TStaticData Get<TStaticData>(string id) where TStaticData : class, IMetaDataStaticData
		{
			if (_persistentData.TryGetValue(id, out var data) == false)
			{
				_temporaryData.TryGetValue(id, out data);
			}
			return data as TStaticData;
		}

		public TStaticData[] GetAll<TStaticData>(bool includePersistent = true,
			bool includeTemporary = true) where TStaticData : class, IMetaDataStaticData
		{
			var returnList = new List<TStaticData>();
			if (includePersistent == true)
			{
				returnList.AddRange(_persistentData.Values.OfType<TStaticData>().ToList());
			}
			if (includeTemporary == true)
			{
				returnList.AddRange(_temporaryData.Values.OfType<TStaticData>().ToList());
			}
			return returnList.ToArray();
		}

		public List<TStaticData> GetAll<TStaticData>(string[] ids) where TStaticData : class, IMetaDataStaticData
		{
			return GetAll<TStaticData>().Where(data => ids.Contains(data.ID)).ToList();
		}

		public TStaticData[] GetAll<TStaticData>(Func<TStaticData, bool> predicate) where TStaticData : class, IMetaDataStaticData
		{
			return GetAll<TStaticData>().Where(predicate).ToArray();
		}

		public string[] GetAllIds<TStaticData>(bool ignoreInherited = true,
			bool includePersistent = true,
			bool includeTemporary = true) where TStaticData : class, IMetaDataStaticData
		{
			List<string> result = new();
			if (ignoreInherited)
			{
				if (includePersistent == true)
				{
					result.AddRange(_persistentData.Values.OfType<TStaticData>()
						.Where(it => it.GetType() == typeof(TStaticData))
						.Select(data => data.ID)
						.ToList());
				}

				if (includeTemporary == true)
				{
					result.AddRange(_temporaryData.Values.OfType<TStaticData>()
						.Where(it => it.GetType() == typeof(TStaticData))
						.Select(data => data.ID)
						.ToList());
				}
			}
			else
			{
				if (includePersistent == true)
				{
					result.AddRange(_persistentData.OfType<TStaticData>()
						.Select(data => data.ID)
						.ToList());
				}

				if (includeTemporary == true)
				{
					result.AddRange(_temporaryData.OfType<TStaticData>()
						.Select(data => data.ID)
						.ToList());
				}
			}

			return result.ToArray();
		}
		#endregion
	}
}
