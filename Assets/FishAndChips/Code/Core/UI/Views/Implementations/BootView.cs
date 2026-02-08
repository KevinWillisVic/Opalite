using UnityEngine;
using TMPro;

namespace FishAndChips
{
    public class BootView : GameView
    {
		#region -- Inspector --
		public GameObject SplashScreenContent;
		public TextMeshProUGUI ClientVersionText;
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
			SplashScreenContent.SetActiveSafe(true);
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();

			// Set any specific client information here. Branch, commit, etc.
			string clientVersionString = $"{Application.version}";
			ClientVersionText.SetTextSafe(clientVersionString);
		}

		public override bool AddToHistory()
		{
			return false;
		}
		#endregion
	}
}
