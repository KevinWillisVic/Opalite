using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FishAndChips
{
    public partial class EncyclopediaView
    {
		#region -- Inspector --
		[Header("Home View")]
		// Game Completion Progress.
		public TextMeshProUGUI DiscoveredItemsText;
		// Tips.
		public GameTipComponentListItem TipComponentListItem;

		// Recently Found.
		public int MaxRecentItemsToDisplay = 8;
		public GameObject RecentlyFoundContainer;
		public CraftItemComponentList RecentlyFoundComponentList;
		#endregion

		#region -- Private Methods --
		private void DisplayHomeView()
		{
			var totalItems = _statService.GetTotalCraftItemEntities();
			var totalUnlockedItems = _statService.GetTotalUnlockedCraftItemEntities();

			DiscoveredItemsText.SetTextSafe($"{totalUnlockedItems}/{totalItems}");

			var tip = _dataService.TipDatabase.GetAllObjects().Where(t => t.IsDefaultTip).First();
			if (TipComponentListItem != null)
			{
				TipComponentListItem.ListObject = tip;
				TipComponentListItem.Initialize();
			}

			if (RecentlyFoundComponentList != null)
			{
				RecentlyFoundContainer.SetActiveSafe(true);

				var recentItems = _craftingService.CraftItemEntities.Where(i => i.Unlocked == true && i.CraftItemData.Keywords.Contains(eCraftItemKeyword.Basic) == false).ToList();
				if (recentItems != null && recentItems.Count > 0)
				{
					recentItems = recentItems.OrderByDescending(r => r.CraftItemSavedData.TimeUnlocked).ToList();
					List<CraftItemEntity> displayedItems = new();
					for (int i = 0; i < MaxRecentItemsToDisplay && i < recentItems.Count; i++)
					{
						displayedItems.Add(recentItems[i]);
					}
					RecentlyFoundComponentList.FillList(displayedItems);

					RecentlyFoundComponentList.OnListItemSelected -= OnSelectedRecentItem;
					RecentlyFoundComponentList.OnListItemSelected += OnSelectedRecentItem;
				}
				else
				{
					RecentlyFoundContainer.SetActiveSafe(false);
				}
			}
			else
			{
				RecentlyFoundContainer.SetActiveSafe(false);
			}
		}
		#endregion

		#region -- Public Methods --
		public void TriggerShowHomeView()
		{
			_currentViewMode = eEncyclopediaViewMode.Home;
			DisplayCurrentUIView();
		}

		public void OnSelectedRecentItem(CraftItemComponentListItem item)
		{
			_currentViewMode = eEncyclopediaViewMode.Items;
			DisplayCurrentUIView();
			DisplayCraftItemInformation(item.Entity);
		}
		#endregion
	}
}
