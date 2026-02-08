using UnityEngine;
using System;
using System.Collections.Generic;

namespace FishAndChips
{
    public partial class EncyclopediaView : GameView
    {
		#region -- Supporting --
		public enum eEncyclopediaViewMode
		{
			None = -1,
			Home = 0,
			Items = 1,
			Stats= 2,
			Tips = 3
		}
		#endregion

		#region -- Inspector --
		[Header("Core")]
		public List<GameObject> DisplayRoots = new();
		public CollectionGroup ButtonCollection;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemImageService _imageService;
		protected CraftingSystemStatService _statService;
		protected CraftingSystemDataService _dataService;
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Member Vars --
		private eEncyclopediaViewMode _currentViewMode = eEncyclopediaViewMode.Home;
		#endregion

		#region -- Private Methods --
		private void SetButtonListeners()
		{
			if (ButtonCollection == null)
			{
				return;
			}
			ButtonCollection.OnButtonSelected -= OnButtonCategorySelected;
			ButtonCollection.OnButtonSelected += OnButtonCategorySelected;
		}
		
		private void DisplayCurrentUIView()
		{
			foreach (var root in DisplayRoots)
			{
				root.SetActiveSafe(false);
			}

			int index = (int)_currentViewMode;
			if (index >= 0 && index < DisplayRoots.Count)
			{
				DisplayRoots[index].SetActiveSafe(true);
			}

			switch (_currentViewMode)
			{
				case eEncyclopediaViewMode.Home:
					DisplayHomeView();
					break;
				case eEncyclopediaViewMode.Items:
					DisplayItemsView();
					break;
				case eEncyclopediaViewMode.Stats:
					DisplayStatsView();
					break;
				case eEncyclopediaViewMode.Tips:
					DisplayTipsView();
					break;
				default:
					break;
			}
		}

		private void OnButtonCategorySelected(CollectionGroupButton button)
		{
			if (button == null)
			{
				return;
			}
			var previousViewMode = _currentViewMode;
			_currentViewMode = eEncyclopediaViewMode.None;
			if (Enum.TryParse(button.ButtonName, out eEncyclopediaViewMode mode) == false)
			{
				return;
			}
			_currentViewMode = mode;
			if (previousViewMode != _currentViewMode)
			{
				DisplayCurrentUIView();
			}
		}
		#endregion

		#region -- Protected Methods --
		protected override void SubscribeListeners()
		{
			base.SubscribeListeners();
			EventManager.SubscribeEventListener<CraftItemSearchEvent>(OnSearchRaised);
		}

		protected override void UnsubsribeListeners()
		{
			base.UnsubsribeListeners();
			EventManager.UnsubscribeEventListener<CraftItemSearchEvent>(OnSearchRaised);
		}
		#endregion

		#region -- Public Methods --
		public override void Activate()
		{
			_imageService = CraftingSystemImageService.Instance as CraftingSystemImageService;
			_statService = CraftingSystemStatService.Instance;
			_dataService = CraftingSystemDataService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;

			_currentViewMode = eEncyclopediaViewMode.None;

			SetButtonListeners();
			if (ButtonCollection != null)
			{
				ButtonCollection.ForceSelectToggle("Home");
			}

			base.Activate();
		}

		public override void Deactivate()
		{
			if (ButtonCollection != null)
			{
				ButtonCollection.OnButtonSelected -= OnButtonCategorySelected;
			}
			ClearActiveSearches();
			base.Deactivate();
		}

		public override void Initialize()
		{
			base.Initialize();
			if (ButtonCollection != null)
			{
				ButtonCollection.Initialize();
			}
			SetButtonListeners();
		}

		public void LeaveEncyclopediaView()
		{
			_uiService.ActivateView(UIEnumTypes.eViewType.GameplaySceneView.ToString());
		}
		#endregion
	}
}
