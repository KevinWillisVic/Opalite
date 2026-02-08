using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
	/// <summary>
	/// Overlay to handle yes / no feedback prompts.
	/// </summary>
    public class OverlayYesNo : GameOverlay
    {
		#region -- Inspector --
		public Action<GameOverlay> YesSelected { get; set; }
		public Action<GameOverlay> NoSelected { get; set; }
		#endregion

		#region -- Inspector --
		[Header("Buttons")]
		public Button YesButton;
		public Button NoButton;

		[Header("Text")]
		public TextMeshProUGUI YesText;
		public TextMeshProUGUI NoText;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Handle setting up button on click callbacks.
		/// </summary>
		private void SetupButtons()
		{
			if (YesButton != null)
			{
				YesButton.onClick.AddListener(UserSelectedYes);
			}

			if (NoButton != null)
			{
				NoButton.onClick.AddListener(UserSelectedNo);
			}
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();

			SetupButtons();
		}

		/// <summary>
		/// Set text for the yes and no buttons.
		/// </summary>
		/// <param name="yes">Text for yes button.</param>
		/// <param name="no">Text for no button.</param>
		public void SetButtonText(string yes, string no)
		{
			YesText.SetTextSafe(yes);
			NoText.SetTextSafe(no);
		}

		/// <summary>
		/// Set text for yes button.
		/// </summary>
		/// <param name="yes">Text for yes button.</param>
		public void SetYesButtonText(string yes)
		{
			YesText.SetTextSafe(yes);
		}

		/// <summary>
		/// Set text for no button.
		/// </summary>
		/// <param name="no">Text for no button.</param>
		public void SetNoButtonText(string no)
		{
			NoText.SetTextSafe(no);
		}

		/// <summary>
		/// Callback for user hitting the yes button.
		/// </summary>
		public virtual void UserSelectedYes()
		{
			YesSelected.FireSafe(this);
			DismissSelected();
		}

		/// <summary>
		/// Callback for user hitting the no button.
		/// </summary>
		public virtual void UserSelectedNo()
		{
			NoSelected.FireSafe(this);
			DismissSelected();
		}
		#endregion
	}
}
