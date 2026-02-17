using UnityEngine;

namespace FishAndChips
{
    public class AreaDetection : MonoBehaviour
    {
		#region -- Private Methods --
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag == "Player")
			{
				Debug.Log("OnTriggerEnter");
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
