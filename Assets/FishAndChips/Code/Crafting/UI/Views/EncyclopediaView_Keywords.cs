using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FishAndChips
{
    public partial class EncyclopediaView
    {
		#region -- Properties --
		public List<eCraftItemKeyword> SelectedKeywords => _selectedKeywords;
		#endregion

		#region -- Inspector --
		[Header("Keywords")]
		public KeywordComponentList KeywordList;
		public Transform RemoveKeywordButton;
		#endregion

		#region -- Private Member Vars --
		private List<eCraftItemKeyword> _selectedKeywords = new();
		#endregion

		#region -- Public Methods --
		public void ShowKeywordFilters()
		{
			ItemViewStateMachine.SetState("Keyword");

			if (AllKeywordList != null)
			{
				AllKeywordList.Encyclopedia = this;
				// All keyword filters.
				var values = Enum.GetValues(typeof(eCraftItemKeyword)).Cast<eCraftItemKeyword>();
				var packagedKeywords = new List<PackagedKeyword>();
				foreach (var keyword in values)
				{
					PackagedKeyword packagedKeyword = new PackagedKeyword();
					packagedKeyword.Keyword = keyword;
					packagedKeywords.Add(packagedKeyword);
				}
				AllKeywordList.FillList(packagedKeywords);
			}
		}

		public void SetUpKeywordList()
		{
			if (KeywordList == null)
			{
				return;
			}
			var packagedKeywords = new List<PackagedKeyword>();
			foreach (var keyword in _selectedKeywords)
			{
				PackagedKeyword packagedKeyword = new PackagedKeyword();
				packagedKeyword.Keyword = keyword;
				packagedKeywords.Add(packagedKeyword);
			}
			KeywordList.FillList(packagedKeywords);

			if (RemoveKeywordButton != null)
			{
				RemoveKeywordButton.SetAsLastSibling();
			}
		}

		public void RemoveKeyword()
		{
			if (_selectedKeywords.Count > 0)
			{
				_selectedKeywords.RemoveAt(_selectedKeywords.Count - 1);
			}

			// Set up keyword UI.
			if (_selectedKeywords.Count == 0)
			{
				CategoryList.SetActiveSafe(false);
			}
			else
			{
				CategoryList.SetActiveSafe(true);
				SetUpKeywordList();
			}

			if (ItemScrollRect != null)
			{
				ItemScrollRect.verticalNormalizedPosition = 1;
			}

			DisplayCraftItemList();
			SetupCraftItemScrollRect(_lastSearch);
		}

		public void ToggleInclusionOfKeyword(eCraftItemKeyword keyword)
		{
			if (_selectedKeywords.Contains(keyword) == true)
			{
				_selectedKeywords.Remove(keyword);
			}
			else
			{
				_selectedKeywords.Add(keyword);
			}
		}
		#endregion
	}
}
