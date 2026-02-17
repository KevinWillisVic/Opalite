using UnityEngine;

namespace FishAndChips
{
    public class OpaliteCameraPosition : MonoBehaviour
    {

		#region -- Inspector --
		public Transform CameraPosition;
		#endregion

		#region -- Private Methods --
		private void Update()
		{
			if (CameraPosition == null)
			{
				return;
			}
			transform.position = CameraPosition.position;
		}
		#endregion
	}
}
