using UnityEngine;
using UnityEditor;

namespace FishAndChips
{
    public class CraftingTool
    {
        [MenuItem("Tools/Fish And Chips/Crafting System/Ping/CraftItem folder")]
        public static void PingCraftItemFolder()
        {
			EditorUtility.FocusProjectWindow();
			Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/FishAndChips/Data/Crafting/CraftItems");
			Selection.activeObject = obj;
		}

		[MenuItem("Tools/Fish And Chips/Crafting System/Ping/CraftRecipe folder")]
		public static void PingCraftRecipeFolder()
		{
			EditorUtility.FocusProjectWindow();
			Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/FishAndChips/Data/Crafting/CraftRecipes");
			Selection.activeObject = obj;
		}

		[MenuItem("Tools/Fish And Chips/Crafting System/Open Editow Window %g")]
		public static void OpenEditorWindow()
		{
			CraftingEditorWindow.OpenWindow();
		}
	}
}
