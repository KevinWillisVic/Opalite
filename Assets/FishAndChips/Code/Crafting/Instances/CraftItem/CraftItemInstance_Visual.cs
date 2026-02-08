using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
	/// <summary>
	/// Handle visuals for CraftItemInstaance.
	/// </summary>
    public partial class CraftItemInstance
    {
		#region -- Inspector --
		[Header("Visuals")]
		public SpriteRenderer SpriteRendererVisual;
		public Image ImageVisual;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemImageService _imageService;
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Set visuals of the CraftItemInstance.
		/// </summary>
		/// <param name="key">Id of visual.</param>
		public void SetVisual(string key)
		{
			if (_imageService == null)
			{
				_imageService = CraftingSystemImageService.Instance;
			}

			var sprite = _imageService.GetCraftImage(key);
			if (sprite == null)
			{
				return;
			}

			SpriteRendererVisual.SetSpriteSafe(sprite);
			ImageVisual.SetSpriteSafe(sprite);
		}

		/// <summary>
		/// Set visuals of the CraftItemInstance.
		/// </summary>
		/// <param name="entity">CraftItemEntity to base the visual off of.</param>
		public void SetVisual(CraftItemEntity entity)
		{
			if (entity == null)
			{
				Logger.LogError("CraftItemInstance_Visual.SetVisual : Entity was null.");
				return;
			}
			SetVisual(entity.CraftItemData.CraftItemModelData.VisualKey);
		}

		/// <summary>
		/// Set visuals of the CraftItemInstance using the CraftItemEntity property.
		/// </summary>
		public void SetVisual()
		{
			if (CraftItemEntity == null)
			{
				Logger.LogError("CraftItemInstance_Visual.SetVisual : CraftItemEntity was null.");
				return;
			}
			SetVisual(CraftItemEntity);
		}
		#endregion
	}
}
