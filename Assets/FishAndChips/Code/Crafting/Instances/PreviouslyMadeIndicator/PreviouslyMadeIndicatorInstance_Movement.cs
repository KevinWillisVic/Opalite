using UnityEngine;
using UnityEngine.Playables;

namespace FishAndChips
{
	/// <summary>
	/// Handle the movement of the PreviouslyMadeIndicatorInstance.
	/// </summary>
    public partial class PreviouslyMadeIndicatorInstance
    {
		#region -- Inspector --
		[Header("Movement")]
		[Tooltip("This is animated in the timeline and used to lerp between the starting and target position.")]
		public float AnimationProgress = 0f;
		[Tooltip("Animation for the indicator appearing.")]
		public PlayableDirector AppearTimeline;
		#endregion

		#region -- Private Member Vars --
		private Vector3 _startingPosition = Vector3.zero;
		private Vector3 _targetPosition = Vector3.zero;
		#endregion

		#region -- Private Methods --
		private void Update()
		{
			if (this == null || transform == null)
			{
				return;
			}
			UpdatePosition();
		}
		#endregion

		#region -- Protected Methods --
		/// <summary>
		/// Lerp between starting and target position based on AnimationProgress which is animated in the timeline.
		/// </summary>
		protected virtual void UpdatePosition()
		{
			float tValue = Mathf.SmoothStep(0.0f, 1.0f, AnimationProgress);
			transform.localPosition = Vector3.Lerp(_startingPosition, _targetPosition, tValue);
		}

		/// <summary>
		/// Prepare timeline aided movement.
		/// </summary>
		protected virtual void InitializeMovement()
		{
			AnimationProgress = 0f;

			// Calculate end points we are lerping between.
			_startingPosition = transform.localPosition;
			_targetPosition = _craftingService.GameplayBoard.GetPositionOnCircle(_startingPosition, true, true);

			AppearTimeline.PlaySafe();
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Repool instance.
		/// Triggered by timeline signal track.
		/// </summary>
		public virtual void OnAnimationComplete()
		{
			_poolingService.PoolPreviouslyMadeIndicatorInstance(this);
		}
		#endregion
	}
}
