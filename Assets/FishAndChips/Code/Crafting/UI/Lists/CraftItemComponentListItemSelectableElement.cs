using UnityEngine;
using UnityEngine.InputSystem;

namespace FishAndChips
{
    public class CraftItemComponentListItemSelectableElement : CraftItemComponentListItem
    {
		#region -- Public Methods --
		public override void Initialize()
		{
			_craftingService = CraftingSystemCraftingService.Instance;
			base.Initialize();
		}

		public override void OnPointerExit()
		{
			if (Entity == null || _isPointerDown == false)
			{
				return;
			}

			Vector3 position = Vector3.zero;
			position = Mouse.current.position.ReadValue();

			// Spawn a new CraftItem instance.
			var newInstance = _craftingService.SpawnAndReturnCraftItemInstance(Entity,
				position,
				triggerSaveEvent: true,
				spawnAnimation: CraftItemInstance.eCraftItemAnimationKeys.SpawnFromScrollRect.ToString());

			newInstance.OnSelected();
			newInstance.SetActiveSafe(true);
			base.OnPointerExit();
		}
		#endregion
	}
}
