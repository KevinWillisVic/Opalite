using UnityEditor;

namespace FishAndChips
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GameScriptableObject), editorForChildClasses: true, isFallback = true)]
	public class GameScriptableObjectEditor : FishAndChipsEditor
    {
		#region -- Public Methods --
		public override void OnInspectorGUI()
		{
			var obj = target as GameScriptableObject;
			DrawButton("Create GUID", () => obj.CreateGUID());
			base.OnInspectorGUI();
		}
		#endregion
	}
}
