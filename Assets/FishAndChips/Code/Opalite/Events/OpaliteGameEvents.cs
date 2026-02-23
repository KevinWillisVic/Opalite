using System;
using UnityEngine;

namespace FishAndChips
{
	public class OpaliteResetLevelEvent : IEvent
	{
		public Type[] DispatchAs { get; internal set; }

		public OpaliteResetLevelEvent()
		{
			DispatchAs = new[] { typeof(OpaliteResetLevelEvent), typeof(GameEvent) };
		}
	}
}
