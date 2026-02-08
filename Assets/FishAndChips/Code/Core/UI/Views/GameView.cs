using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FishAndChips
{
	public class GameView : MonoBehaviour, INavigatable
	{
		#region -- Properties --
		public Action<GameView> OnActivationComplete { get; set; }
		public Action<GameView> OnDeactivationComplete { get; set; }
		public bool IsActive { get; set; }
		public GameViewLayer Layer { get; set; }
		public int OrderInLayer => Layer == null ? 0 : Layer.OrderInLayer;
		#endregion

		#region -- Protected Member Vars --
		protected UIPlayableDirector _activationDirector;
		protected UIPlayableDirector _deactivationDirector;
		protected bool _active;

		protected UIService _uiService;
		protected NavigationService _navigationService;
		#endregion

		#region -- Protected Methods --
		protected virtual void OnDestroy()
		{
			UnsubsribeListeners();
		}

		protected bool WillStateChange(bool activeState)
		{
			return activeState != gameObject.activeSelf;
		}

		protected virtual void SubscribeListeners()
		{
			EventManager.SubscribeEventListener<OnViewActivatedEvent>(OnViewActivated);
		}

		protected virtual void UnsubsribeListeners()
		{
			EventManager.UnsubscribeEventListener<OnViewActivatedEvent>(OnViewActivated);
		}

		protected virtual void OnActivationDirectorComplete()
		{
			_active = true;
			OnActivationComplete.FireSafe(this);
			_activationDirector.OnComplete -= OnActivationDirectorComplete;
		}

		protected virtual void OnDeactivationDirectorComplete()
		{
			gameObject.SetActiveSafe(false);
			OnDeactivationComplete.FireSafe(this);
			_deactivationDirector.OnComplete -= OnDeactivationDirectorComplete;
		}

		protected virtual void PlayDeactivationDirector()
		{
			if (_activationDirector != null)
			{
				_activationDirector.OnComplete -= OnActivationDirectorComplete;
				_activationDirector.Director.Stop();
			}

			if (_deactivationDirector != null)
			{
				_deactivationDirector.OnComplete -= OnDeactivationDirectorComplete;
				_deactivationDirector.OnComplete += OnDeactivationDirectorComplete;
				_deactivationDirector.Director.Play();
			}
		}

		protected virtual void PlayActivationDirector()
		{
			if (_deactivationDirector != null)
			{
				_deactivationDirector.OnComplete -= OnDeactivationDirectorComplete;
				_deactivationDirector.Director.Stop();
			}

			if (_activationDirector != null)
			{
				_activationDirector.OnComplete -= OnActivationDirectorComplete;
				_activationDirector.OnComplete += OnActivationDirectorComplete;
				_activationDirector.Director.Play();
			}
		}

		protected virtual void OnViewActivated(OnViewActivatedEvent gameEvent)
		{
		}
		#endregion

		#region -- Public Methods --
		public virtual void Initialize()
		{
			_uiService = UIService.Instance;
			_navigationService = NavigationService.Instance;
			Layer = GetComponent<GameViewLayer>();

			UnsubsribeListeners();
			SubscribeListeners();

			var directors = GetComponentsInChildren<UIPlayableDirector>(includeInactive: true);

			var activateDirectors = directors.Where(d => d.DirectorType == UIPlayableDirector.eDirectorType.Activation).ToList();
			var deactivateDirectors = directors.Where(d => d.DirectorType == UIPlayableDirector.eDirectorType.Deactivation).ToList();

			if (activateDirectors.Count() > 0)
			{
				_activationDirector = activateDirectors.FetchRandomElement();
				_activationDirector.Initialize();
			}
			if (deactivateDirectors.Count() > 0)
			{
				_deactivationDirector = deactivateDirectors.FetchRandomElement();
				_deactivationDirector.Initialize();
			}
		}

		public virtual async Task WaitUntilOpen()
		{
			if (_active == true)
			{
				return;
			}
			bool completed = false;
			OnActivationComplete += (overlay) =>
			{
				completed = true;
			};
			while (completed == false && this != null)
			{
				await Awaitable.EndOfFrameAsync();
			}
		}

		public virtual async Task WaitUntilClose()
		{
			var completed = false;
			OnDeactivationComplete += (overlay) =>
			{
				completed = true;
			};
			while (completed == false && this != null)
			{
				await Awaitable.EndOfFrameAsync();
			}
		}

		public virtual void Activate()
		{
			gameObject.SetActiveSafe(true);

			if (_activationDirector == null)
			{
				OnActivationComplete.FireSafe(this);
				_active = true;
				return;
			}

			PlayActivationDirector();
		}

		public virtual void Deactivate()
		{
			if (this == null)
			{
				return;
			}

			if (_deactivationDirector == null)
			{
				gameObject.SetActiveSafe(false);
				return;
			}

			PlayDeactivationDirector();
		}

		public virtual void UpdateView()
		{
		}

		public virtual bool DoesConsumeBackRequest()
		{
			return false;
		}

		public virtual bool IsRoot()
		{
			return false;
		}

		public virtual bool AddToHistory()
		{
			return true;
		}

		public virtual void RequestBackNavigation()
		{
			_navigationService.OnBackPressed();
		}

		public virtual NavigationRequest.eRequestStatus SystemRequestingNavigation(string destinaton)
		{
			return NavigationRequest.eRequestStatus.Ok;
		}
		#endregion
	}
}
