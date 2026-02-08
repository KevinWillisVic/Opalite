using TMPro;

namespace FishAndChips
{
    public class GameTipComponentListItem : ComponentListItem
    {
		#region -- Properties --
		public GameTipData TipData => _tipData;
		#endregion

		#region -- Inspector --
		public TextMeshProUGUI TipTitle;
		public TextMeshProUGUI TipDescription;
		#endregion

		#region -- Private Member Vars --
		private GameTipData _tipData;
		#endregion

		#region -- Private Methods --
		private void SetText()
		{
			TipTitle.SetTextSafe(_tipData.TipTitle);
			TipDescription.SetTextSafe(_tipData.Tip);
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_tipData = ListObject as GameTipData;
			if (_tipData == null)
			{
				return;
			}
			SetText();
		}
		#endregion
	}
}
