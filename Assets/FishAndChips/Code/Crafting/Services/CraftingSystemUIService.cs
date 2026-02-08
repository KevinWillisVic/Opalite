using System;
using UnityEngine;

namespace FishAndChips
{
    public class CraftingSystemUIService : UIService
    {
		#region -- Properties --
		public static new CraftingSystemUIService Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = (CraftingSystemUIService)UnityEngine.Object.FindAnyObjectByType(typeof(CraftingSystemUIService));

						if (_instance == null)
						{
							GameObject singletonObject = new GameObject();
							_instance = singletonObject.AddComponent<CraftingSystemUIService>();
							singletonObject.name = $"Singleton {typeof(CraftingSystemUIService).ToString()}";
							DontDestroyOnLoad(singletonObject);
						}
					}
					return _instance as CraftingSystemUIService;
				}
			}
		}

		public bool IsGameplayViewActive => _activeViewName.IsNullOrEmpty() == false && _activeViewName == UIEnumTypes.eViewType.GameplaySceneView.ToString();
		#endregion

		#region -- Public Methods --
		public T ShowOverlay<T>(UIEnumTypes overlayType,
			string title = "",
			Action<GameOverlay> onDismissed = null,
			bool isPermanent = false,
			bool forceCreateNew = false,
			GameViewLayer.eGameViewLayer layer = GameViewLayer.eGameViewLayer.Overlay) where T : GameOverlay
		{
			return ShowOverlay<T>(overlayType.ToString(), title, onDismissed, isPermanent, forceCreateNew, layer);
		}
		#endregion
	}
}
