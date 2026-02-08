using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FishAndChips
{
    public partial class EncyclopediaView
    {
		#region -- Inspector --
		[Header("Stats View")]
		public GameStatComponentList GameStatComponentList;
		#endregion

		#region -- Private Methods --
		private void DisplayStatsView()
		{
			if (GameStatComponentList != null)
			{
				var stats = GetGameStats();
				GameStatComponentList.FillList(stats);
			}
		}

		private List<GameStatData> GetGameStats()
		{
			List<GameStatData> dataList = new();

			// Total items.
			{
				var totalItems = _statService.GetTotalCraftItemEntities();
				var totalUnlockedItems = _statService.GetTotalUnlockedCraftItemEntities();

				var stat = new GameStatData();
				stat.Text = $"{totalUnlockedItems}/{totalItems}\nItems Discovered";
				dataList.Add(stat);
			}

			// Basic Items
			{
				var totalBasicItems = _statService.GetTotalCraftItemsWithKeyword(eCraftItemKeyword.Basic);
				var stat = new GameStatData();
				stat.Text = $"{totalBasicItems}\n Basic Items";
				stat.Callback += BasicItemStatCallback;
				dataList.Add(stat);
			}

			// Hints.
			{
				List<CraftItemEntity> hintItems = new();
				hintItems = _craftingService.CraftItemEntities.Where(i => i.HintGiven).ToList();

				if (hintItems.Count > 0)
				{
					var stat = new GameStatData();
					stat.Text = $"{hintItems.Count}\nActive Hints";
					stat.Callback += HintStatCallback;
					dataList.Add(stat);
				}
			}

			return dataList;
		}

		private void BasicItemStatCallback()
		{
			_selectedKeywords.Clear();
			_selectedKeywords.Add(eCraftItemKeyword.Basic);
			_currentViewMode = eEncyclopediaViewMode.Items;
			DisplayCurrentUIView();
		}

		private void HintStatCallback()
		{
			_selectedKeywords.Clear();
			_selectedKeywords.Add(eCraftItemKeyword.Hint);
			_currentViewMode = eEncyclopediaViewMode.Items;
			DisplayCurrentUIView();
		}
		#endregion

		#region -- Public Methods --
		public void TriggerShowStatView()
		{
			_currentViewMode = eEncyclopediaViewMode.Stats;
			DisplayCurrentUIView();
		}
		#endregion
	}
}
