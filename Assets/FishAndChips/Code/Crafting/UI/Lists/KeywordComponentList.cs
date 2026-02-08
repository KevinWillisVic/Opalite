namespace FishAndChips
{
    public class KeywordComponentList : ComponentList<KeywordComponentListItem>
    {
		#region -- Properties --
		public EncyclopediaView Encyclopedia { get; set; }
		#endregion

		#region -- Protected Methods --
		protected override void PreInitializeItem(KeywordComponentListItem item)
		{
			base.PreInitializeItem(item);
			if (Encyclopedia != null)
			{
				var keyword = item.ListObject as PackagedKeyword;
				if (keyword == null)
				{
					return;
				}
				item.SetActiveContainerState(Encyclopedia.SelectedKeywords.Contains(keyword.Keyword));
			}
		}

		protected override void OnListItemSelectedInternal(ComponentListItem selectedItem)
		{
			base.OnListItemSelectedInternal(selectedItem);
			if (Encyclopedia != null)
			{
				KeywordComponentListItem item = selectedItem as KeywordComponentListItem;
				var keyword = item.Keyword.Keyword;
				Encyclopedia.ToggleInclusionOfKeyword(keyword);
				item.SetActiveContainerState(Encyclopedia.SelectedKeywords.Contains(keyword));
			}
		}
		#endregion
	}
}
