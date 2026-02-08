using System;
using System.Collections.Generic;

namespace FishAndChips
{
	public delegate void EventHandler<in T>(T eventTrigger) where T : class, IEvent;

	public static class EventManager
    {
		#region -- Private Member Vars --
		private static Dictionary<Type, Delegate> _globalDelegates = new();
		#endregion

		#region -- Constructor --
		static EventManager()
		{
		}
		#endregion

		#region -- Private Methods --
		private static void AddGlobal<T>(Type key, EventHandler<T> listener) where T : class, IEvent
		{
			if (_globalDelegates.ContainsKey(key))
			{
				_globalDelegates[key] = (_globalDelegates[key] as EventHandler<T>) + listener;
			}
			else
			{
				_globalDelegates.Add(key, listener);
			}
		}

		private static void RemoveGlobal<T>(Type key, EventHandler<T> listener) where T : class, IEvent
		{
			if (_globalDelegates.ContainsKey(key))
			{
				_globalDelegates[key] = (_globalDelegates[key] as EventHandler<T>) - listener;
			}
		}
		#endregion

		#region -- Public Methods --
		public static void ClearAllEvents()
		{
			_globalDelegates.Clear();
		}

		public static void TriggerEvent<T>(T eventTrigger) where T : class, IEvent
		{
			FireEvent(new EventInvoker<T>(eventTrigger));
		}

		public static void FireEvent(IEventInvoker eventInvoker)
		{
			eventInvoker.Fire(_globalDelegates);
		}

		public static Action SubscribeEventListener<T>(EventHandler<T> listener) where T : class, IEvent
		{
			var key = typeof(T);
			AddGlobal(key, listener);
			return () => { UnsubscribeEventListener(listener); };
		}

		public static void UnsubscribeEventListener<T>(EventHandler<T> listener) where T : class, IEvent
		{
			var key = typeof(T);
			RemoveGlobal(key, listener);
		}
		#endregion
	}
}
