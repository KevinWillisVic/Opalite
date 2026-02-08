using System;
using UnityEngine;

namespace FishAndChips
{
	/// <summary>
	/// Base class for save data for an unlockable entity.
	/// </summary>
    [Serializable]
    public class UnlockableEntitySavedData : SavedData
    {
		#region -- Properties --
		#region -- Unlock --
		public bool Unlocked => _unlocked;
		public long TimeUnlocked => _timeUnlocked;
		#endregion

		#region -- Hint --
		public bool HintGiven => _hintGiven;
		#endregion
		#endregion

		#region -- Protected Member Vars --
		[SerializeField] protected bool _unlocked;
		[SerializeField] protected long _timeUnlocked;
		[SerializeField] protected bool _hintGiven;
		#endregion

		#region -- Constructors --
		public UnlockableEntitySavedData(string saveId) : base(saveId)
		{
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Set the unlocked state of the save data.
		/// </summary>
		/// <param name="state">State to set for unlock.</param>
		public void SetUnlockedState(bool state)
		{
			_unlocked = state;
			if (_unlocked == true)
			{
				_timeUnlocked = DateTime.UtcNow.ToTimestamp();
			}
			Save();
		}

		/// <summary>
		/// Set the hint state of the save data.
		/// </summary>
		/// <param name="state">State to set for hint.</param>
		public void SetHintGivenState(bool state)
		{
			_hintGiven = state;
			Save();
		}

		/// <summary>
		/// Handle save state on game reset / object reset.
		/// </summary>
		public override void Reset()
		{
			_hintGiven = false;
			_unlocked = false;
			base.Reset();
		}
		#endregion
	}
}
