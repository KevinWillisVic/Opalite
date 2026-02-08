using UnityEngine;

namespace FishAndChips
{
    public class EnableTracker : MonoBehaviour
    {
		public string Key = string.Empty;

		private void OnEnable()
		{
			Debug.Log($"Enable {Key}");
		}

		private void OnDisable()
		{
			Debug.Log($"Disable {Key}");
		}
	}
}
