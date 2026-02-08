using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
    public class BaseButton : Button
    {
		#region -- Inspector --
		[Header("Base Class")]
		public bool HandleButtonAudio = true;
		public BaseAudioTypes.eSFXType ButtonReleaseSFX = BaseAudioTypes.eSFXType.None;

		[Tooltip("For non-rectangular buttons, the image used needs to have read/write enabled, full rect, and non atlased.")]
		public bool NonRectangularButton;
		public float NonRectangularButtonAlphaMinimumThreshold = 0.1f;
		#endregion

		#region -- Protected Member Vars --
		protected AudioService _audioService;
		#endregion

		#region -- Protected Methods --
		protected override void Awake()
		{
			base.Awake();
			if (Application.isPlaying == true)
			{
				_audioService = AudioService.Instance;
				onClick.AddListener(OnButtonClicked);

				if (NonRectangularButton == true)
				{
					var targetGraphic = base.targetGraphic as Image;
					if (targetGraphic != null)
					{
						targetGraphic.alphaHitTestMinimumThreshold = NonRectangularButtonAlphaMinimumThreshold;
					}
				}
			}
		}

		protected override void OnDestroy()
		{
			onClick.RemoveListener(OnButtonClicked);
			base.OnDestroy();
		}
		#endregion

		#region -- Public Methods --
		public virtual void OnButtonClicked()
		{
			if (_audioService == null)
			{
				return;
			}

			if (ButtonReleaseSFX != BaseAudioTypes.eSFXType.None)
			{
				_audioService.PlayUISFX(ButtonReleaseSFX.ToString());
			}
		}
		#endregion
	}
}
