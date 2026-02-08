using UnityEditor;
using UnityEditor.UI;

namespace FishAndChips
{
    [CustomEditor(typeof(BaseButton))]
    public class BaseButtonEditor : ButtonEditor
    {
		#region -- Private Member Vars --
		private SerializedProperty TargetGraphicProperty;
        private SerializedProperty ReleaseSFXProperty;
        private SerializedProperty NonRectangularButtonProperty;
        private SerializedProperty NonRectangularButtonAlphaMinimumThresholdProperty;
        #endregion

        #region -- Protected Methods --
        protected override void OnEnable()
		{
            base.OnEnable();

			TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
			ReleaseSFXProperty = serializedObject.FindProperty("ButtonReleaseSFX");
			NonRectangularButtonProperty = serializedObject.FindProperty("NonRectangularButton");
			NonRectangularButtonAlphaMinimumThresholdProperty = serializedObject.FindProperty("NonRectangularButtonAlphaMinimumThreshold");
		}
		#endregion

		#region -- Public Methods --
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();
			EditorGUILayout.PropertyField(TargetGraphicProperty);
			EditorGUILayout.PropertyField(ReleaseSFXProperty);
			EditorGUILayout.PropertyField(NonRectangularButtonProperty);
			EditorGUILayout.PropertyField(NonRectangularButtonAlphaMinimumThresholdProperty);

			serializedObject.ApplyModifiedProperties();
		}
        #endregion
    }
}
