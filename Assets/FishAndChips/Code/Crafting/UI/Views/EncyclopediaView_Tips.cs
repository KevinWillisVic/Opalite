using System.Linq;
using UnityEngine;

namespace FishAndChips
{
    public partial class EncyclopediaView 
    {
		#region -- Inspector --
		[Header("Tip View")]
		public GameTipComponentList GameTipComponentList;
		public GameTipComponentListItem SingleTipListItem;

		public GameObjectStateMachine TipUIStateMachine;
		#endregion

		#region -- Private Methods --
		private void DisplayTipsView()
		{
			DisplayTipList();

			if (GameTipComponentList != null)
			{
				var tips = _dataService.TipDatabase.GetAllObjects().Where(t => !t.IsDefaultTip).ToList();
				GameTipComponentList.FillList(tips);

				GameTipComponentList.OnListItemSelected -= OnSelectedTip;
				GameTipComponentList.OnListItemSelected += OnSelectedTip;
			}
		}
		#endregion

		#region -- Public Methods --
		public void TriggerShowTipView()
		{
			_currentViewMode = eEncyclopediaViewMode.Tips;
			DisplayCurrentUIView();
		}

		public void DisplayTipList()
		{
			TipUIStateMachine.SetState("TipList");
		}

		public void OnSelectedTip(GameTipComponentListItem item)
		{
			DisplayTipInformation(item.TipData);
		}

		public void DisplayTipInformation(GameTipData data)
		{
			if (data == null)
			{
				return;
			}
			TipUIStateMachine.SetState("SelectedTip");
			if (SingleTipListItem != null)
			{
				SingleTipListItem.ListObject = data;
				SingleTipListItem.Initialize();
			}
		}

		public void TriggerShowTipsView()
		{
			_currentViewMode = eEncyclopediaViewMode.Tips;
			DisplayCurrentUIView();
		}
		#endregion
	}
}
