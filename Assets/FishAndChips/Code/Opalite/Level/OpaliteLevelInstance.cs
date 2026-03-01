using UnityEngine;

namespace FishAndChips
{
    public class OpaliteLevelInstance : FishScript
    {
		#region -- Inspector --
		[Header("Position Helpers")]
		public Transform PlayerTransform;
		public Transform StartingTransform;
		public BoxCollider LevelEndTrigger;
		#endregion

		#region -- Private Methods --
		private void OnResetLevel(OpaliteResetLevelEvent resetEvent)
		{
			ResetScene(resetEvent.Passed);
		}

		private void ResetPlayerPosition()
		{
			var currentPosition = PlayerTransform.position;

			// New z position.
			float adjustedZPosition = StartingTransform.position.z;

			// New y position.
			// Remap
			float levelEndTriggerXPosition = LevelEndTrigger.transform.position.x;
			float levelEndTriggerXScale = LevelEndTrigger.size.x;

			float min = levelEndTriggerXPosition - (levelEndTriggerXScale / 2.0f);
			float max = levelEndTriggerXPosition + (levelEndTriggerXScale / 2.0f);

			// TODO : This only works on the -1 to 1 mapping, if its a different value it must be updated.
			float adjustedXPosition = currentPosition.x.Remap(min, max, -1f, 1f);

			currentPosition = currentPosition.WithX(adjustedXPosition).WithZ(adjustedZPosition);
			PlayerTransform.position = currentPosition;
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
			ResetPlayerPosition();
		}
		#endregion
	}
}
