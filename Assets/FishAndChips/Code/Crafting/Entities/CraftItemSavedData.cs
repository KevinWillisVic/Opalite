using System;

namespace FishAndChips
{
	/// <summary>
	/// Save data for CraftItem.
	/// </summary>
	[Serializable]
	public class CraftItemSavedData : UnlockableEntitySavedData
	{
		#region -- Constructors --
		public CraftItemSavedData(string saveId) : base(saveId)
		{
			// Implement any custom save variables / logic here.
		}
		#endregion
	}
}
