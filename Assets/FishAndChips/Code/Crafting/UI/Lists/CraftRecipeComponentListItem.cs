using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
    public class CraftRecipeComponentListItem : ComponentListItem
    {
		#region -- Properties --
		public CraftRecipeEntity Entity => _entity;
		#endregion

		#region -- Inspector --
		/*
		public SpriteRenderer FirstIngredientSpriteRendererVisual;
		public SpriteRenderer SecondIngredientSpriteRendererVisual;

		public Image FirstIngredientImageVisual;
		public Image SecondIngredientImageVisual;
		*/
		public CraftItemComponentList CraftItemComponentList;
		#endregion

		#region -- Protected Member Vars --
		//protected CraftingSystemImageService _imageService;
		protected CraftingSystemCraftingService _craftingService;

		protected CraftRecipeEntity _entity;
		#endregion

		#region -- Private Methods --
		private void SetVisual()
		{
			if (_entity == null)
			{
				return;
			}

			if (CraftItemComponentList == null)
			{
				return;
			}

			List<CraftItemEntity> craftItems = new();
			foreach (var requirement in _entity.CraftRecipeData.IngredientMap)
			{
				CraftItemEntity craftItem = _craftingService.FetchCraftItemEntity(requirement.IngredientKey);
				for (int i = 0; i < requirement.RequiredCount; i++)
				{
					craftItems.Add(craftItem);
				}
			}

			CraftItemComponentList.FillList(craftItems);

			/*
			var firstIngredientModelData = _craftingService.FetchCraftItemModelData(_entity.CraftRecipeData.IdFirstIngredient);
			var secondIngredientModelData = _craftingService.FetchCraftItemModelData(Entity.CraftRecipeData.IdSecondIngredient);

			if (firstIngredientModelData != null)
			{
				var firstIngredientVisualKey = firstIngredientModelData.VisualKey;
				var sprite = _imageService.GetCraftImage(firstIngredientVisualKey);

				if (FirstIngredientSpriteRendererVisual != null)
				{
					FirstIngredientSpriteRendererVisual.sprite = sprite;
				}

				if (FirstIngredientImageVisual != null)
				{
					FirstIngredientImageVisual.sprite = sprite;
				}
			}

			if (secondIngredientModelData != null)
			{
				var secondIngredientVisualKey = secondIngredientModelData.VisualKey;
				var sprite = _imageService.GetCraftImage(secondIngredientVisualKey);

				if (SecondIngredientSpriteRendererVisual != null)
				{	
					SecondIngredientSpriteRendererVisual.sprite = sprite;
				}

				if (SecondIngredientImageVisual != null)
				{	
					SecondIngredientImageVisual.sprite = sprite;
				}
			}
			*/
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_entity = ListObject as CraftRecipeEntity;
			if (_entity == null)
			{
				return;
			}
			//_imageService = CraftingSystemImageService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;

			SetVisual();
		}
		#endregion
	}
}
