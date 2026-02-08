using System.Collections.Generic;

namespace FishAndChips
{
	/// <summary>
	/// Service handling hints for the game.
	/// </summary>
    public class CraftingSystemHintService : Singleton<CraftingSystemHintService>, IInitializable
    {
		#region -- Protected Member Vars --
		// Services.
		protected CraftingSystemStatService _statService;
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Get list of all CraftItems that are locked but buildable.
		/// </summary>
		/// <returns>List of all CraftItem entities that are locked but buildable.</returns>
		private List<CraftItemEntity> GetLockedButBuildableCraftItemEntityList()
		{
			List<CraftItemEntity> buildableEntities = new();
			var allEntities = _craftingService.CraftItemEntities;
			foreach (var entity in allEntities)
			{
				if (entity.Unlocked == true)
				{
					continue;
				}
				// Get recipes that make the entity (where entity is a product).
				if (_craftingService.ProductRecipes.ContainsKey(entity.InstanceId) == false)
				{
					continue;
				}
				List<CraftRecipeEntity> recipes = _craftingService.ProductRecipes[entity.InstanceId];
				foreach (var recipe in recipes)
				{
					if (recipe.CanBuildRecipe() == true)
					{
						if (buildableEntities.Contains(entity) == false)
						{
							buildableEntities.Add(entity);
							break;
						}
					}
				}
			}
			return buildableEntities;
		}

		/// <summary>
		/// Get list of all CraftRecipes that are locked but buildable.
		/// </summary>
		/// <returns> List of all CraftRecipe entites that are locked but buildable.</returns>
		private List<CraftRecipeEntity> GetLockedButBuildableCraftRecipeEntityList()
		{
			List<CraftRecipeEntity> buildableEntites = new();
			var allRecipeEntities = _craftingService.CraftRecipeEntities;
			foreach (var recipeEntity in allRecipeEntities)
			{
				if (recipeEntity.Unlocked == true)
				{
					continue;
				}
				if (recipeEntity.CanBuildRecipe() == true)
				{
					if (buildableEntites.Contains(recipeEntity) == false)
					{
						buildableEntites.Add(recipeEntity);
					}
				}
			}
			return buildableEntites;
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			// Services.
			_statService = CraftingSystemStatService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;
		}

		/// <summary>
		/// Is it still possible to display a hint in the game.
		/// </summary>
		/// <returns>True if it is possible to display a hint, false otherwise.</returns>
		public bool HasHintAvailable()
		{
			bool hasLockedCraftItemEntities = _statService.GetTotalLockedCraftItemEntities() != 0;
			bool hasLockedCraftRecipeEntities = _statService.GetTotalLockedCraftRecipeEntities() != 0;
			return hasLockedCraftItemEntities || hasLockedCraftRecipeEntities;
		}

		/// <summary>
		/// Return a CraftRecipeEntity that is valid for a hint.
		/// </summary>
		/// <returns>Return a locked CraftRecipeEntity that is buildable.</returns>
		public CraftRecipeEntity GetCraftRecipeEntityAsHint()
		{
			var allLockedButBuildableEntities = GetLockedButBuildableCraftRecipeEntityList();
			if (allLockedButBuildableEntities == null || allLockedButBuildableEntities.Count == 0)
			{
				return null;
			}
			allLockedButBuildableEntities.Shuffle();
			return allLockedButBuildableEntities[0];
		}

		/// <summary>
		/// Return a CraftItemEntity that is valid for a hint.
		/// </summary>
		/// <returns>Return a locked CraftItemEntity that is buildable.</returns>
		public CraftItemEntity GetCraftItemEntityAsHint()
		{
			var allLockedButBuildableEntities = GetLockedButBuildableCraftItemEntityList();
			if (allLockedButBuildableEntities == null || allLockedButBuildableEntities.Count == 0)
			{
				return null;
			}
			allLockedButBuildableEntities.Shuffle();
			return allLockedButBuildableEntities[0];
		}
		#endregion
	}
}
