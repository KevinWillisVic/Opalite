#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;

namespace FishAndChips
{
	/// <summary>
	/// Debug component to trigger something on key press.
	/// </summary>
    public class DebugKeyPressResponse : MonoBehaviour
    {
		#region -- Inspector --
		public KeyCode TriggerKey = KeyCode.A;
		public UnityEvent PressAction;
		#endregion

		#region -- Private Methods --
		private void Update()
		{
			// Trigger action on key press.
			if (Input.GetKeyDown(TriggerKey) == true)
			{
				PressAction.FireSafe();
			}
		}
		#endregion
	}
}
#endif
