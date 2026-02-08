using UnityEngine;
using System.Collections.Generic;

namespace FishAndChips
{
	/// <summary>
	/// Simple Uility component to turn gameobjects on / off.
	/// </summary>
    public class GameObjectEnabler : MonoBehaviour
    {
		#region -- Inspector --
		public List<GameObject> ToEnableOnActive;
		public List<GameObject> ToEnableOnInactive;
		#endregion

		#region -- Public Methods --
		public void SetEnabled(bool value)
		{
			if (this == null || gameObject == null)
			{
				return;
			}

			foreach (var enableOnActive in ToEnableOnActive)
			{
				enableOnActive.SetActiveSafe(value);
			}

			foreach (var enableOnInactive in ToEnableOnInactive)
			{
				enableOnInactive.SetActiveSafe(!value);
			}
		}
		#endregion
	}
}
