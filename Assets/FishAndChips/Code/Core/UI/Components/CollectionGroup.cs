using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FishAndChips
{
    public class CollectionGroup : MonoBehaviour
    {
		#region -- Properties --
		public Action<CollectionGroupButton> OnButtonSelected { get; set; }
        public Action<CollectionGroupButton> OnButtonDisabled { get; set; }
		#endregion

		#region -- Inspector --
		public List<CollectionGroupButton> Buttons = new();
		public CollectionGroupButton ActiveButton;
		public bool AllowNoSelection = false;
		#endregion

		#region -- Public Methods --
		public void Initialize()
		{
			foreach (var button in Buttons)
			{
				if (button == null)
				{
					continue;
				}
				button.Initialize(this);
			}
		}

		public string GetActiveButtonName()
		{
			return ActiveButton != null ? ActiveButton.ButtonName : string.Empty;
		}

		public void ButtonPressed(CollectionGroupButton button)
		{
			if (button.Disabled)
			{
				OnButtonDisabled.FireSafe(button);
				return;
			}

			bool wasActive = ActiveButton == button;
			var unselected = (wasActive && AllowNoSelection == false) ? ActiveButton : null;
			ActiveButton = wasActive ? unselected : button;

			foreach (var b in Buttons)
			{
				b.CheckActive();
			}

			OnButtonSelected.FireSafe(ActiveButton);
		}

		public void ForceSelectToggle(string buttonString)
		{
			var button = GetButtonFromName(buttonString);

			if (button == null)
			{
				ButtonPressed(null);
				return;
			}

			button.SetPressed();
			button.OnPointerUp(null);
		}

		public void CleanUp()
		{
			ActiveButton = null;
			foreach (var button in Buttons)
			{
				button.CleanUp();
			}
		}

		public CollectionGroupButton GetButtonFromName(string buttonString)
		{
			return Buttons.FirstOrDefault(b => b.ButtonName == buttonString);
		}
		#endregion
	}
}
