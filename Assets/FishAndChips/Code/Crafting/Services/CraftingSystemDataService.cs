using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FishAndChips
{
    public class CraftingSystemDataService : DataService
    {
		#region -- Properties --
		public static new CraftingSystemDataService Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = (CraftingSystemDataService)Object.FindAnyObjectByType(typeof(CraftingSystemDataService));

						if (_instance == null)
						{
							GameObject singletonObject = new GameObject();
							_instance = singletonObject.AddComponent<CraftingSystemDataService>();
							singletonObject.name = $"Singleton {typeof(CraftingSystemDataService).ToString()}";
							DontDestroyOnLoad(singletonObject);
						}
					}
					return _instance as CraftingSystemDataService;
				}
			}
		}
		#endregion

		#region -- Public Member Vars --
		// Collection of game tips.
		public ScriptableObjectDatabase<GameTipData> TipDatabase = new();
		#endregion

		#region -- Public Methods --
		public override void Cleanup()
		{
			base.Cleanup();
			TipDatabase.Clear();
		}

		public override async Task Load()
		{
			// Load collection of tips.
			BuildTipDatabase("Tips");
			await base.Load();
		}

		/// <summary>
		/// Build collection of GameTipData
		/// </summary>
		/// <param name="containerFolder">The folder the GameTipData are contained in.</param>
		public void BuildTipDatabase(string containerFolder)
		{
			// Fetch all GameTipData in Resources / Tips folder.
			var tips = BuildDB<GameTipData>(containerFolder);
			foreach (var tip in tips)
			{
				if (TipDatabase.Contains(tip.GUID) == false)
				{
					TipDatabase.AddObject(tip.GUID, tip);
				}
			}
		}

		/// <summary>
		/// Return list of specified object from specified folder.
		/// </summary>
		/// <typeparam name="T">Type of asset to return.</typeparam>
		/// <param name="containerFolder">Folder the assets would be contained in.</param>
		/// <returns>List of asset type.</returns>
		private List<T> BuildDB<T>(string containerFolder) where T : Object
		{
			var collection = Resources.LoadAll<T>(containerFolder);
			return collection.ToList();
		}
		#endregion
	}
}
