using UnityEngine;

namespace FishAndChips
{
	[CreateAssetMenu(menuName = "FishAndChips/Metadata/CraftRecipe")]
	public class CraftRecipeScriptableData : GameScriptableObject, IMetadataAsset<CraftRecipeData>
	{
		#region -- Properties --
		public string ID => _data.ID;
		public new CraftRecipeData Data => _data;
		#endregion

		#region -- Inspector --
		[SerializeField] protected CraftRecipeData _data;
		#endregion

#if UNITY_EDITOR
		public override void OnImport()
		{
		}
#endif
	}
}
