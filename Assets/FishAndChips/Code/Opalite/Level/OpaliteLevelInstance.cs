using UnityEngine;

namespace FishAndChips
{
    public class OpaliteLevelInstance : FishScript
    {
		#region -- Inspector --
		public Transform PlayerTransform;
		public Transform StartingTransform;
		#endregion

		#region -- Private Methods --
		private void OnResetLevel(OpaliteResetLevelEvent resetLevel)
		{
			ResetScene();
		}
		#endregion

		#region -- Protected Methods --
		protected override void SubscribeEventListeners()
		{
			base.SubscribeEventListeners();
			EventManager.SubscribeEventListener<OpaliteResetLevelEvent>(OnResetLevel);
		}

		protected override void UnsubscribeEventListeners()
		{
			base.UnsubscribeEventListeners();
			EventManager.UnsubscribeEventListener<OpaliteResetLevelEvent>(OnResetLevel);
		}
		#endregion

		#region -- Public Methods --
		public void ResetScene()
		{
			PlayerTransform.position = StartingTransform.position;
		}
		#endregion
	}
}
