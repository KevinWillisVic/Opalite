using System;

namespace FishAndChips
{
	public class SaveEvent : IEvent
	{
		public Type[] DispatchAs { get; internal set; }

		public SaveEvent()
		{
			DispatchAs = new[] { typeof(SaveEvent) };
		}
	}

	public class GameEvent : IEvent
	{
		public Type[] DispatchAs { get; internal set; }

		public GameEvent()
		{
			DispatchAs = new[] { typeof(GameEvent) };
		}
	}

	public class GameResetEvent : GameEvent
	{
		public GameResetEvent()
		{
			DispatchAs = new[] { typeof(GameResetEvent), typeof(GameEvent) };
		}
	}

	public class OnViewActivatedEvent : IEvent
	{
		public Type[] DispatchAs { get; internal set; }

		public string View = string.Empty;

		public OnViewActivatedEvent(string view)
		{
			View = view;
			DispatchAs = new[] { typeof(OnViewActivatedEvent)};
		}
	}

	public class OnLoadStartEvent : IEvent
	{
		public Type[] DispatchAs { get; internal set; }

		public string Message = string.Empty;
		public int MaxValue = 0;

		public OnLoadStartEvent(string message, int maxValue)
		{
			Message = message;
			MaxValue = maxValue;
			DispatchAs = new[] { typeof(OnLoadStartEvent) };
		}
	}

	public class OnLoadProgressEvent : IEvent
	{
		public Type[] DispatchAs { get; internal set; }

		public string Message = string.Empty;

		public OnLoadProgressEvent(string message)
		{
			Message = message;
			DispatchAs = new[] { typeof(OnLoadProgressEvent) };
		}
	}
}
