using UnityEngine;

namespace FishAndChips
{
	public class CraftingSystemSavingService : SavingService, IInitializable
	{
		#region -- Properties --
		public static new CraftingSystemSavingService Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = (CraftingSystemSavingService)Object.FindAnyObjectByType(typeof(CraftingSystemSavingService));

						if (_instance == null)
						{
							GameObject singletonObject = new GameObject();
							_instance = singletonObject.AddComponent<CraftingSystemSavingService>();
							singletonObject.name = $"Singleton {typeof(CraftingSystemSavingService).ToString()}";
							DontDestroyOnLoad(singletonObject);
						}
					}
					return _instance as CraftingSystemSavingService;
				}
			}
		}

		public BoardSaveInfo BoardSaveState => _boardSaveState;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Member Vars --
		private BoardSaveInfo _boardSaveState;
		#endregion

		#region -- Private Methods --
		private void SubscribeEventListeners()
		{
			// Board saving.
			EventManager.SubscribeEventListener<GeneralPositionSaveEvent>(OnGeneralPositionEvent);
			EventManager.SubscribeEventListener<PositionSaveObjectAddedEvent>(OnObjectAddedToBoard);
			EventManager.SubscribeEventListener<PositionSaveObjectRemovedEvent>(OnObjectRemovedFromBoard);

			// Life-cycle.
			EventManager.SubscribeEventListener<GameResetEvent>(OnGameReset);
		}

		private void UnsubscribeEventListeners()
		{
			// Board saving.
			EventManager.UnsubscribeEventListener<GeneralPositionSaveEvent>(OnGeneralPositionEvent);
			EventManager.UnsubscribeEventListener<PositionSaveObjectAddedEvent>(OnObjectAddedToBoard);
			EventManager.UnsubscribeEventListener<PositionSaveObjectRemovedEvent>(OnObjectRemovedFromBoard);

			// Life-cycle.
			EventManager.UnsubscribeEventListener<GameResetEvent>(OnGameReset);
		}

		/// <summary>
		/// Callback for event in which items are added to the gameboard.
		/// </summary>
		/// <param name="gameEvent">Event triggered.</param>
		private void OnObjectAddedToBoard(PositionSaveObjectAddedEvent gameEvent)
		{
			var instance = gameEvent.CraftItemInstance;
			if (instance == null)
			{
				return;
			}
			// TODO : Check on conditions here.
			if (_craftingService.IsFinalItem(instance) == false)
			{
				TrackItemPosition(instance);
			}
		}

		/// <summary>
		/// Callback for event in which items are removed from the gameboard.
		/// </summary>
		/// <param name="gameEvent">Event triggered.</param>
		private void OnObjectRemovedFromBoard(PositionSaveObjectRemovedEvent gameEvent)
		{
			StopTrackingItemPosition(gameEvent.CraftItemInstance);
		}

		/// <summary>
		/// Callback for event when saveable objects are moved.
		/// </summary>
		/// <param name="gameEvent">Event triggered.</param>
		private void OnGeneralPositionEvent(GeneralPositionSaveEvent gameEvent)
		{
			_boardSaveState.Save();
		}

		/// <summary>
		/// Callback for event in which game is reset.
		/// </summary>
		/// <param name="gameEvent">Event triggered.</param>
		private void OnGameReset(GameResetEvent gameEvent)
		{
			_boardSaveState.Reset();

			// Reset the craft item, and craft recipe save data.
			var craftItemEntites = _craftingService.CraftItemEntities;
			var craftRecipeEntities = _craftingService.CraftRecipeEntities;

			foreach (var entity in craftItemEntites)
			{
				entity.Reset();
				entity.EnsureStartingValues();
			}

			foreach (var entity in craftRecipeEntities)
			{
				entity.Reset();
			}
		}

		/// <summary>
		/// Inform the board to start tracking a CraftItemInstance.
		/// </summary>
		/// <param name="instance">Instance to be tracked.</param>
		private void TrackItemPosition(CraftItemInstance instance)
		{
			if (instance == null || _boardSaveState == null)
			{
				return;
			}
			_boardSaveState.TrackElement(instance);
		}

		/// <summary>
		/// Inform the board to stop tracking a CraftItemInstance.
		/// </summary>
		/// <param name="instance">Instance to not longer track.</param>
		private void StopTrackingItemPosition(CraftItemInstance instance)
		{
			if (instance == null || _boardSaveState == null)
			{
				return;
			}
			_boardSaveState.UntrackElement(instance);
		}

		/// <summary>
		/// Load save data.
		/// </summary>
		private void LoadSaveData()
		{
			_boardSaveState = new BoardSaveInfo(GameConstants.BoardSaveId);
			_boardSaveState.Load();
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();

			_craftingService = CraftingSystemCraftingService.Instance;

			LoadSaveData();
			SubscribeEventListeners();
		}

		public override void Cleanup()
		{
			base.Cleanup();
			UnsubscribeEventListeners();
		}
		#endregion
	}
}
