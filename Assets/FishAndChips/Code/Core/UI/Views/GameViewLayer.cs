using UnityEngine;

namespace FishAndChips
{
    public class GameViewLayer : MonoBehaviour
    {
		#region -- Supporting --
		public enum eGameViewLayer
		{
			Background = -1,
			Main,
			Overlay,
			SafeArea,
			Boot,
			FullScreen
		}
		#endregion

		#region -- Inspector --
		public eGameViewLayer Layer;
		public int OrderInLayer;
		#endregion

		#region -- Public Methods --
		public void DestroyChildren()
		{
			transform.DestroyChildren();
		}

		public void ParentObject(Transform toParent, bool keepWorldPosition)
		{
			toParent.SetParent(transform, keepWorldPosition);
		}
		#endregion
	}
}
