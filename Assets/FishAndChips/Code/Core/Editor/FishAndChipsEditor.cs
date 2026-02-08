using UnityEngine;
using UnityEditor;
using System;

namespace FishAndChips
{
    public class FishAndChipsEditor : Editor
    {
		#region -- Protected Methods --
		protected void DrawButtonWithGUIEnabledState(string text, bool enabledState, Action buttonAction)
		{
			GUI.enabled = enabledState;
			if (GUILayout.Button(text) == true)
			{
				buttonAction.FireSafe();
			}
		}

		protected void DrawButton(string text, Action buttonAction)
		{
			if (GUILayout.Button(text) == true)
			{
				buttonAction.FireSafe();
			}
		}

		protected void DrawUILine(Color lineColor, int lineThickness = 1, int padding = 20)
		{
			var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + lineThickness));
			r.height = lineThickness;
			r.y += padding / 2;
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, lineColor);
		}
		#endregion
	}
}
