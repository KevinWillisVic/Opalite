using UnityEngine;
using System.Collections.Generic;

namespace FishAndChips
{
	/// <summary>
	/// Cache related WaitFor[x]
	/// </summary>
	public static class Yielders
    {
		#region -- Properties -- 
		public static WaitForEndOfFrame EndOfFrame
		{
			get
			{
				if (_waitForEndOfFrame == null)
				{
					_waitForEndOfFrame = new();
				}
				return _waitForEndOfFrame;
			}
		}

		public static WaitForFixedUpdate FixedUpdate
		{
			get
			{
				if (_waitForFixedUpdate == null)
				{
					_waitForFixedUpdate = new();
				}
				return _waitForFixedUpdate;
			}
		}
		#endregion

		#region -- Private Member Vars --
		private static Dictionary<float, WaitForSeconds> _timerIntervals = new();
		private static WaitForEndOfFrame _waitForEndOfFrame = new();
		private static WaitForFixedUpdate _waitForFixedUpdate = new();
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Get a cached WaitForSeconds.
		/// </summary>
		/// <param name="time">The amount desired to be waited.</param>
		/// <returns>A WaitForSeconds with the desired amount to be waited.</returns>
		public static WaitForSeconds Get(float time)
		{
			if (_timerIntervals.ContainsKey(time) == false)
			{
				_timerIntervals.Add(time, new WaitForSeconds(time));
			}
			return _timerIntervals[time];
		}
		#endregion
	}
}
