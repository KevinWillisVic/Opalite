using UnityEngine;

namespace FishAndChips
{
    public class FishScript : MonoBehaviour
    {
		#region -- Protected Methods --
		protected virtual void Start()
		{
			SubscribeEventListeners();
		}

		protected virtual void OnDestroy()
		{
			UnsubscribeEventListeners();
		}

		protected virtual void SubscribeEventListeners()
		{
		}

		protected virtual void UnsubscribeEventListeners()
		{
		}
		#endregion
	}
}
