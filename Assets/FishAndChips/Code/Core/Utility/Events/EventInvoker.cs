using UnityEngine;
using System;
using System.Collections.Generic;

namespace FishAndChips
{
    public class EventInvoker<T> : IEventInvoker where T : class, IEvent
	{
		#region -- Properties --
		public IEvent EventObject => _eventObject;
		#endregion

		#region -- Private Member Vars --
		private readonly T _eventObject;
		#endregion

		#region -- Constructor --
		public EventInvoker(T eventObject)
		{
			_eventObject = eventObject;
		}
		#endregion

		#region -- Public Methods --
		public void Fire(Dictionary<Type, Delegate> delegates)
		{
			var dispatchAs = EventObject.DispatchAs;
			var length = dispatchAs.Length;
			for (var i = 0; i < length; i++)
			{
				var key = dispatchAs[i];
				if (delegates.TryGetValue(key, out var dispatchable) == false)
				{
					continue;
				}

				try
				{
					(dispatchable as EventHandler<T>)?.Invoke(_eventObject);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
 		#endregion
	}
}
