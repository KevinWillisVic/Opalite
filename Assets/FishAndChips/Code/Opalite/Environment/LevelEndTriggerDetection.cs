using UnityEngine;

namespace FishAndChips
{
    public class LevelEndTriggerDetection : MonoBehaviour
    {
		#region -- Private Methods --
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag == "Player")
			{
				Debug.Log("OnTriggerEnter");
				EventManager.TriggerEvent<OpaliteResetLevelEvent>(new OpaliteResetLevelEvent());
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.tag == "Player")
			{
				Debug.Log("OnTriggerExit");
			}
		}
		#endregion
	}
}
