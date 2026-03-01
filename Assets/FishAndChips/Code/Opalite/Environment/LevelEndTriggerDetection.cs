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
				bool passedLevel = CheckIfPassedLevel();
				EventManager.TriggerEvent<OpaliteResetLevelEvent>(new OpaliteResetLevelEvent(passedLevel));
			}
		}

		private bool CheckIfPassedLevel()
		{
			return true;
		}
		#endregion
	}
}
