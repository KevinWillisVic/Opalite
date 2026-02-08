using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;

namespace FishAndChips
{
	/// <summary>
	/// Handle flow of the game, scene transition etc.
	/// </summary>
    public class CraftingSystemGameFlowService : Singleton<CraftingSystemGameFlowService>, IInitializable
    {
		#region -- Enumerations --
		public enum eGameScenes
		{
			GameplayScene
		}
		#endregion

		#region -- Protected Member Vars --
		protected UIService _uiService;
		protected NavigationService _navigationService;
		#endregion

		#region -- Private Member Vars --
		private IUpdateable[] _updateables = new IUpdateable[0];
		private ICleanable[] _cleanables = new ICleanable[0];
		#endregion

		#region -- Private Methods --
		private void Update()
		{
			if (_initialized == false)
			{
				return;
			}

			if (_updateables == null)
			{
				return;
			}

			foreach (var updateable in _updateables)
			{
				if (updateable != null)
				{
					updateable.HandleUpdate();
				}
			}
		}

		private async Task LoadIntoGameplayScene(string loadingOverlayName)
		{
			_navigationService.AllowBackButton = false;

			// Check on getting loading overlay.
			// Construct any loading parameters.

			// If for whatever reason the loading overlay isnt up, attempt show.
			var loadingOverlay = _uiService.GetOverlay(loadingOverlayName) as LoadingOverlay;
			if (loadingOverlay == null)
			{
				loadingOverlay = _uiService.ShowOverlay<LoadingOverlay>(loadingOverlayName);
				if (loadingOverlay != null)
				{
					await loadingOverlay.WaitUntilOpen();
				}
			}


			//	Load into gameplay scene.
			bool isAdditive = false;
			var mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
			var sceneLoadOperation = SceneManager.LoadSceneAsync(CraftingSystemGameFlowService.eGameScenes.GameplayScene.ToString(), mode);
			while (sceneLoadOperation.isDone == false)
			{
				await Awaitable.EndOfFrameAsync();
			}

			// Clear loading overlay.
			if (loadingOverlay != null)
			{
				if (loadingOverlay.UserControlledDismissal == true)
				{
					var disableInputOverlay = _uiService.GetOverlay(UIEnumTypes.eOverlayType.OverlayDisableUIInput.ToString());
					disableInputOverlay.SetActiveSafe(false);

					loadingOverlay.OnLoadComplete();
					disableInputOverlay.SetActiveSafe(true);
				}
				else
				{
					loadingOverlay.DismissSelected();
				}
			}

			_navigationService.AllowBackButton = true;

			// Check on ui navigation.
			if (_uiService.ActiveView != null)
			{
				_uiService.ActiveView.Deactivate();
			}

			GameSceneInstance gameSceneInstance = FindAnyObjectByType<GameSceneInstance>();
			await gameSceneInstance.BeginGameplay();
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_uiService = UIService.Instance;
			_navigationService = NavigationService.Instance;

			_updateables = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
											.OfType<IUpdateable>()
											.ToArray();
			_cleanables = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
											.OfType<ICleanable>()
											.ToArray();

			_initialized = true;
		}

		public void TriggerCleanup()
		{
			foreach (var cleanable in _cleanables)
			{
				if (cleanable != null)
				{
					cleanable.Cleanup();
				}
			}
		}

		public async void LoadSceneAsync(int index)
		{
			await SceneManager.LoadSceneAsync(index);
		}

		public async void LoadSceneAsync(string scene)
		{
			await SceneManager.LoadSceneAsync(scene);
		}

		public async void LoadSceneAsync(string scene, LoadSceneMode mode)
		{
			await SceneManager.LoadSceneAsync(scene, mode);
		}

		public async void LoadSceneAsync(string scene, LoadSceneParameters parameters)
		{
			await SceneManager.LoadSceneAsync(scene, parameters);
		}

		public async Task EnterGameplayFromBoot(string loadingOverlayName)
		{
			await LoadIntoGameplayScene(loadingOverlayName);
		}
		#endregion
	}
}
