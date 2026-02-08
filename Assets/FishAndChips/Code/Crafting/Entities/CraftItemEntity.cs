using System.Collections.Generic;

namespace FishAndChips
{
	/// <summary>
	/// Entity for CraftItem.
	/// </summary>
    public class CraftItemEntity : UnlockableEntity
    {
		#region -- Properties --
		public CraftItemData CraftItemData => _craftItemData;
		public CraftItemSavedData CraftItemSavedData => _craftItemSavedData;
		public CraftItemModelData ModelData => _craftItemData != null ? _craftItemData.CraftItemModelData : null;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Member Vars
		private CraftItemData _craftItemData;
		private CraftItemSavedData _craftItemSavedData;

		private List<eCraftItemKeyword> _tempKeywords = new();
		#endregion

		#region -- Constructors --
		public CraftItemEntity(string instanceId) : base(instanceId)
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

			_craftItemData = Data as CraftItemData;
			_craftItemSavedData = SavedData as CraftItemSavedData;
		}

		/// <summary>
		/// Cleanup entity.
		/// </summary>
		public override void Cleanup()
		{
			_craftItemData = null;
			_craftItemSavedData = null;
		}

		/// <summary>
		/// Return list of all keywords associated with CraftItem.
		/// </summary>
		/// <returns>List of keywords currently associated with the CraftItem.</returns>
		public List<eCraftItemKeyword> GetKeywords()
		{
			List<eCraftItemKeyword> keywords = new List<eCraftItemKeyword>();
			// Add on the fly keywords.
			keywords.AddRange(_tempKeywords);
			// Add built in keywords.
			keywords.AddRange(_craftItemData.Keywords);
			return keywords;
		}

		/// <summary>
		/// Is the keyword currently associated with the CraftItem.
		/// </summary>
		/// <param name="keyword">Keyword being checked.</param>
		/// <returns>True if keyword is associated with CraftItem, false otherwise.</returns>
		public bool HasKeyword(eCraftItemKeyword keyword)
		{
			return _tempKeywords.Contains(keyword) || _craftItemData.Keywords.Contains(keyword);
		}

		/// <summary>
		/// Add keyword to list of temporary keywords.
		/// </summary>
		/// <param name="keyword">Keyword being added to temporary collection.</param>
		public void ActivateTempKeyword(eCraftItemKeyword keyword)
		{
			if (_tempKeywords.Contains(keyword) == false)
			{
				_tempKeywords.Add(keyword);
			}
		}

		/// <summary>
		/// Remove keyword from list of temporary keywords.
		/// </summary>
		/// <param name="keyword">Keyword being removed from temporary collection.</param>
		public void RemoveTempKeyword(eCraftItemKeyword keyword)
		{
			if (_tempKeywords.Contains(keyword) == true)
			{
				_tempKeywords.Remove(keyword);
			}
		}

		/// <summary>
		/// Set up starting values for the entity, such as temporary on the fly keywords.
		/// </summary>
		public void EnsureStartingValues()
		{
			// Save data.
			if (_craftItemData.Keywords.Contains(eCraftItemKeyword.Basic) && _craftItemSavedData.Unlocked == false)
			{
				_craftItemSavedData.SetUnlockedState(true);
			}

			// Kwywords.
			if (_craftItemSavedData.HintGiven == true)
			{
				ActivateTempKeyword(eCraftItemKeyword.Hint);
			}

			if (_craftingService.IsDepletedItem(this) == true)
			{
				ActivateTempKeyword(eCraftItemKeyword.Depleted);
			}
		}

		/// <summary>
		/// Handle save state on game reset / object reset.
		/// </summary>
		public override void Reset()
		{
			_tempKeywords.Clear();
			_craftItemSavedData.Reset();
			base.Reset();
		}


		/// <summary>
		/// Set unlocked state of the entity.
		/// </summary>
		/// <param name="state">Unlock state of the entity.</param>
		public override void SetUnlockState(bool state)
		{
			base.SetUnlockState(state);
			// Turn off hint once unlocked.
			if (_craftItemSavedData.Unlocked == true)
			{
				SetHintState(false);
			}
		}

		/// <summary>
		/// Set hint state of the entity.
		/// </summary>
		/// <param name="state">Hint state of the entity.</param>
		public override void SetHintState(bool state)
		{
			base.SetHintState(state);
			// Handle dynamic keywords.
			if (state == true)
			{
				ActivateTempKeyword(eCraftItemKeyword.Hint);
			}
			else
			{
				RemoveTempKeyword(eCraftItemKeyword.Hint);
			}
		}
		#endregion
	}
}
