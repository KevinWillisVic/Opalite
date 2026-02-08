using UnityEngine;

namespace FishAndChips
{
	/// <summary>
	/// Custom yield instruction that waits for mouse to be clicked.
	/// </summary>
	public class WaitForMouseDown : CustomYieldInstruction
    {
		#region -- Properties --
		public override bool keepWaiting
		{
			get
			{
				return Input.GetMouseButtonDown(0) == false;
			}
		}
		#endregion

		#region -- Constructor --
		public WaitForMouseDown()
		{
		}
		#endregion
	}
}
