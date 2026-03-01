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
				// TODO : Verify flag of passed level.
				bool passedLevel = true;
				EventManager.TriggerEvent<OpaliteResetLevelEvent>(new OpaliteResetLevelEvent(passedLevel));
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
