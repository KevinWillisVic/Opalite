using TMPro;
using UnityEngine;

namespace FishAndChips
{
    public class GameStatComponentListItem : ComponentListItem
    {

		#region -- Inspector --
		public TextMeshProUGUI StatText;
		#endregion

		#region -- Private Member Vars --
		private GameStatData _statData;
		#endregion

		#region -- Private Methods --
		private void SetText()
		{
			StatText.SetTextSafe(_statData.Text);
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_statData = ListObject as GameStatData;
			if (_statData == null)
			{
				return;
			}
			SetText();
		}

		public override void Selected()
		{
			base.Selected();
			if (_statData != null)
			{
				_statData.Callback.FireSafe();
			}
		}

		public override void SelectedFromButton()
		{
			base.SelectedFromButton();
			if (_statData != null)
			{
				//_statData.Callback.FireSafe();
			}
		}
		#endregion
	}
}
