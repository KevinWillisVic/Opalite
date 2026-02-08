namespace FishAndChips
{
	/// <summary>
	/// Event to trigger a save related to positions.
	/// </summary>
	public class GeneralPositionSaveEvent : SaveEvent
	{
		public GeneralPositionSaveEvent()
		{
			DispatchAs = new[] { typeof(GeneralPositionSaveEvent), typeof(SaveEvent) };
		}
	}

	/// <summary>
	/// Event to indicate a CraftItemInstance should start being tracked.
	/// </summary>
	public class PositionSaveObjectAddedEvent : SaveEvent
	{
		public CraftItemInstance CraftItemInstance;

		public PositionSaveObjectAddedEvent(CraftItemInstance craftItemInstance)
		{
			CraftItemInstance = craftItemInstance;
			DispatchAs = new[] { typeof(PositionSaveObjectAddedEvent), typeof(SaveEvent) };
		}
	}

	/// <summary>
	/// Event to indicate a CraftItemInstance should no longer be tracked.
	/// </summary>
	public class PositionSaveObjectRemovedEvent : SaveEvent
	{
		public CraftItemInstance CraftItemInstance;
		public bool RePoolImmediate;
		public float WaitTimeBeforeRepool;

		public PositionSaveObjectRemovedEvent(CraftItemInstance craftItemInstance, bool immediate = false, float waitTimeBeforeRepool = 1)
		{
			CraftItemInstance = craftItemInstance;
			RePoolImmediate = immediate;
			WaitTimeBeforeRepool = waitTimeBeforeRepool;
			DispatchAs = new[] { typeof(PositionSaveObjectRemovedEvent), typeof(SaveEvent) };
		}
	}
}
