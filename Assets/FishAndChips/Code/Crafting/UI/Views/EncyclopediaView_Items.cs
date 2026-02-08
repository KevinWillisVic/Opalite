using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
    public partial class EncyclopediaView
    {
		#region -- Inspector --
		[Header("Item View")]
		public ScrollRect ItemScrollRect;
		public CraftItemComponentList CraftItemComponentList;
		public CraftItemComponentListItem SingleCraftItemListItem;
		public KeywordComponentList AllKeywordList;
		public GameObject CategoryList;

		public GameObjectStateMachine ItemViewStateMachine;
		#endregion

		#region -- Private Member Vars --
		private List<CraftItemEntity> _craftItemEntities = new();
		#endregion

		#region -- Private Methods --
		private void DisplayItemsView()
		{
			ClearActiveSearches();
			if (ItemScrollRect != null)
			{
				ItemScrollRect.verticalNormalizedPosition = 1;
			}
			DisplayCraftItemList();
			SetupCraftItemScrollRect();
		}

		private void OnCraftItemSelected(CraftItemComponentListItem item)
		{
			DisplayCraftItemInformation(item.Entity);
		}

		private void DisplayCraftItemInformation(CraftItemEntity entity)
		{
			if (entity == null)
			{
				return;
			}

			ItemViewStateMachine.SetState("Single");
			if (SingleCraftItemListItem != null)
			{
				SingleCraftItemListItem.ListObject = entity;
				SingleCraftItemListItem.Initialize();
			}
		}
		#endregion

		#region -- Public Methods --
		public void SetupCraftItemScrollRect()
		{
			SetupCraftItemScrollRect(string.Empty);
		}

		public void SetupCraftItemScrollRect(string filter)
		{
			if (CraftItemComponentList == null)
			{
				return;
			}

			_lastSearch = filter;

			// Filter.
			_craftItemEntities.Clear();
			_craftItemEntities.AddRange(_craftingService.CraftItemEntities);
			_craftItemEntities.RemoveAll(c => IsValidForDisplaying(c) == false);

			CraftItemComponentList.FillList(_craftItemEntities);
			CraftItemComponentList.OnListItemSelected -= OnCraftItemSelected;
			CraftItemComponentList.OnListItemSelected += OnCraftItemSelected;
		}

		public void ItemViewRequestBack()
		{
			DisplayCraftItemList();
			SetupCraftItemScrollRect(_lastSearch);
		}

		public void DisplayCraftItemList()
		{
			ItemViewStateMachine.SetState("List");

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
		}
		#endregion
	}
}
