using UnityEngine;
using TMPro;

namespace FishAndChips
{
	/// <summary>
	/// Handle updating the text of the clear button.
	/// </summary>
    public class ClearButtonDisplay : MonoBehaviour
    {
		#region -- Inspector --
		public TextMeshProUGUI ButtonText;
		#endregion

		#region -- Private Methods --
		private void OnEnable()
		{
			EventManager.SubscribeEventListener<RecycleStateUpdateEvent>(OnButtonRecycleStateChanged);
			SetInitialButtonState();
		}

		private void OnDisable()
		{
			EventManager.UnsubscribeEventListener<RecycleStateUpdateEvent>(OnButtonRecycleStateChanged);
		}

		/// <summary>
		/// Set initial text of the button.
		/// </summary>
		private void SetInitialButtonState()
		{
			// TODO : See if there is a better way of doing this.
			CraftingSystemCraftingService craftingService = CraftingSystemCraftingService.Instance;
			if (craftingService != null && craftingService.GameplayBoard != null)
			{
				SetTextBasedOnState(craftingService.GameplayBoard.RecycleState);
			}
		}

		/// <summary>
		/// Event handler for changes to recycle state.
		/// </summary>
		/// <param name="gameEvent">Event for recycle state.</param>
		private void OnButtonRecycleStateChanged(RecycleStateUpdateEvent gameEvent)
		{
			SetTextBasedOnState(gameEvent.RecycleState);
		}

		/// <summary>
		/// Set the text based on the recycle state.
		/// </summary>
		/// <param name="state">The state to base the text on.</param>
		private void SetTextBasedOnState(SimpleGameplayBoard.eRecycleState state)
		{
			switch (state)
			{
				case SimpleGameplayBoard.eRecycleState.CleanState:
					ButtonText.SetTextSafe("Clear");
					break;
				case SimpleGameplayBoard.eRecycleState.UndoState:
					ButtonText.SetTextSafe("Undo Clear");
					break;
			}
		}
		#endregion
	}
}
