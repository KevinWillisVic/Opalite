 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.EventSystems;

namespace FishAndChips
{
    public class CollectionGroupButton : BaseButton
    {
		#region -- Inspector --
		public string ButtonName;

		public PlayableDirector PressedTimeline;
		public PlayableDirector ReleasedTimeline;
		public PlayableDirector InactiveTimeline;

		public Image PressedImage;
		public Image NotPressedImage;
		#endregion

		#region -- Public Member Vars --
		[HideInInspector] public bool Disabled = false;
		#endregion

		#region -- Private Member Vars --
		private CollectionGroup _parentGroup;
		private bool _isActive;
		#endregion

		#region -- Private Methods --
		private void ButtonReleaseTimelineComplete(object button)
		{
			SetInactive();
		}

		private bool IsThisButtonActive(string name)
		{
			return name == ButtonName;
		}
		#endregion

		#region -- Public Methods --
		public void Initialize(CollectionGroup parent)
		{
			_parentGroup = parent;
			_isActive = false;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);

			if (Disabled == false)
			{
				SetPressed();
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (eventData != null)
			{
				base.OnPointerUp(eventData);
			}

			if (eventData != null && eventData.pointerEnter != gameObject)
			{
				CheckActive();
			}
			else
			{
				if (Disabled == false)
				{
					_isActive = true;
				}

				if (_parentGroup != null)
				{
					_parentGroup.ButtonPressed(this);
				}
			}
		}

		public void CheckActive()
		{
			if (_parentGroup == null)
			{
				return;
			}

			string activeButton = _parentGroup.GetActiveButtonName();

			if (IsThisButtonActive(activeButton) == false)
			{
				if (_isActive)
				{
					SetReleased();
				}
				else
				{
					SetInactive();
				}

				_isActive = false;
			}
		}

		public void SetPressed()
		{
			InactiveTimeline.StopSafe();
			ReleasedTimeline.StopSafe();
			PressedTimeline.PlaySafe();
		}

		public void SetReleased()
		{
			InactiveTimeline.StopSafe();
			PressedTimeline.StopSafe();

			if (ReleasedTimeline != null)
			{
				ReleasedTimeline.stopped -= ButtonReleaseTimelineComplete;
				ReleasedTimeline.stopped += ButtonReleaseTimelineComplete;
				ReleasedTimeline.PlaySafe();
			}
		}

		public void SetInactive()
		{
			PressedTimeline.StopSafe();
			ReleasedTimeline.StopSafe();
			InactiveTimeline.PlaySafe();
		}

		public void SetTint(Color color)
		{
			if (PressedImage != null)
			{
				PressedImage.color = color;
			}

			if (NotPressedImage != null)
			{
				NotPressedImage.color = color;
			}
		}

		public void CleanUp()
		{
			SetReleased();
		}

		public void OnButtonActivated(string name)
		{
			if (IsThisButtonActive(name) == false)
			{
				SetInactive();
			}
			else
			{
				SetReleased();
			}
		}
		#endregion
	}
}
