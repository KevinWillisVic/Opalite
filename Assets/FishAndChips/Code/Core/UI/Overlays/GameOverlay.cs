using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using TMPro;

namespace FishAndChips
{
    public class GameOverlay : GameView
    {
		#region -- Properties --
		public Action<GameOverlay> OnDismiss { get; set; }
		public Action<GameOverlay> OnDismissRequested { get; set; }
		#endregion

		#region -- Inspector --
		public bool ConsumesBackRequest;
		public TextMeshProUGUI TitleText;
		public TextMeshProUGUI DescriptionText;

		public bool GenerateBackground;
		public float GeneratedBackgroundFadeDuration = 0.2f;
		public Color BackgroundColor;
		#endregion

		#region -- Private Member Vars --
		private GameObject _generatedBackground;
		#endregion

		#region -- Private Methods --
		private void DestroySafe()
		{
			if (this != null && gameObject != null)
			{
				Destroy(gameObject);
			}
		}
		#endregion

		#region -- Protected Methods --
		protected virtual void SetTitle(string title)
		{
			TitleText.SetTextSafe(title);
		}

		protected virtual void SetDescription(string description)
		{
			DescriptionText.SetTextSafe(description);
		}

		protected void CreateBackground()
		{
			if (GenerateBackground == false)
			{
				return;
			}

			var texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, BackgroundColor);
			texture.Apply();

			_generatedBackground = new GameObject("PopupBackground");
			var image = _generatedBackground.AddComponent<Image>();
			var rect = new Rect(0, 0, texture.width, texture.height);
			var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 1);

			image.material = new Material(image.material);
			image.material.mainTexture = texture;
			image.sprite = sprite;

			var col = image.color;
			image.color = col;

			image.canvasRenderer.SetAlpha(0f);
			image.CrossFadeAlpha(1f, 0.5f, false);

			var canvas = GetComponentInParent<Canvas>();
			_generatedBackground.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;

			_generatedBackground.transform.ResetScale();
			_generatedBackground.transform.SetParent(this.transform, false);
			_generatedBackground.transform.SetSiblingIndex(0);
		}

		protected void FadeBackgroundOut()
		{
			if (_generatedBackground != null)
			{
				var image = _generatedBackground.GetComponent<Image>();
				if (image != null)
				{
					image.CrossFadeAlpha(0.0f, GeneratedBackgroundFadeDuration, false);
				}
			}
		}
		#endregion

		#region -- Public Methods --
		public override NavigationRequest.eRequestStatus SystemRequestingNavigation(string aRequestedDestination)
		{
			return NavigationRequest.eRequestStatus.Ok;
		}

		public override bool IsRoot()
		{
			return false;
		}

		public override bool AddToHistory()
		{
			return false;
		}

		public override bool DoesConsumeBackRequest()
		{
			return ConsumesBackRequest;
		}

		public virtual void Initialize(string title)
		{
			SetTitle(title);
		}

		public virtual void Initialize(string title, string description)
		{
			SetTitle(title);
			SetDescription(description);
		}

		public override void Activate()
		{
			base.Activate();
			CreateBackground();
		}

		public override void Deactivate()
		{
			base.Deactivate();

			if (_deactivationDirector == null)
			{
				OnDeactivationComplete.FireSafe(this);
				OnDismiss.FireSafe(this);
				DestroySafe();
			}
		}

		public virtual async Task WaitUntilDismissRequested()
		{
			bool completed = false;
			OnDismissRequested += (overlay) => { completed = true; };
			while (completed == false)
			{
				await Awaitable.EndOfFrameAsync();
			}
		}

		public virtual void DismissSelected()
		{
			if (_generatedBackground!= null)
			{
				FadeBackgroundOut();
			}

			if (_deactivationDirector != null)
			{
				_deactivationDirector.OnComplete += () =>
				{
					OnDeactivationComplete.FireSafe(this);
					OnDismiss.FireSafe(this);
					DestroySafe();
				};
			}

			OnDismissRequested.FireSafe(this);
			Deactivate();
		}
		#endregion
	}
}
