using UnityEngine;

namespace FishAndChips
{
    public class OpaliteLevelInstance : FishScript
    {
		#region -- Inspector --
		public Transform PlayerTransform;
		public Transform StartingTransform;
		public BoxCollider LevelEndTrigger;
		#endregion

		#region -- Private Methods --
		private void OnResetLevel(OpaliteResetLevelEvent resetEvent)
		{
			ResetScene(resetEvent.Passed);
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
		public void ResetScene(bool passed)
		{
			var currentPosition = PlayerTransform.position;
			currentPosition = currentPosition.WithZ(StartingTransform.position.z);
			//float horizontalPosition = currentPosition.x - StartingTransform.position.x;
			//currentPosition = currentPosition.WithX(horizontalPosition);

			if (LevelEndTrigger != null)
			{
				float levelEndTriggerXPosition = LevelEndTrigger.transform.position.x;
				float levelEndTriggerXScale = LevelEndTrigger.size.x;

				float min = levelEndTriggerXPosition - (levelEndTriggerXScale / 2.0f);
				float max = levelEndTriggerXPosition + (levelEndTriggerXScale / 2.0f);

				float currentX = currentPosition.x;
				currentX = currentX.Remap(min, max, -1f, 1f);

				Debug.Log($"Min = {min.ToString()} : Max = {max.ToString()} : CurrentPosition = {currentPosition.x.ToString()}");
				currentPosition = currentPosition.WithX(currentX);
			}
			PlayerTransform.position = currentPosition;
		}
		#endregion
	}
}
