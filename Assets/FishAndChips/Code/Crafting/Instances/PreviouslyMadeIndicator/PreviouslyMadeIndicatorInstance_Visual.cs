using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
	/// <summary>
	/// Handle the visuals of the PreviouslyMadeIndicatorInstance.
	/// </summary>
    public partial class PreviouslyMadeIndicatorInstance
    {
		#region -- Inspector --
		[Header("Visuals")]
		public Image ImageVisual;
		public SpriteRenderer SpriteRendererVisual;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemImageService _imageService;
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Set visuals of the PreviouslyMadeIndicatorInstance.
		/// </summary>
		/// <param name="key">Id of visual.</param>
		public void SetVisual(string key)
		{
			if (key.IsNullOrEmpty() == true)
			{
				return;
			}
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
		/// Set visuals of the PreviuslyMadeIndicatorInstance.
		/// </summary>
		/// <param name="entity">CraftItemEntity to base the visual off of.</param>
		public void SetVisual(CraftItemEntity entity)
		{
			if (entity == null)
			{
				Logger.LogError("PreviouslyMadeIndicatorInstance_Visual.SetVisual : Entity was null.");
				return;
			}
			SetVisual(entity.CraftItemData.CraftItemModelData.VisualKey);
		}

		/// <summary>
		/// Set visuals of the PreviuslyMadeIndicatorInstance using the CraftItemEntity property.
		/// </summary>
		public void SetVisual()
		{
			if (CraftItemEntity == null)
			{
				Logger.LogError("PreviouslyMadeIndicatorInstance_Visual.SetVisual : CraftItemEntity was null.");
				return;
			}
			SetVisual(CraftItemEntity);
		}
		#endregion
	}
}
