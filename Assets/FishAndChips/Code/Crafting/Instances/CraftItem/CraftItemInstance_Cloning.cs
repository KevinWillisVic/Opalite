using UnityEngine;

namespace FishAndChips
{
	/// <summary>
	/// Handle cloning logic for CraftItemInstance.
	/// </summary>
    public partial class CraftItemInstance
    {
		#region -- Inspector --
		[Header("Cloning")]
		[Tooltip("How many taps must occur before a clone happens.")]
		public int TapsToTriggerClone = 2;
		[Tooltip("Time in which taps must occur for a clone to happen.")]
		public float TimingThresholdForClone = 0.5f;
		#endregion

		#region -- Private Member Vars --
		private int _currentTapCount = 0;
		private float _cloningCutoffTime = 0f;

		private SimpleGameplayBoard _board = null;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Position CraftItemInstance on the board.
		/// </summary>
		/// <param name="instance"></param>
		private void PositionClonedInstance(CraftItemInstance instance)
		{
			if (instance == null)
			{
				return;
			}
			if (_board == null)
			{
				UIService uiService = UIService.Instance;
				var gameplayView = uiService.GetView<GameplaySceneView>();
				if (gameplayView != null)
				{
					_board = gameplayView.SimpleGameplayBoard;
				}
			}
			if (_board != null)
			{
				var pos = _board.GetPositionOnCircle(instance.transform.localPosition, true, true);
				instance.transform.localPosition = pos;
			}
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Check if instance was selected after the cut off threshold,
		/// if so it should reset the cloning variables.
		/// </summary>
		public void AttemptResetCloningVariables()
		{
			// We care about checking the release on the press down, so here we just reset the cloning variables if needed.
			if (Time.time > _cloningCutoffTime)
			{
				ResetCloningVariables();
			}
		}

		/// <summary>
		/// Reset cloning variables. Set tap count to 0 and calculate new cutoff threshold.
		/// </summary>
		public void ResetCloningVariables()
		{
			_currentTapCount = 0;
			_cloningCutoffTime = Time.time + TimingThresholdForClone;
		}

		/// <summary>
		/// On release of the instance and if nothing was crafted attempt cloning.
		/// </summary>
		public void AttemptCloning()
		{
			// Time lapsed, reset variables and start counting towards cloning fresh.
			if (Time.time > _cloningCutoffTime)
			{
				ResetCloningVariables();
				_currentTapCount++;
				return;
			}

			_currentTapCount++;
			if (_currentTapCount >= TapsToTriggerClone)
			{
				var instance = _craftingService.SpawnClone(this);
				PositionClonedInstance(instance);
				ResetCloningVariables();
			}
		}
		#endregion
	}
}
