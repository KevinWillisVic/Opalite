using UnityEngine;

namespace FishAndChips
{
    public partial class EncyclopediaView
    {
		#region -- Private Member Vars --
		private string _lastSearch;
		#endregion

		#region -- Private Methods --
		private void ClearActiveSearches()
		{
			var searchComponents = FindObjectsByType<CraftItemSearch>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (var component in searchComponents)
			{
				component.ClearSearch();
			}
		}

		private void OnSearchRaised(CraftItemSearchEvent gameEvent)
		{
			if (gameObject.activeInHierarchy == false)
			{
				return;
			}
			SetupCraftItemScrollRect(gameEvent.SearchFilter);
		}

		private bool MatchesSearch(CraftItemEntity entity)
		{
			// Check keyword matching.
			if (_selectedKeywords.Count > 0)
			{
				foreach (var keyword in _selectedKeywords)
				{
					if (entity.HasKeyword(keyword) == false)
					{
						return false;
					}
				}
			}
			// Check string input.
			if (_lastSearch.IsNullOrEmpty() == true)
			{
				return true;
			}
			var modelData = entity.CraftItemData.CraftItemModelData;
			var entityName = modelData.DisplayName;

			bool nameMatches = entityName.StartsWith(_lastSearch, System.StringComparison.OrdinalIgnoreCase);
			return nameMatches;
		}


		private bool IsValidForDisplaying(CraftItemEntity entity)
		{
			if (entity == null)
			{
				return false;
			}
			if (MatchesSearch(entity) == false)
			{
				return false;
			}
			return entity.Unlocked || entity.HintGiven;
		}
		#endregion
	}
}
