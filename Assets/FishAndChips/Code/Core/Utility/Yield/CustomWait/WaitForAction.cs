using UnityEngine;
using System;

namespace FishAndChips
{
	/// <summary>
	/// Custom yield instruction that waits for an action to be fired.
	/// </summary>
	public class WaitForAction : CustomYieldInstruction
    {
		#region -- Properties --
		public override bool keepWaiting
		{
			get
			{
				return _continueWaiting;
			}
		}
		#endregion

		#region -- Private Member Vars --
		private bool _continueWaiting = true;
		private Action _cachedAction;
		#endregion

		#region -- Constructor --
		public WaitForAction(ref Action action)
		{
			action += OnActionRaised;
			_cachedAction = action;
		}
		#endregion

		#region -- Private Methods --
		private void OnActionRaised()
		{
			_continueWaiting = false;
			if (_cachedAction != null)
			{
				_cachedAction -= OnActionRaised;
			}
		}
		#endregion
	}
}
