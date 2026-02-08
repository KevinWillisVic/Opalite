using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
    public class OverlaySettings : GameOverlay
    {
		#region -- Inspector --
		[Header("Buttons")]
		public Button ClearButton;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Handle setting up button on click callbacks.
		/// </summary>
		private void SetupButtons()
		{
			if (ClearButton != null)
			{
				ClearButton.onClick.AddListener(HandleHitRecycleButton);
			}
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();

			SetupButtons();
		}

		/// <summary>
		/// Attempt to reset the game.
		/// </summary>
		public void ResetGame()
		{
			var yesNoOverlay = _uiService.ShowOverlay<OverlayYesNo>("OverlayYesNo");
			// Configure yes no overlay.
			yesNoOverlay.Initialize("Are You Sure?", 
				"This will delete all your saved data and you will be on a fresh game.");
			yesNoOverlay.SetButtonText("Yes", "No");

			// User must select yes in order to reset the game.
			yesNoOverlay.YesSelected += o =>
			{
				DismissSelected();
				EventManager.TriggerEvent(new ToastEvent("Fresh Game!"));
				// Reset Game.
				EventManager.TriggerEvent(new GameResetEvent());
			};

			DismissSelected();
		}

		/// <summary>
		/// Handle hitting on the recycle/clear button.
		/// </summary>
		public void HandleHitRecycleButton()
		{
			EventManager.TriggerEvent<RecycleTriggerableEvent>(new RecycleTriggerableEvent());

			string toastMessage = "Board Cleared!";
			CraftingSystemCraftingService craftingService = CraftingSystemCraftingService.Instance;
			if (craftingService != null && craftingService.GameplayBoard != null)
			{
				if (craftingService.GameplayBoard.RecycleState == SimpleGameplayBoard.eRecycleState.CleanState)
				{
					toastMessage = "Undone!";
				}
			}
			EventManager.TriggerEvent(new ToastEvent(toastMessage));
		}
		#endregion
	}
}
