using TMPro;
using UnityEngine;

namespace FishAndChips
{
    public class OverlayHint : GameOverlay
	{
		#region -- Inspector --
		[Header("Craft Item UI")]
		public CraftItemComponentListItem CraftItemComponentListItem;

		[Header("Hint Message")]
		public TextMeshProUGUI HintText;
		#endregion

		#region -- Public Methods --
		public void Initialize(CraftItemEntity entity)
		{
			if (entity == null)
			{
				DismissSelected();
				return;
			}

			if (CraftItemComponentListItem != null)
			{
				CraftItemComponentListItem.ListObject = entity;
				CraftItemComponentListItem.Initialize();
			}

			string hintMessage = $"{entity.ModelData.DisplayName}... \nYou can create this item with what you already have unlocked.";
			HintText.SetTextSafe(hintMessage);
		}
		#endregion
	}
}
