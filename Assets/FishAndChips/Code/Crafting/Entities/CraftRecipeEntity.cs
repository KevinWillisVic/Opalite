namespace FishAndChips
{
	/// <summary>
	/// Entity for CraftRecipe.
	/// </summary>
    public class CraftRecipeEntity : UnlockableEntity
    {
		#region -- Properties --
		public CraftRecipeData CraftRecipeData => _craftRecipeData;
		public CraftRecipeSavedData CraftRecipeSavedData => _craftRecipeSavedData;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Member Vars --
		private CraftRecipeData _craftRecipeData;
		private CraftRecipeSavedData _craftRecipeSavedData;
		#endregion

		#region -- Constructors --
		public CraftRecipeEntity(string instanceId) : base(instanceId)
		{
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Initialize entity.
		/// </summary>
		public override void Initialize()
		{
			_craftingService = CraftingSystemCraftingService.Instance;

			_craftRecipeData = Data as CraftRecipeData;
			_craftRecipeSavedData = SavedData as CraftRecipeSavedData;
		}

		/// <summary>
		/// Cleanup entity.
		/// </summary>
		public override void Cleanup()
		{
			_craftRecipeData = null;
			_craftRecipeSavedData = null;
		}

		/// <summary>
		/// Handle save state on game reset / object reset.
		/// </summary>
		public override void Reset()
		{
			_craftRecipeSavedData.Reset();
			base.Reset();
		}

		/// <summary>
		/// Are all of the ingredients of this recipe unlocked and thus the recipe is buildable.
		/// </summary>
		/// <returns>True if it is possible to craft the recipe, false otherwise.</returns>
		public bool CanBuildRecipe()
		{
			foreach (var ingredient in _craftRecipeData.IngredientMap)
			{
				var entity = _craftingService.FetchCraftItemEntity(ingredient.IngredientKey);
				if (entity.Unlocked == false)
				{
					return false;
				}
			}
			return true;
		}
		#endregion
	}
}
