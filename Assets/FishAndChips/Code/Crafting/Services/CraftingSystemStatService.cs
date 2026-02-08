using System.Linq;
using System.Collections.Generic;

namespace FishAndChips
{

	/// <summary>
	/// Service handling statistics for the game.
	/// </summary>
	public class CraftingSystemStatService : Singleton<CraftingSystemStatService>, IInitializable
    {
		#region -- Properties --
		public List<CraftItemEntity> CraftItemEntities => _craftingService.CraftItemEntities;
		public List<CraftRecipeEntity> CraftRecipeEntities => _craftingService.CraftRecipeEntities;
		#endregion

		#region -- Protected Member Vars --
		// Services.
		protected EntityService _entityService;
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			// Services.
			_entityService = EntityService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;
		}

		/// <summary>
		/// Get total CraftItems in project.
		/// </summary>
		/// <returns>The total number of CraftItems in project.</returns>
		public int GetTotalCraftItemEntities()
		{
			return CraftItemEntities.Count;
		}

		/// <summary>
		/// Get the total number of unlocked CraftItems in project.
		/// </summary>
		/// <returns>The total number of unlocked CraftItems in project.</returns>
		public int GetTotalUnlockedCraftItemEntities()
		{
			return CraftItemEntities.Count(c => c.Unlocked == true);
		}

		/// <summary>
		/// Get the total number of locked CraftItems in project.
		/// </summary>
		/// <returns>The total number of locked CraftItems in project.</returns>
		public int GetTotalLockedCraftItemEntities()
		{
			return CraftItemEntities.Count(c => c.Unlocked == false);
		}

		/// <summary>
		/// Get the total number of CraftItems that contain the supplied keyword.
		/// </summary>
		/// <param name="keyword">The keyword we are checking for.</param>
		/// <returns>The total number of CraftItems that contain the supplied keyword.</returns>
		public int GetTotalCraftItemsWithKeyword(eCraftItemKeyword keyword)
		{
			return CraftItemEntities.Count(c => c.CraftItemData.Keywords.Contains(keyword) == true);
		}

		/// <summary>
		/// Get the total number of CraftRecipes in the project.
		/// </summary>
		/// <returns>The total number of CraftRecipes in the project.</returns>
		public int GetTotalCraftRecipeEntities()
		{
			return CraftRecipeEntities.Count;
		}

		/// <summary>
		/// Get the total number of unlocked CraftRecipes in project.
		/// </summary>
		/// <returns>The total number of locked CraftRecipes in project.</returns>
		public int GetTotalUnlockedCraftRecipeEntities()
		{
			return CraftRecipeEntities.Count(c => c.Unlocked == true);
		}

		/// <summary>
		/// Get the total number of locked CraftRecipes in project.
		/// </summary>
		/// <returns>The total number of locked CraftRecipes in project.</returns>
		public int GetTotalLockedCraftRecipeEntities()
		{
			return CraftRecipeEntities.Count(c => c.Unlocked == false);
		}
		#endregion
	}
}
