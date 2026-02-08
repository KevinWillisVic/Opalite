using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
	/// <summary>
	/// Main view for the gameplay.
	/// </summary>
    public class GameplaySceneView : GameView
    {
		#region -- Inspector --
		[Header("Displayed Craft Items")]
		public CraftItemComponentList DisplayedCraftItems;

		[Header("Gameboard")]
		public SimpleGameplayBoard SimpleGameplayBoard;

		[Header("Buttons")]
		public Button HintButton;
		public Button SettingsButton;
		public Button ClearButton;
		public Button EncyclopediaButton;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemHintService _hintService;
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Member Vars --
		private string _lastSearch;
		private List<CraftItemEntity> _craftItemEntities = new();
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Handle what happens when the game is reset.
		/// </summary>
		private void OnResetGame(GameResetEvent resetEvent)
		{
			// Remove all search filters.
			SetUpCraftItemsToDisplay(string.Empty);
			UpdateVisibleStateOfHintButton();
		}

		/// <summary>
		/// Handle what happens when a CraftItem has been unlocked.
		/// </summary>
		private void OnCraftItemUnlocked(CraftItemEntityUnlockEvent gameEvent)
		{
			// Re-do last search to potentially get rid of any depleted items.
			SetUpCraftItemsToDisplay(_lastSearch);
			UpdateVisibleStateOfHintButton();
		}

		/// <summary>
		/// Handler for search raised events.
		/// </summary>
		private void OnSearchRaised(CraftItemSearchEvent gameEvent)
		{
			SetUpCraftItemsToDisplay(gameEvent.SearchFilter);
		}

		/// <summary>
		/// Handle start up of the game.
		/// </summary>
		private void OnSceneReady(OnGameSceneReady gameEvent)
		{
			SetUpCraftItemsToDisplay(string.Empty);
			UpdateVisibleStateOfHintButton();
		}

		/// <summary>
		/// Does the CraftItem matches the search filter.
		/// </summary>
		/// <param name="entity">CraftItem being compared.</param>
		/// <returns>True if matches the search filter, false otherwise.</returns>
		private bool MatchesSearch(CraftItemEntity entity)
		{
			if (_lastSearch.IsNullOrEmpty() == true)
			{
				return true;
			}
			var modelData = entity.CraftItemData.CraftItemModelData;
			var entityName = modelData.DisplayName;

			bool nameMatches = entityName.StartsWith(_lastSearch, System.StringComparison.OrdinalIgnoreCase);
			return nameMatches;
		}

		/// <summary>
		/// Determine if CraftItem should be displayed.
		/// </summary>
		/// <param name="entity">CraftItem being checked.</param>
		/// <returns>True if should be displayed, false otherwise.</returns>
		private bool IsCraftItemValidForDisplaying(CraftItemEntity entity)
		{
			if (entity == null)
			{
				return false;
			}
			if (MatchesSearch(entity) == false)
			{
				return false;
			}
			// Final items and depleted items should not be displayed.
			return entity.Unlocked &&
				_craftingService.IsFinalItem(entity) == false && 
				_craftingService.IsDepletedItem(entity) == false;
		}


		/// <summary>
		/// Determine if hint button should be displayed.
		/// </summary>
		private void UpdateVisibleStateOfHintButton()
		{
			HintButton.SetActiveSafe(_hintService.HasHintAvailable());
		}

		/// <summary>
		/// Handle setting up button on click callbacks.
		/// </summary>
		private void SetupButtons()
		{
			if (HintButton != null)
			{
				HintButton.onClick.AddListener(HandleHitHintButton);
			}

			if (SettingsButton != null)
			{
				SettingsButton.onClick.AddListener(HandleHitSettingsButton);
			}

			if (ClearButton != null)
			{
				ClearButton.onClick.AddListener(HandleHitRecycleButton);
			}

			if (EncyclopediaButton != null)
			{
				EncyclopediaButton.onClick.AddListener(HandleHitEncyclopediaButton);
			}
		}
		#endregion

		#region -- Protected Methods --
		/// <summary>
		/// Handle event subscribtion.
		/// </summary>
		protected override void SubscribeListeners()
		{
			base.SubscribeListeners();
			EventManager.SubscribeEventListener<GameResetEvent>(OnResetGame);
			EventManager.SubscribeEventListener<OnGameSceneReady>(OnSceneReady);
			EventManager.SubscribeEventListener<CraftItemSearchEvent>(OnSearchRaised);
			EventManager.SubscribeEventListener<CraftItemEntityUnlockEvent>(OnCraftItemUnlocked);
		}

		/// <summary>
		/// Handle event unsubscription.
		/// </summary>
		protected override void UnsubsribeListeners()
		{
			base.UnsubsribeListeners();
			EventManager.UnsubscribeEventListener<GameResetEvent>(OnResetGame);
			EventManager.UnsubscribeEventListener<OnGameSceneReady>(OnSceneReady);
			EventManager.UnsubscribeEventListener<CraftItemSearchEvent>(OnSearchRaised);
			EventManager.SubscribeEventListener<CraftItemEntityUnlockEvent>(OnCraftItemUnlocked);
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Setup view.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			// Services.
			_hintService = CraftingSystemHintService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;

			SetupButtons();
		}

		/// <summary>
		/// Set up list of CraftItems to display with supplied search filter.
		/// </summary>
		/// <param name="filter">Search filter to limit displayed CraftItems.</param>
		public void SetUpCraftItemsToDisplay(string filter)
		{
			_lastSearch = filter;
			if (DisplayedCraftItems == null)
			{
				return;
			}

			_craftItemEntities.Clear();
			_craftItemEntities.AddRange(_craftingService.CraftItemEntities);

			// Filter.
			_craftItemEntities.RemoveAll(c => IsCraftItemValidForDisplaying(c) == false);
			DisplayedCraftItems.FillList(_craftItemEntities);
		}

		/// <summary>
		/// Show the settings overlay.
		/// </summary>
		public void HandleHitSettingsButton()
		{
			var settingsOverlay = _uiService.ShowOverlay<OverlaySettings>(UIEnumTypesBase.eOverlayTypeBase.OverlaySettings.ToString(), "Settings");
		}

		/// <summary>
		/// Show the enccylopedia view.
		/// </summary>
		public void HandleHitEncyclopediaButton()
		{
			_uiService.ActivateView(UIEnumTypes.eViewType.EncyclopediaView.ToString());
		}

		/// <summary>
		/// Click callback for hint button.
		/// </summary>
		public void HandleHitHintButton()
		{
			if (_hintService.HasHintAvailable() == true)
			{
				var craftItemEntity = _hintService.GetCraftItemEntityAsHint();
				// Give CraftItem as hint.
				if (craftItemEntity != null)
				{
					// For keyword tracking.
					craftItemEntity.SetHintState(true);

					// Create overlay indicating this item has a hint given.
					var hintOverlay = _uiService.ShowOverlay<OverlayHint>(UIEnumTypes.eOverlayType.OverlayHint.ToString());
					if (hintOverlay != null)
					{
						hintOverlay.Initialize(craftItemEntity);
					}
				}
				else
				{
					// TODO : See about recipe hint.
				}
			}
			else
			{
				UpdateVisibleStateOfHintButton();
			}
		}

		/// <summary>
		/// Handle hitting the recycle button.
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
