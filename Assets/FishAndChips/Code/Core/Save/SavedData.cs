using UnityEngine;
using System;

namespace FishAndChips
{
	/// <summary>
	/// Base class for saveable objects in the game.
	/// </summary>
    [Serializable]
    public abstract class SavedData : ISavedData
    {
		#region -- Properties --
		public long TimeUpdated => _timeUpdated;
		public long TimeCreated => _timeCreated;
		#endregion

		#region -- Protected Member Vars --
		protected string _saveId = string.Empty;

		[SerializeField] protected long _timeUpdated;
		[SerializeField] protected long _timeCreated;
		#endregion

		#region -- Constructors --
		public SavedData(string saveId)
		{
			_saveId = saveId;
		}
		#endregion

		#region -- Protected Methods --
		/// <summary>
		/// Mark creation time and save to create file.
		/// </summary>
		protected virtual void Create()
		{
			_timeCreated = DateTime.UtcNow.ToTimestamp();
			Save();
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Attempt to deserialize into object, or create fresh save.
		/// </summary>
		public virtual void Load()
		{
			// If unable to retrieve json or deserialize json into object, create new save.
			if (Deserialize() == false)
			{
				Create();
			}
		}

		/// <summary>
		/// Turn object into json.
		/// </summary>
		public virtual void Save()
		{
			try
			{
				_timeUpdated = DateTime.UtcNow.ToTimestamp();
				var dataString = JsonUtility.ToJson(this);
				SavingService.Instance.SaveJson(_saveId, dataString);
			}
			catch (Exception e)
			{
				Logger.LogError($"[{GetType().Name}] Error in save.\n{e}");
			}
		}

		/// <summary>
		/// Attempt to turn saved json into object.
		/// </summary>
		/// <returns>True if successful, false otherwise.</returns>
		public virtual bool Deserialize()
		{
			var dataString = SavingService.Instance.LoadJson(_saveId);
			if (dataString.IsNullOrEmpty())
			{
				return false;
			}

			try
			{
				JsonUtility.FromJsonOverwrite(dataString, this);
			}
			catch (Exception e)
			{
				Logger.LogError($"[{GetType().Name}] Error in deserialize.\n{e}");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Delete saved file.
		/// </summary>
		public virtual void Delete()
		{
			SavingService.Instance.DeleteFile(_saveId);
		}

		/// <summary>
		/// Handle save state on game reset / object reset.
		/// </summary>
		public virtual void Reset()
		{
			Save();
		}

		public virtual void Dispose()
		{
			Save();
		}
		#endregion
	}
}
