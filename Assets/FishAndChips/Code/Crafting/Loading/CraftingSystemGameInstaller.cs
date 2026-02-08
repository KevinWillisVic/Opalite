using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FishAndChips
{
    public class CraftingSystemGameInstaller : GameInstaller
    {
		#region -- Inspector --
		[Tooltip("Prefab Instances To Instantiate Services.")]
		public InstallerSettings Settings;
		#endregion

		#region -- Protected Member Vars --
		protected UICanvas _uiCanvas;

		// All Services In Project.
		// Add additional services here.
		//*****************************************************
		protected EntityService _entityService;
		protected CraftingSystemUIService _uiService;
		protected NavigationService _navigationService;
		protected CraftingSystemDataService _dataService;
		protected CraftingSystemHintService _hintService;
		protected CraftingSystemStatService _statService;
		protected CraftingSystemImageService _imageService;
		protected CraftingSystemAudioService _audioService;
		protected CraftingSystemSavingService _savingService;
		protected CraftingSystemPoolingService _poolingService;
		protected CraftingSystemCraftingService _craftingService;
		protected CraftingSystemGameFlowService _gameFlowService;
		//*****************************************************
		#endregion

		#region -- Private Member Vars --
		// Views that will appear in the game.
		private readonly string[] _permanentViews =
		{
			UIEnumTypes.eViewType.GameplaySceneView.ToString(),
			UIEnumTypes.eViewType.GameplayUnlockView.ToString(),
			UIEnumTypes.eViewType.EncyclopediaView.ToString()
		};
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Run initialization on all services that need it.
		/// </summary>
		private void InitializeServices()
		{
			// Initialize any service that needs to be initialized.
			IInitializable[] initializables = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
											.OfType<IInitializable>()
											.ToArray();

			foreach (var s in initializables)
			{
				s.Initialize();
			}
		}

		/// <summary>
		/// Set up UI, further setup services, enter gameplay.
		/// </summary>
		private async Task Load(string loadingOverlayName)
		{
			// Load permanent views.
			_uiService.LoadViews(_permanentViews, false);

			// Handle service loading.
			Task[] tasks = new Task[]
			{
				_imageService.Load(),
				_dataService.Load()
			};
			await Task.WhenAll(tasks);
			// data service tasks needs to finish before crafting service can load.
			_craftingService.Load();

			// Enter gameplay scene.
			await _gameFlowService.EnterGameplayFromBoot(loadingOverlayName);
		}

		/// <summary>
		/// Load boot view.
		/// </summary>
		private async Task LoadBootView()
		{
			var bootView = _uiService.LoadBootView();
			_uiService.ActivateView(UIEnumTypesBase.eViewTypeBase.BootView.ToString());
			await bootView.WaitUntilOpen();
		}

		/// <summary>
		/// Clean up boot view.
		/// </summary>
		private async Task CleanUpBootView()
		{
			while (_uiService.ActiveView as BootView != null)
			{
				await Awaitable.EndOfFrameAsync();
			}
			_uiService.DestroyView<BootView>();
		}

		/// <summary>
		/// Unload splash scene.
		/// </summary>
		private async Task UnloadSplashScene()
		{
			// Unload spash scene.
			var activeSceneName = SceneManager.GetActiveScene().name;
			if (activeSceneName == CoreConstants.SplashSceneName)
			{
				await SceneManager.UnloadSceneAsync(CoreConstants.SplashSceneName);
			}
		}


		private void Cleanup()
		{
			ICleanable[] cleanables = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
								.OfType<ICleanable>()
								.ToArray();

			foreach (var s in cleanables)
			{
				s.Cleanup();
			}
		}
		#endregion

		#region -- Protected Methods --
		/// <summary>
		/// Set up services for gameplay.
		/// Set up initial UI.
		/// </summary>
		protected override async Task RunGameInitialization()
		{
			InitializeServices();
			await LoadBootView();

			// Show loading overlay.
			string loadingOverlayName = UIEnumTypes.eOverlayType.OverlayLoadingProgressBar.ToString();
			_uiService.ShowOverlay<LoadingOverlay>(loadingOverlayName);

			await UnloadSplashScene();
			await Load(loadingOverlayName);
			await CleanUpBootView();
		}

		/// <summary>
		/// Create services instances that do not require a prefab instance.
		/// </summary>
		protected override void CreateServices()
		{
			// Generic services.
			_entityService = EntityService.Instance;
			_navigationService = NavigationService.Instance;

			// Game specific services.
			_uiService = CraftingSystemUIService.Instance;
			_dataService = CraftingSystemDataService.Instance;
			_statService = CraftingSystemStatService.Instance;
			_hintService = CraftingSystemHintService.Instance;
			_imageService = CraftingSystemImageService.Instance;
			_audioService = CraftingSystemAudioService.Instance;
			_savingService = CraftingSystemSavingService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;
			_gameFlowService = CraftingSystemGameFlowService.Instance;
		}

		/// <summary>
		/// Create service instances from instantiated prefabs.
		/// </summary>
		protected override void CreateServicesFromPrefab()
		{
			if (Settings == null)
			{
				throw new Exception($"[CraftingSystemGameInstaller] Setup the settings before game initialization.");
			}
			// Create and setup services.
			_uiCanvas = CreateFromPrefab<UICanvas>(Settings.UICanvasPrefab, parentTransform:null, dontDestroyOnLoad:true);
			_poolingService = CreateFromPrefab<CraftingSystemPoolingService>(Settings.PoolingServicePrefab, parentTransform:null, dontDestroyOnLoad:true);
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Actually instantiate prefabs and get service from spawned object.
		/// </summary>
		/// <typeparam name="T">The type of service.</typeparam>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="parentTransform">Parent to spawn under.</param>
		/// <param name="dontDestroyOnLoad">Should the spawned object be marked dont destroy on load.</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public T CreateFromPrefab<T>(T prefab, Transform parentTransform, bool dontDestroyOnLoad) where T : Component
		{
			if (prefab == null)
			{
				throw new Exception($"[CraftingSystemGameInstaller] Prefab for services {typeof(T)} was null.");
			}

			var prefabInstance = Instantiate(prefab, parentTransform);
			if (dontDestroyOnLoad == true)
			{
				DontDestroyOnLoad(prefabInstance);
			}
			prefabInstance.name = $"[Singleton] {typeof(T)}";
			return prefabInstance.GetComponent<T>();
		}
		#endregion
	}
}
