using System;
using System.Collections.Generic;

namespace FishAndChips
{
    public interface IEventInvoker
    {
		IEvent EventObject { get; }

		void Fire(Dictionary<Type, Delegate> delegates);
	}
}
