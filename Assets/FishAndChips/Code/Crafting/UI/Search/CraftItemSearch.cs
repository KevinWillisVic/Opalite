using UnityEngine;
using TMPro;

namespace FishAndChips
{
    [RequireComponent(typeof(TMP_InputField))]
    public class CraftItemSearch : MonoBehaviour
    {
		#region -- Private Member Vars --
		private TMP_InputField _inputField;
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
			_inputField = GetComponent<TMP_InputField>();
			_inputField.onValueChanged.AddListener(OnSearchStringChanged);
			EventManager.SubscribeEventListener<GameResetEvent>(OnGameReset);
		}

		private void OnDestroy()
		{
			_inputField.onValueChanged.RemoveAllListeners();
			EventManager.UnsubscribeEventListener<GameResetEvent>(OnGameReset);
		}

		private void OnSearchStringChanged(string searchString)
		{
			EventManager.TriggerEvent(new CraftItemSearchEvent(searchString));
		}

		private void OnGameReset(GameResetEvent resetEvent)
		{
			ClearSearch();
		}
		#endregion

		#region -- Public Methods --
		public void ClearSearch()
		{
			if (_inputField == null)
			{
				return;
			}
			_inputField.text = string.Empty;
		}
		#endregion
	}
}
