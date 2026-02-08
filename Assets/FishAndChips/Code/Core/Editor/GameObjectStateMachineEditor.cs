using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FishAndChips
{
    [CustomEditor(typeof(GameObjectStateMachine))]
    public class GameObjectStateMachineEditor : Editor
	{
		#region -- Private Member Vars --
		private SerializedProperty _refreshProperty;
		#endregion

		#region -- Private Methods --
		private void OnEnable()
		{
			_refreshProperty = serializedObject.FindProperty("RefreshInStart");
		}

		private bool DisplayListCounter<T>(string label, List<T> list)
		{
			var numEnables = EditorGUILayout.IntField(label, list.Count);
			if (numEnables != list.Count)
			{
				while (list.Count > 0 && list.Count > numEnables)
				{
					list.RemoveAt(list.Count - 1);
				}
				while (list.Count < numEnables)
				{
					list.Add(default(T));
				}
				return true;
			}
			return false;
		}

		private void CheckEmptyStates()
		{
			var stateMachine = (GameObjectStateMachine)target;
			for (int i = 0; i < stateMachine.States.Count; i++)
			{
				if (stateMachine.States[i] == null)
				{
					stateMachine.States[i] = new GameObjectStateMachine.State();
					stateMachine.States[i].ToEnable = new();
				}
			}
		}

		private bool ShowArrayButtons<T>(List<T> list, bool rightJustify)
		{
			bool hasChanged = false;
			EditorGUILayout.BeginHorizontal();
			if (rightJustify)
			{
				GUILayout.FlexibleSpace();
			}
			if (GUILayout.Button("+", GUILayout.Width(30)))
			{
				list.Add(default(T));
				hasChanged = true;
			}
			if (GUILayout.Button("-", GUILayout.Width(30)))
			{
				list.RemoveAt(list.Count - 1);
				hasChanged = true;
			}
			EditorGUILayout.EndHorizontal();
			return hasChanged;
		}

		private bool ShowDisplayDropdown(string label, ref string value, string[] options)
		{
			int index = 0;
			for (int i = 0; i < options.Length; i++)
			{
				if (options[i] == value)
				{
					index = i;
				}
			}
			index = EditorGUILayout.Popup(label, index, options);
			if (value != options[index])
			{
				value = options[index];
				return true;
			}
			return false;
		}
		#endregion

		#region -- Public Methods --
		public override void OnInspectorGUI()
		{
			var stateMachine = (GameObjectStateMachine)target;
			serializedObject.Update();

			EditorGUILayout.LabelField("This script will let you define various");
			EditorGUILayout.LabelField("states and switch between them. Any Gameobject");
			EditorGUILayout.LabelField("that you attach will get enabled when its state is");
			EditorGUILayout.LabelField("active and disabled on other states without that");
			EditorGUILayout.LabelField("Gameobject in their list.");
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_refreshProperty, new GUIContent("Refresh In Start"));

			string[] options;
			if (stateMachine.States.Count == 0)
			{
				options = new string[1];
				options[0] = string.Empty;
			}
			else
			{
				options = new string[stateMachine.States.Count];
				for (int i = 0; i < stateMachine.States.Count; i++)
				{
					options[i] = stateMachine.States[i].StateName;
				}
			}

			bool refresh = false;
			if (ShowDisplayDropdown("Editor Preview State", ref stateMachine.EditorPreviewState, options))
			{
				refresh = true;
			}
			ShowDisplayDropdown("Default State", ref stateMachine.DefaultState, options);
			if (DisplayListCounter<GameObjectStateMachine.State>("Num States ", stateMachine.States))
			{
				refresh = true;
				CheckEmptyStates();
			}


			EditorGUI.indentLevel++;
			foreach (var state in stateMachine.States)
			{
				EditorGUILayout.BeginHorizontal();
				var prevName = state.StateName;
				state.StateName = EditorGUILayout.TextField("", state.StateName, GUILayout.Width(150));
				if (prevName != state.StateName)
				{
					refresh = true;
				}

				EditorGUILayout.BeginVertical();
				state.Foldout = !EditorGUILayout.Foldout(!state.Foldout, "State List");
				if (!state.Foldout)
				{
					if (DisplayListCounter<GameObject>("Length", state.ToEnable))
					{
						refresh = true;
					}
					for (int i = 0; i < state.ToEnable.Count; i++)
					{
						var enableObj = state.ToEnable[i];
						state.ToEnable[i] = (GameObject)EditorGUILayout.ObjectField("", enableObj, typeof(GameObject), true);
						if (enableObj != state.ToEnable[i])
						{
							refresh = true;
						}
					}
					if (ShowArrayButtons<GameObject>(state.ToEnable, true))
					{
						refresh = true;
					}
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}


			if (ShowArrayButtons<GameObjectStateMachine.State>(stateMachine.States, false))
			{
				CheckEmptyStates();
				refresh = true;
			}

			EditorGUI.indentLevel--;
			serializedObject.ApplyModifiedProperties();

			if (refresh)
			{
				stateMachine.Refresh();
				EditorUtility.SetDirty(stateMachine);
			}
		}
		#endregion
	}
}
