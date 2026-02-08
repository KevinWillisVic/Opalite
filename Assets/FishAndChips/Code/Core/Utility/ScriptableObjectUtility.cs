#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FishAndChips
{
    public class ScriptableObjectUtility
    {
		#region -- Public Methods --
		public static T CreateAsset<T>(string path = null, string fileName = null, bool saveDatabase = true) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			if (path.IsNullOrEmpty() == true)
			{
				path = AssetDatabase.GetAssetPath(Selection.activeObject);
			}

			if (path.IsNullOrEmpty())
			{
				path = "Assets/FishAndChips/Data";
			}
			
			if (Path.GetExtension(path).IsNullOrEmpty() == false)
			{
				string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
				string fileNameAtPath = Path.GetFileName(assetPath);
				path = path.Replace(fileNameAtPath, "");
			}

			if (Directory.Exists(path) == false)
			{
				Directory.CreateDirectory(path);
			}

			fileName = fileName.IsNullOrEmpty() ? $"New_{typeof(T).ToString()}" : fileName;
			string rawPathAndName = $"{path}/{fileName}.asset";
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(rawPathAndName);

			AssetDatabase.CreateAsset(asset, assetPathAndName);
			if (saveDatabase == true)
			{
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = asset;
			}
			return asset;
		}

		public static T GetAssetWithType<T>(string assetName) where T : ScriptableObject
		{
			var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name} {assetName}");
			foreach (string guid in guids)
			{
				var asset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));
				if (asset != null)
				{
					return EditorUtility.InstanceIDToObject(asset.GetInstanceID()) as T;
				}
			}
			return default(T);
		}

		public static T[] GetAssetsWithType<T>() where T : ScriptableObject
		{
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			T[] allAssetsOfType = new T[guids.Length];
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				allAssetsOfType[i] = AssetDatabase.LoadAssetAtPath<T>(path);
			}
			return allAssetsOfType;
		}
		#endregion
	}
}
#endif
