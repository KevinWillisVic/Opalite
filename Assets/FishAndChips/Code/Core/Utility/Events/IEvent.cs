using System;

namespace FishAndChips
{
	/// <summary>
	/// The base interface for all game events using the event system.
	/// </summary>
	public interface IEvent
	{
		Type[] DispatchAs { get; }
	}
}
