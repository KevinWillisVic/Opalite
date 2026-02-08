using UnityEngine;
using TMPro;

namespace FishAndChips
{
	/// <summary>
	/// Wrapper class for keywords.
	/// </summary>
	public class PackagedKeyword
	{
		public eCraftItemKeyword Keyword;
	}

    public class KeywordComponentListItem : ComponentListItem
    {
		#region -- Properties --
		public PackagedKeyword Keyword => _keyword;
		#endregion

		#region -- Inspector --
		public TextMeshProUGUI KeywordName;
		public GameObject ActiveContainer;
		#endregion

		#region -- Private Member Vars --
		private PackagedKeyword _keyword;
		#endregion

		#region -- Private Methods --
		private void SetText()
		{
			KeywordName.SetTextSafe(_keyword.Keyword.ToString());
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_keyword = ListObject as PackagedKeyword;
			if (_keyword == null)
			{
				return;
			}
			SetText();
		}

		public void SetActiveContainerState(bool state)
		{
			ActiveContainer.SetActiveSafe(state);
		}
		#endregion
	}
}
