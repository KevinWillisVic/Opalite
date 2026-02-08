#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FishAndChips
{
    public class GameTipData : ScriptableObjectData
    {
		#region -- Inspector --
		public string TipTitle;
		public string Tip;
		public bool IsDefaultTip = false;
		#endregion

#if UNITY_EDITOR
		[MenuItem("Assets/Create/FishAndChips/GameData/GameTipData")]
		public static void CreateAsset()
		{
			var data = ScriptableObjectUtility.CreateAsset<GameTipData>("Assets/FishAndChips/Data/Crafting/ScriptableData/Resources/Tips");
		}
#endif
	}
}
