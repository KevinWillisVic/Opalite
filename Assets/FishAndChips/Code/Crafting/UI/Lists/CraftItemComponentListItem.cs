using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace FishAndChips
{
    public class CraftItemComponentListItem : ComponentListItem
    {
		#region -- Properties --
		public CraftItemEntity Entity => _entity;
		public CraftItemModelData ModelData => _entity.ModelData;
		#endregion

		#region -- Inspector --
		[Header("Visuals")]
		public SpriteRenderer CraftItemSpriteRendererVisual;
		public Image CraftItemImageVisual;

		[Header("Text")]
		public TextMeshProUGUI CraftItemName;
		public TextMeshProUGUI CraftItemBlurb;

		[Header("Keywords")]
		public KeywordComponentList KeywordList;

		// What makes this CraftItem.
		[Header("Combination Container")]
		public GameObject CombinationContainer;
		public CraftRecipeComponentList CombinationRecipeComponentList;

		// What this CraftItem makes.
		[Header("Make Container")]
		public GameObject MakeContainer;
		public CraftItemComponentList MakeCraftItemList;

		[Header("Copy")]
		public Button CopyButton;
		#endregion

		#region -- Protected Member Vars --
		// Services.
		protected CraftingSystemImageService _imageService;
		protected CraftingSystemCraftingService _craftingService;

		// Underlying entity.
		protected CraftItemEntity _entity;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Handle setting up button on click callbacks.
		/// </summary>
		private void SetupButtons()
		{
			if (CopyButton != null)
			{
				CopyButton.onClick.AddListener(CopyItem);
			}
		}

		/// <summary>
		/// Set up either the Image, or Sprite Renderer to show the visual of the CraftItem.
		/// </summary>
		private void SetVisual()
		{
			if (_entity == null 
				|| _entity.ModelData == null)
			{
				return;
			}

			if (_imageService == null)
			{
				return;
			}

			var visualKey = _entity.CraftItemData.CraftItemModelData.VisualKey;
			var sprite = _imageService.GetCraftImage(visualKey);

			if (sprite == null)
			{
				return;
			}
			
			if (CraftItemSpriteRendererVisual != null)
			{
				CraftItemSpriteRendererVisual.sprite = sprite;
			}

			if (CraftItemImageVisual != null)
			{
				CraftItemImageVisual.sprite = sprite;		
			}
		}

		/// <summary>
		/// Set text fields related to the CraftItem.
		/// </summary>
		private void SetText()
		{
			if (_entity == null
				|| _entity.ModelData == null)
			{
				return;
			}
			CraftItemName.SetTextSafe(ModelData.DisplayName);
			CraftItemBlurb.SetTextSafe(ModelData.Blurb);
		}

		/// <summary>
		/// Display keywords related to the CraftItem.
		/// </summary>
		private void SetUpKeywordList()
		{
			if (_entity == null || KeywordList == null)
			{
				return;
			}

			// Get the built in keywords for the CraftItem,
			// as well as the temporary keywords such as "Hint".
			var keywords = _entity.GetKeywords();

			List<PackagedKeyword> packagedKeywords = new();
			foreach (var keyword in keywords)
			{
				PackagedKeyword packaged = new PackagedKeyword();
				packaged.Keyword = keyword;
				packagedKeywords.Add(packaged);
			}
			KeywordList.FillList(packagedKeywords);
			KeywordList.SetActiveSafe(packagedKeywords.Count != 0);
		}

		/// <summary>
		/// Some items it does not make sense to have a copy button.
		/// A final item, or depleted item should not be copied.
		/// </summary>
		private void DetermineCopyButtonState()
		{
			bool display = true;
			if (_entity.Unlocked == false)
			{
				display = false;
			}
			if (_craftingService.IsFinalItem(_entity)
				|| _craftingService.IsDepletedItem(_entity))
			{
				display = false;
			}
			CopyButton.SetActiveSafe(display);
		}

		/// <summary>
		/// Items in which this item can create.
		/// CraftItem is an ingredient, display what it can make (products).
		/// </summary>
		private void DisplayMakeItems()
		{
			if (MakeContainer == null)
			{
				return;
			}
			MakeContainer.SetActiveSafe(false);

			if (MakeCraftItemList == null)
			{
				return;
			}

			_craftingService.IngredientRecipes.TryGetValue(_entity.InstanceId, out var recipeList);
			if (recipeList == null || recipeList.Count == 0)
			{
				return;
			}
			List<CraftRecipeEntity> unlockedRecipes = recipeList.Where(r => r.Unlocked).ToList();
			if (unlockedRecipes.Count == 0)
			{
				return;
			}
			MakeContainer.SetActiveSafe(true);
			List<string> ids = new();
			foreach (var recipe in unlockedRecipes)
			{
				foreach (var product in recipe.CraftRecipeData.RecipeProducts)
				{
					if (ids.Contains(product) == false)
					{
						ids.Add(product);
					}
				}
			}

			List<CraftItemEntity> craftItems = new();
			foreach (var id in ids)
			{
				var entity = _craftingService.FetchCraftItemEntity(id);
				craftItems.Add(entity);
			}

			MakeCraftItemList.FillList(craftItems);

			// TODO : Handle blurb on unfound make statements.
		}

		/// <summary>
		/// Items that create this item. 
		/// This CraftItem is a product, display the CraftRecipes that make it (ingredients).
		/// </summary>
		private void DisplayCombinationItems()
		{
			CombinationContainer.SetActiveSafe(false);
			if (CombinationRecipeComponentList == null)
			{
				return;
			}

			_craftingService.ProductRecipes.TryGetValue(_entity.InstanceId, out var recipeList);
			if (recipeList == null || recipeList.Count == 0)
			{
				return;
			}
			List<CraftRecipeEntity> unlockedRecipes = recipeList.Where(r => r.Unlocked).ToList();
			if (unlockedRecipes.Count == 0)
			{
				return;
			}
			CombinationContainer.SetActiveSafe(true);
			CombinationRecipeComponentList.FillList(unlockedRecipes);

			// TODO : Handle blurb of unfound combinations.
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_entity = ListObject as CraftItemEntity;
			if (_entity == null)
			{
				return;
			}

			// Gather relevant services.
			_imageService = CraftingSystemImageService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;

			SetupButtons();
			SetVisual();
			SetText();
			SetUpKeywordList();
			DetermineCopyButtonState();

			// What this CraftItem makes.
			DisplayMakeItems();
			// What makes this CraftItem.
			DisplayCombinationItems();
		}

		/// <summary>
		/// Create an instance of the CraftItem and add it to the game board.
		/// </summary>
		public void CopyItem()
		{
			if (_craftingService == null || _entity == null)
			{
				return;
			}
			var instance = _craftingService.SpawnAndReturnCraftItemInstance(_entity, Vector3.zero, true);
		}
		#endregion
	}
}
