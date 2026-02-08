using UnityEditor;

namespace FishAndChips
{
    [CustomEditor(typeof(MetadataDatabase), editorForChildClasses:true)]
    public class MetadataDatabaseEditor : FishAndChipsEditor
    {
		#region -- Public Methods --
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var database = (MetadataDatabase)target;
			DrawButton("Fill",() => database.Fill());
		}
		#endregion
	}
}
