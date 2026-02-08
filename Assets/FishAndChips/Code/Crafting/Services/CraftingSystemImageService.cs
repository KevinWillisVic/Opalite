using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace FishAndChips
{
	public class CraftingSystemImageService : ImageService<CraftingSystemImageService>
	{
		
		#region -- Properties --
		/*
		public static new CraftingSystemImageService Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = (CraftingSystemImageService)Object.FindAnyObjectByType(typeof(CraftingSystemImageService));
						
						if (_instance == null)
						{
							GameObject singletonObject = new GameObject();
							_instance = singletonObject.AddComponent<CraftingSystemImageService>();
							singletonObject.name = $"Singleton {typeof(CraftingSystemImageService).ToString()}";
							DontDestroyOnLoad(singletonObject);
						}
					}
					return _instance as CraftingSystemImageService;
				}
			}
		}
		*/
		#endregion
		

		#region -- Private Member Vars --
		private SpriteAtlas _craftingAtlas;
		#endregion

		#region -- Public Methods --
		public override async Task LoadSpriteAtlases()
		{
			await base.LoadSpriteAtlases();
			await Task.WhenAll(
					LoadSpriteAtlasFromRresources("CraftItemIconAtlas", false)
				);

			TryGetSpriteAtlases("CraftItemIconAtlas", out _craftingAtlas);
		}

		public Sprite GetCraftImage(string imageId)
		{
			Sprite sprite = null;
			if (_craftingAtlas != null)
			{
				sprite = _craftingAtlas.GetSprite(imageId);
			}
			return sprite != null ? sprite : GetImageSafe(imageId);
		}
		#endregion
	}
}
