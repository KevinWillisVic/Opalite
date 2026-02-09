using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishAndChips
{
    public class NavigationService : Singleton<NavigationService>, IInitializable, IUpdateable
    {
		#region -- Properties --
		public bool AllowBackButton { get; set; } = true;
		#endregion

		#region -- Protected Member Vars --
		protected UIService _uiService;
		#endregion

		#region -- Private Member Vars --
		private NavigationHistory _navigationHistory = new();
		private List<NavigationRequest> _activeRequests = new();
		#endregion

		#region -- Private Methods --
		private void OnNavigation(NavigationRequest requeust)
		{
			if (requeust.Navigatable.AddToHistory() == true && requeust.Navigatable.IsRoot() == false)
			{
				_navigationHistory.AddRequest(requeust.RequestName);
			}
			else
			{
				_navigationHistory.Clear();
			}
		}

		#region UNITY_ANDROID
		private void UpdateInput()
		{
			/*
			if (Input.GetKeyDown(KeyCode.Escape) == false)
			{
				return;
			}
			*/
			if (Keyboard.current.escapeKey.wasPressedThisFrame == false)
			{
				return;
			}
			
			var currentView = _uiService.ActiveView;
			if (currentView != null)
			{
				currentView.RequestBackNavigation();
			}
			else
			{
				OnBackPressed();
			}
		}
		#endregion
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			_uiService = UIService.Instance;
		}

		public void HandleUpdate()
		{
#if UNITY_ANDROID
			UpdateInput();
#endif
			for (int i = _activeRequests.Count - 1; i >= 0; i--)
			{
				var request = _activeRequests[i];
				if (request == null)
				{
					continue;
				}
				var requestStatus = NavigationRequest.eRequestStatus.Ok;

				if (_uiService.ActiveView != null)
				{
					requestStatus |= _uiService.ActiveView.SystemRequestingNavigation(_activeRequests[i].RequestName);
				}

				// Discard the request on rejection.
				if ((requestStatus & NavigationRequest.eRequestStatus.Reject) != 0)
				{
					request.OnRequestRejected(request);
					_activeRequests.Remove(request);
					return;
				}

				// Try again next frame.
				if ((requestStatus & NavigationRequest.eRequestStatus.Wait) != 0)
				{
					return;
				}

				request.OnRequestComplete(request);
				OnNavigation(request);
				_activeRequests.Remove(request);
			}
		}

		public void RegisterView(GameView view)
		{
			if (view == null)
			{
				return;
			}

			if (view.IsRoot() == true && _navigationHistory.RootViewNames.Contains(view.name) == false)
			{
				_navigationHistory.RootViewNames.Add(view.name);
			}
		}

		public void UnregisterView(GameView view)
		{
			if (view == null)
			{
				return;
			}

			if (_navigationHistory.RootViewNames.Contains(view.name) == true)
			{
				_navigationHistory.RootViewNames.Remove(view.name);
			}
		}

		public void QueueNavigationRequest(string requestName,
			INavigatable navigatable,
			Action<NavigationRequest> onRequestApproved,
			Action<NavigationRequest> onRequestRejected)
		{
			if (_activeRequests.Any(r => r.RequestName == requestName))
			{
				return;
			}

			var request = new NavigationRequest()
			{
				RequestName = requestName,
				OnRequestComplete = onRequestApproved,
				OnRequestRejected = onRequestRejected,
				Navigatable = navigatable,
			};

			_activeRequests.Insert(0, request);
		}

		public virtual void ShowGameplayView()
		{
			_uiService.ActivateView(UIEnumTypesBase.eViewTypeBase.GameplaySceneView.ToString());
		}

		public void OnBackPressed()
		{
			if (AllowBackButton == false)
			{
				return;
			}

			var overlays = _uiService.ActiveOverlays;
			bool foundDialog = false;

			for (int i = overlays.Count - 1; i >= 0; i--)
			{
				bool backRequest = overlays[i].DoesConsumeBackRequest();
				if (backRequest == true)
				{
					overlays[i].DismissSelected();
				}
				foundDialog = true;
			}

			if (foundDialog == true)
			{
				return;
			}

			var requestedViewName = _navigationHistory.Peek();
			var currentView = _uiService.ActiveView;
			if (currentView == null || currentView.DoesConsumeBackRequest() == true)
			{
				return;
			}

			requestedViewName = _navigationHistory.Pop();
			if (currentView.IsRoot() == true)
			{
				// Check on quiting application with dialog popup.
				ExitApplication();
				return;
			}

			if (_navigationHistory.RequestHistory.Count == 0
				|| requestedViewName.IsNullOrEmpty() == true)
			{
				ShowGameplayView();
			}
			else
			{
				_uiService.ActivateView(requestedViewName);
			}
		}


		public void ExitApplication()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			UnityEngine.Application.Quit();
#endif
		}
		#endregion
	}
}
