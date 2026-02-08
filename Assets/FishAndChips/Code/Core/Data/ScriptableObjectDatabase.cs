using System.Collections.Generic;

namespace FishAndChips
{
	/// <summary>
	/// Collection of ScriptableObjectData.
	/// </summary>
	/// <typeparam name="T">Collection of this type.</typeparam>
    public class ScriptableObjectDatabase<T> where T : ScriptableObjectData
    {
		#region -- Private Member Vars --
		private List<T> _allObjects = new();
		private Dictionary<string, T> _objectDictionary = new();
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Clear collection.
		/// </summary>
		public void Clear()
		{
			_allObjects.Clear();
			_objectDictionary.Clear();
		}

		/// <summary>
		/// Add object to collection.
		/// </summary>
		/// <param name="id">Key for dictionary.</param>
		/// <param name="obj">Object being added to collection.</param>
		public void AddObject(string id, T obj)
		{
			if (_allObjects.Contains(obj) == false)
			{
				_allObjects.Add(obj);
			}
			if (_objectDictionary.ContainsKey(id) == false)
			{
				_objectDictionary.Add(id, obj);
			}
		}

		/// <summary>
		/// Attempty to retrieve object from collection matching key.
		/// </summary>
		/// <param name="id">Key of object trying to get.</param>
		/// <returns>Object if key matches, otherwise null.</returns>
		public T FetchObject(string id)
		{
			if (id.IsNullOrEmpty())
			{
				return null;
			}
			_objectDictionary.TryGetValue(id, out var val);
			return val;
		}

		/// <summary>
		/// Return list of all objects in collection.
		/// </summary>
		/// <returns>All objects in collection.</returns>
		public IList<T> GetAllObjects()
		{
			return _allObjects.AsReadOnly();
		}

		/// <summary>
		/// Get collection of objects that match passed in ids.
		/// </summary>
		/// <param name="ids">Keys of objects trying to be retrieved.</param>
		/// <returns>Collection of objects that match passed in ids.</returns>
		public IList<T> GetObjectsCorrespondingToIds(List<string> ids)
		{
			var returnList = new List<T>();
			foreach (var obj in _allObjects)
			{
				if (ids.Contains(obj.GUID))
				{
					returnList.Add(obj);
				}
			}
			return returnList;
		}

		/// <summary>
		/// Return whether collection has object matching key.
		/// </summary>
		/// <param name="id">Key being checked.</param>
		/// <returns>True if collection has key, false otherwise.</returns>
		public bool Contains(string id)
		{
			return _objectDictionary.ContainsKey(id);
		}
		#endregion
	}
}
