namespace FishAndChips
{
	public class OpaliteResetLevelEvent : GameEvent
	{
		public bool Passed;
		public OpaliteResetLevelEvent(bool passed)
		{
			Passed = passed;
			DispatchAs = new[] { typeof(OpaliteResetLevelEvent), typeof(GameEvent) };
		}
	}
}
