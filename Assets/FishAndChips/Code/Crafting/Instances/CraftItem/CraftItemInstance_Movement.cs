using UnityEngine.InputSystem;

namespace FishAndChips
{
	/// <summary>
	/// Handle movement logic for CraftItemInstance.
	/// </summary>
    public partial class CraftItemInstance
    {
		#region -- Private Methods --
		/// <summary>
		/// Move the CraftItemInstance.
		/// </summary>
		private void HandleMovement()
		{
			if (ShouldMove() == false)
			{
				return;
			}
			transform.position = Mouse.current.position.value;
		}
		#endregion

		#region -- Protected Methods --
		/// <summary>
		/// Determine if the instance allows movement.
		/// </summary>
		/// <returns>True if should move, false otherwise.</returns>
		protected virtual bool ShouldMove()
		{
			if (_craftingService.IsFinalItem(CraftItemEntity) == true
				|| _craftingService.IsDepletedItem(CraftItemEntity) == true)
			{
				return false;
			}
			return true;
		}
		#endregion
	}
}
