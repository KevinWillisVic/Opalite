namespace FishAndChips
{
	/// <summary>
	/// Event for filtering CraftItems.
	/// </summary>
	public class CraftItemSearchEvent : GameEvent
	{
		public string SearchFilter = string.Empty;

		public CraftItemSearchEvent(string searchFilter)
		{
			SearchFilter = searchFilter;
			DispatchAs = new[] { typeof(CraftItemSearchEvent), typeof(GameEvent) };
		}
	}

	/// <summary>
	/// Event that a CraftItemInstance was selected and dragged by the user.
	/// </summary>
	public class CraftItemSelectionEvent : GameEvent
	{
		public CraftItemInstance CraftItemInstance;

		public CraftItemSelectionEvent(CraftItemInstance instance)
		{
			CraftItemInstance = instance;
			DispatchAs = new[] { typeof(CraftItemSelectionEvent), typeof(GameEvent) };
		}
	}

	/// <summary>
	/// Event that a CraftItemInstance was released from the user dragging it.
	/// </summary>
	public class CraftItemReleasedEvent : GameEvent
	{
		public CraftItemInstance CraftItemInstance;

		public CraftItemReleasedEvent(CraftItemInstance instance)
		{
			CraftItemInstance = instance;
			DispatchAs = new[] { typeof(CraftItemReleasedEvent), typeof(GameEvent) };
		}
	}

	/// <summary>
	/// Event indicating that a CraftItem was built for the first time.
	/// </summary>
	public class CraftItemEntityUnlockEvent : GameEvent
	{
		public CraftItemEntity CraftItemEntity;

		public CraftItemEntityUnlockEvent(CraftItemEntity entity)
		{
			CraftItemEntity = entity;
			DispatchAs = new[] { typeof(CraftItemEntityUnlockEvent), typeof(GameEvent) };
		}
	}

	/// <summary>
	/// Event indicating that a CraftRecipe was built for the first time.
	/// </summary>
	public class CraftRecipeUnlockEvent : GameEvent
	{
		public CraftRecipeEntity CraftRecipeEntity;

		public CraftRecipeUnlockEvent(CraftRecipeEntity entity)
		{
			CraftRecipeEntity = entity;
			DispatchAs = new[] { typeof(CraftRecipeUnlockEvent), typeof(GameEvent) };
		}
	}

	/// <summary>
	/// Event indicating that the pooling system can initialize.
	/// </summary>
	public class PoolPopulationReady : GameEvent
	{
		public PoolPopulationReady()
		{
			DispatchAs = new[] { typeof(PoolPopulationReady), typeof(GameEvent) };
		}
	}

	/// <summary>
	/// Event indicating that the gameplayscene is playable.
	/// </summary>
	public class OnGameSceneReady : GameEvent
	{
		public OnGameSceneReady()
		{
			DispatchAs = new[] { typeof(OnGameSceneReady), typeof(GameEvent) };
		}
	}

	/// <summary>
	/// Event for pressing the recycle / undo button.
	/// </summary>
	public class RecycleTriggerableEvent : GameEvent
	{
		public RecycleTriggerableEvent()
		{
			DispatchAs = new[] { typeof(RecycleTriggerableEvent) };
		}
	}

	/// <summary>
	/// Event related to the current state of the game board.
	/// </summary>
	public class RecycleStateUpdateEvent : GameEvent
	{
		public GameplayBoard.eRecycleState RecycleState = GameplayBoard.eRecycleState.CleanState;

		public RecycleStateUpdateEvent(GameplayBoard.eRecycleState state)
		{
			RecycleState = state;
			DispatchAs = new[] { typeof(RecycleStateUpdateEvent) };
		}
	}

	/// <summary>
	/// Event related to toast messages in the game.
	/// </summary>
	public class ToastEvent : GameEvent
	{
		public string Message;

		public ToastEvent(string message)
		{
			Message = message;
			DispatchAs = new [] { typeof(ToastEvent) };
		}
	}
}
