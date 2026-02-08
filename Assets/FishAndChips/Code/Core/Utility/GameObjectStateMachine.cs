using UnityEngine;
using System;
using System.Collections.Generic;

namespace FishAndChips
{
    [ExecuteInEditMode]
    public class GameObjectStateMachine : MonoBehaviour
    {
		#region -- Supporting --
		[Serializable]
        public class State
        {
			#region -- Properties --
            public bool Foldout { get; set; }
			#endregion

			#region -- Inspector --
			public string StateName;
            public List<GameObject> ToEnable = new();
			#endregion

			#region -- Private Member Vars --
			private List<GameObject> _toDisable = new();
			#endregion

			#region -- Public Methods --
			public void Initialize()
			{
				_toDisable = new();
			}

			/// <summary>
			/// Add collection of objects to the _toDisable list.
			/// </summary>
			/// <param name="refList"></param>
			public void AddToDisabledObjectList(List<GameObject> refList)
			{
				foreach (var o in refList)
				{
					if (ToEnable.Contains(o) == false && _toDisable.Contains(o) == false)
					{
						_toDisable.Add(o);
					}
				}
			}

			public void ActivateList()
			{
				foreach (var entry in ToEnable)
				{
					entry.SetActiveSafe(true);
				}

				foreach (var entry in _toDisable)
				{
					entry.SetActiveSafe(false);
				}
			}
			#endregion
		}
		#endregion

		#region -- Inspector --
		public string EditorPreviewState;
		public string DefaultState;

		public List<State> States = new();
		public bool RefreshInStart = true;
		#endregion

		#region -- Private Member Vars --
		private Dictionary<string, State> _stateDictionary;
		#endregion

		#region -- Private Methods --
		private void Start()
		{
			if (RefreshInStart == true)
			{
				Refresh();
			}
		}

		private void GenerateDictionaries()
		{
			_stateDictionary = new();
			if (States == null)
			{
				States = new List<State>();
			}
			foreach (var state in States)
			{
				if (state != null && state.StateName.IsNullOrEmpty() == false)
				{
					_stateDictionary.Add(state.StateName, state);
					state.Initialize();

					foreach (var compState in States)
					{
						if (state != compState)
						{
							state.AddToDisabledObjectList(compState.ToEnable);
						}
					}
				}
			}
		}
		#endregion

		#region -- Public Methods --
		public void Refresh()
		{
			GenerateDictionaries();

			if (Application.isPlaying == true)
			{
				SetState(DefaultState);
			}
			else
			{
				SetState(EditorPreviewState);
			}
		}

		public void SetState(string state)
		{
			if (_stateDictionary == null || _stateDictionary.Count == 0)
			{
				GenerateDictionaries();
			}

			if (state.IsNullOrEmpty() == false)
			{
				if (_stateDictionary.ContainsKey(state))
				{
					_stateDictionary[state].ActivateList();
				}
				else if (state.IsNullOrEmpty() == false && Application.isPlaying)
				{
					Logger.LogWarning($"GameObjectStateMachine : Unable to find state : {state} at {gameObject.name}");
				}
			}
		}

		public void SetStateSafe(string state)
		{
			if (_stateDictionary == null || _stateDictionary.Count == 0)
			{
				GenerateDictionaries();
			}

			if (_stateDictionary == null)
			{
				Logger.LogWarning("Null State Machine");
				return;
			}

			SetState(state);
		}
		#endregion
	}
}
