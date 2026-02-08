using UnityEngine.Playables;
using TMPro;
using UnityEngine;

namespace FishAndChips
{
    public class MessageToast : FishScript
    {
		#region -- Inspector --
		[Tooltip("Text to display the message.")]
		public TextMeshProUGUI ToastText;
		[Tooltip("Animation to display the toast popup.")]
		public PlayableDirector ToastTrack;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Callback for toast game event.
		/// </summary>
		/// <param name="gameEvent">GameEvent for toast.</param>
		private void OnToastEvent(ToastEvent gameEvent)
		{
			if (gameEvent == null)
			{
				return;
			}
			DisplayToast(gameEvent.Message);
		}
		#endregion

		#region -- Protected Methods --
		protected override void SubscribeEventListeners()
		{
			EventManager.SubscribeEventListener<ToastEvent>(OnToastEvent);
		}

		protected override void UnsubscribeEventListeners()
		{
			EventManager.UnsubscribeEventListener<ToastEvent>(OnToastEvent);
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Play toast animation. This invovles setting text and trigger animation.
		/// </summary>
		/// <param name="message">Message to set for the toast.</param>
		public void DisplayToast(string message)
		{
			if (message.IsNullOrEmpty() == true)
			{
				return;
			}
			ToastText.SetTextSafe(message);
			ToastTrack.PlaySafe();
		}
		#endregion
	}
}
