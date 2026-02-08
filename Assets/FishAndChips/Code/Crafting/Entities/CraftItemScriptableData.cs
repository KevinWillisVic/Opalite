using UnityEditor;
using UnityEngine;

namespace FishAndChips
{
	/// <summary>
	/// ScriptableObject representation of CraftItem.
	/// </summary>
	[CreateAssetMenu(menuName = "FishAndChips/Metadata/CraftItem")]
	public class CraftItemScriptableData : GameScriptableObject, IMetadataAsset<CraftItemData>
    {
		#region -- Properties --
		public string ID => _data.ID;
		public new CraftItemData Data => _data;
		#endregion

		#region -- Inspector --
		[SerializeField] protected CraftItemData _data;
		#endregion

		#region -- Public Methods --
#if UNITY_EDITOR
		public void CreateModelData()
		{
			CraftItemModelData modelData = CreateInstance<CraftItemModelData>();
			string assetpath = EditorUtility.SaveFilePanelInProject(
				"New CraftItemModelData",
				"CraftItemModelData",
				"asset",
				"Specify new asset name"
				);

			if (assetpath != "")
			{
				AssetDatabase.CreateAsset(modelData, assetpath);
				AssetDatabase.Refresh();
				AssetDatabase.SaveAssets();
			}
		}

		public CraftItemModelData CreateModelDataAtBuiltInPath()
		{
			CraftItemModelData modelData = CreateInstance<CraftItemModelData>();
			string assetName = $"{name}ModelData.asset";
			string assetPath = $"Assets/FishAndChips/Data/Crafting/CraftItemsModelData/{assetName}";
			modelData.DisplayName = string.Empty;
			modelData.VisualKey = string.Empty;
			modelData.Blurb = string.Empty;
			AssetDatabase.CreateAsset(modelData, assetPath);
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
			return modelData;
		}

		public CraftItemModelData CreateModelDataAtBuiltInPath(string itemName)
		{
			CraftItemModelData modelData = CreateInstance<CraftItemModelData>();
			string assetName = $"{itemName}.asset";
			string assetPath = $"Assets/FishAndChips/Data/Crafting/CraftItemsModelData/{assetName}";
			modelData.DisplayName = string.Empty;
			modelData.VisualKey = string.Empty;
			modelData.Blurb = string.Empty;
			AssetDatabase.CreateAsset(modelData, assetPath);
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
			return modelData;
		}
#endif
		#endregion
	}
}
