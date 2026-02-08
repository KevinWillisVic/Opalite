using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace FishAndChips
{
    public class LoadingOverlay : GameOverlay
    {
		#region -- Inspector --
		public PlayableDirector LoadingLoopable;
		public TextMeshProUGUI StatusText;
		public FillableBar ProgressBar;
		public bool UserControlledDismissal = false;
		public GameObjectEnabler LoadCompleteEnabler;
		#endregion

		#region -- Private Member Vars --
		private bool _loadCompleted;
		#endregion

		#region -- Private Methods --
		private void OnLoadStart(OnLoadStartEvent gameEvent)
		{
			if (gameEvent == null)
			{
				return;
			}
			OnLoadStart(gameEvent.Message, gameEvent.MaxValue);
		}

		private void OnLoadStart(string loadTypeString, int maxValue)
		{
			if (loadTypeString.IsNullOrEmpty() == false)
			{
				UpdateStatusText(loadTypeString);
			}

			if (_loadCompleted == true)
			{
				return;
			}

			if (ProgressBar != null)
			{
				ProgressBar.ShowMaxValue = false;
				ProgressBar.JumpToValue = true;
				ProgressBar.SetActiveSafe(true);
				ProgressBar.SetMaxValue(maxValue);
				ProgressBar.SetCurrentValue(0);
			}
		}

		private void OnLoadProgress(OnLoadProgressEvent gameEvent)
		{
			if (gameEvent == null)
			{
				return;
			}
			OnLoadProgress(gameEvent.Message);
		}

		private void OnLoadProgress(string loadProgressString)
		{
			if (loadProgressString.IsNullOrEmpty() == false)
			{
				UpdateStatusText(loadProgressString);
			}

			if (_loadCompleted == true)
			{
				return;
			}

			if (ProgressBar != null)
			{
				var currentValue = Mathf.Min(ProgressBar.MaxValue, ProgressBar.CurrentValue + 1);
				ProgressBar.ShowMaxValue = false;
				ProgressBar.JumpToValue = false;
				ProgressBar.SetActiveSafe(true);
				ProgressBar.SetCurrentValue(currentValue);
			}
		}

		private void SubscribeEventListeners()
		{
			EventManager.SubscribeEventListener<OnLoadStartEvent>(OnLoadStart);
			EventManager.SubscribeEventListener<OnLoadProgressEvent>(OnLoadProgress);
		}

		private void UnsubscribeEventListeners()
		{
			EventManager.UnsubscribeEventListener<OnLoadStartEvent>(OnLoadStart);
			EventManager.UnsubscribeEventListener<OnLoadProgressEvent>(OnLoadProgress);
		}
		#endregion

		#region -- Protected Methods --
		protected override void OnActivationDirectorComplete()
		{
			base.OnActivationDirectorComplete();
			LoadingLoopable.PlaySafe();
		}
		#endregion

		#region -- Public Methods --
		public override void Activate()
		{
			base.Activate();
			SubscribeEventListeners();
			if (ProgressBar != null)
			{
				ProgressBar.SetMaxValue(1);

				bool previousJumpToValue = ProgressBar.JumpToValue;
				ProgressBar.JumpToValue = true;
				ProgressBar.SetCurrentValue(0);
				ProgressBar.JumpToValue = previousJumpToValue;
			}
			if (LoadCompleteEnabler != null)
			{
				LoadCompleteEnabler.SetEnabled(false);
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();
			if (ProgressBar != null)
			{
				ProgressBar.SetCurrentValue(ProgressBar.MaxValue);
			}
		}

		public override void DismissSelected()
		{
			base.DismissSelected();
			UnsubscribeEventListeners();
		}

		public void UpdateStatusText(string text)
		{
			StatusText.SetTextSafe(text);
		}

		public void UpdatePercentComplete(float normalizedPercent)
		{
			if (ProgressBar != null)
			{
				ProgressBar.SetCurrentValue(Mathf.Clamp01(normalizedPercent));
			}
		}

		public virtual void OnLoadComplete()
		{
			_loadCompleted = true;
			ProgressBar.SetActiveSafe(false);

			if (LoadCompleteEnabler != null)
			{
				LoadCompleteEnabler.SetEnabled(true);
			}
		}
		#endregion
	}
}
