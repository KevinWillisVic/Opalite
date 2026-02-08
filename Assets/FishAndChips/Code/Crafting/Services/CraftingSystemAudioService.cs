using UnityEngine;

namespace FishAndChips
{
    public class CraftingSystemAudioService : AudioService
    {
		#region -- Properties --
		
		public static new CraftingSystemAudioService Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = (CraftingSystemAudioService)Object.FindAnyObjectByType(typeof(CraftingSystemAudioService));

						if (_instance == null)
						{
							GameObject singletonObject = new GameObject();
							_instance = singletonObject.AddComponent<CraftingSystemAudioService>();
							singletonObject.name = $"Singleton {typeof(CraftingSystemAudioService).ToString()}";
							DontDestroyOnLoad(singletonObject);
						}
					}
					return _instance as CraftingSystemAudioService;
				}
			}
		}
		#endregion
	}
}
