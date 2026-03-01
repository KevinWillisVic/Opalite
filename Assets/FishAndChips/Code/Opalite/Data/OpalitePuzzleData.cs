#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FishAndChips
{
    public class OpalitePuzzleData : ScriptableObjectData
    {
#if UNITY_EDITOR
		[MenuItem("Assets/Create/FishAndChips/GameData/PuzzleData")]
		public static void CreateAsset()
		{
			var data = ScriptableObjectUtility.CreateAsset<OpalitePuzzleData>("Assets/FishAndChips/Data/Opalite/ScriptableData/PuzzleData");
		}
#endif
	}
}
