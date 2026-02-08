using UnityEditor;

namespace FishAndChips
{
    [CustomEditor(typeof(CraftItemScriptableData))]
    public class CraftItemScriptableDataEditor : GameScriptableObjectEditor
    {
		#region -- Private Member Vars --
		private bool _expandCreateSection = false;
		#endregion

		#region -- Public Methods --
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var t = (CraftItemScriptableData)target;
			_expandCreateSection = EditorGUILayout.Foldout(_expandCreateSection, "Show Model Creation Tools");
			if (_expandCreateSection == true)
			{
				DrawButton("Create Model Data", () => { t.CreateModelData(); });
				DrawButton("Create Model Data At Built In Path", () => { t.CreateModelDataAtBuiltInPath(); });
			}
		}
        #endregion
    }
}
