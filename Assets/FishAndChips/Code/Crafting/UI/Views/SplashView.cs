using UnityEngine.SceneManagement;

namespace FishAndChips
{
    public class SplashView : GameView
    {
		#region -- Private Methods --
		private void Start()
		{
			SceneManager.LoadSceneAsync(GameConstants.BootScene, LoadSceneMode.Additive);
		}
		#endregion
	}
}
