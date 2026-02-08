using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FishAndChips.UIEnumTypesBase;

namespace FishAndChips
{
    public class UIService : Singleton<UIService>, IInitializable, ICleanable
    {
		#region -- Properties --
		public GameView ActiveView => _activeView;
		public string ActiveViewName => _activeViewName;
		public List<GameOverlay> ActiveOverlays { get; set; } = new();
		public List<GameOverlay> PermanentOverlays { get; set; } = new();
		public bool AnyActiveOverlays => ActiveOverlays.Count > 0;
		#endregion

		#region -- Protected Member Vars --
		protected UICanvas _uiCanvas;
		protected NavigationService _navigationService;

		protected GameView _activeView;
		protected string _activeViewName;
		#endregion

		#region -- Private Member Vars --

		private Dictionary<string, GameView> _viewDictionary = new();
		#endregion

		#region -- Private Methods --
		private void OrderViews()
		{
			var orderedList = _viewDictionary.Values.OrderBy(v => v.Layer?.OrderInLayer).ToList();
			int length = orderedList.Count;
			for (int i = 0; i < length; i++)
			{
				var view = orderedList[i];
				if (view == null)
				{
					continue;
				}
				view.transform.SetSiblingIndex(i);
			}
		}

		private void DestroyAllViews()
		{
			foreach (var view in _viewDictionary.Values.Where(view => view != null))
			{
				Destroy(view.gameObject);
			}
			_viewDictionary.Clear();
			_activeView = null;
			_activeViewName = string.Empty;
		}

		private void DestroyAllNonPermanentOverlays()
		{
			for (var i = 0; i < ActiveOverlays.Count; i++)
			{
				if (ActiveOverlays[i] == null)
				{
					continue;
				}
				DismissOverlay(ActiveOverlays[i].name);
			}
			ActiveOverlays.Clear();
		}

		private void DestroyAllOverlays()
		{
			if (this == null || gameObject == null)
			{
				return;
			}

			for (var i = 0; i < PermanentOverlays.Count; i++)
			{
				if (PermanentOverlays[i] == null)
				{
					continue;
				}
				Destroy(PermanentOverlays[i].gameObject);
			}

			for (var i = 0; i < ActiveOverlays.Count; i++)
			{
				if(ActiveOverlays[i] == null) 
				{
					continue;
				}
				ActiveOverlays[i].DismissSelected();
			}
			PermanentOverlays.Clear();
			ActiveOverlays.Clear();
		}

		private void OnOverlayDismissed(GameOverlay overlay)
		{
			if (overlay == null)
			{
				return;
			}
			if (ActiveOverlays.Contains(overlay))
			{
				ActiveOverlays.Remove(overlay);
			}

			if (PermanentOverlays.Contains(overlay))
			{
				PermanentOverlays.Remove(overlay);
			}
		}
		#endregion

		#region -- Protected Methods --
		protected virtual void OnViewDeactivated()
		{
			_activeViewName = string.Empty;
		}

		protected virtual void OnViewActivated(string viewName)
		{
			_activeViewName = viewName;
		}

		protected virtual void AddToDictionary(GameView view)
		{
			CheckAddView(view.name, view);
		}

		protected void CheckAddView<T>(UIEnumTypesBase.eViewTypeBase viewType, T view) where T : GameView
		{
			CheckAddView(viewType.ToString(), view);
		}

		protected void CheckAddView<T>(string viewType, T view) where T : GameView
		{
			if (view as T == null)
			{
				return;
			}
			if (_viewDictionary.ContainsKey(viewType) == false)
			{
				_viewDictionary.Add(viewType, view);
			}
		}

		protected virtual void OnNavigationRequestRejected(NavigationRequest request)
		{
		}

		protected virtual void OnNavigationRequestCompleted(NavigationRequest request)
		{
			// Condition for turning off the currently active view.
			if (ActiveView != null && ActiveView.name != request.RequestName)
			{
				DeactivateView(ActiveView);
			}

			var kvp = _viewDictionary.FirstOrDefault(v => v.Key == request.RequestName);
			if (kvp.Value == null)
			{
				Logger.LogError($"Unable to find view {request.RequestName}");
				return;
			}

			_activeView = kvp.Value;
			_activeView.IsActive = true;
			_activeView.Activate();
			OnViewActivated(request.RequestName);
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();

			_viewDictionary = new();

			_uiCanvas = UICanvas.Instance;
			_navigationService = NavigationService.Instance;
		}

		public override void Cleanup()
		{
			base.Cleanup();

			DestroyAllViews();
			DestroyAllOverlays();
		}

		// TODO : Call handle update.
		public void HandleUpdate()
		{
			if (_activeView != null)
			{
				_activeView.UpdateView();
			}

			foreach (var overlay in PermanentOverlays)
			{
				if (overlay == null)
				{
					continue;
				}
				overlay.UpdateView();
			}

			foreach (var overlay in ActiveOverlays)
			{
				if (overlay == null)
				{
					continue;
				}
				overlay.UpdateView();
			}
		}

		public GameView GetView(UIEnumTypesBase.eViewTypeBase viewType)
		{
			return GetView(viewType.ToString());
		}

		public GameView GetView(string viewType)
		{
			_viewDictionary.TryGetValue(viewType, out var returnView);
			return returnView;
		}

		public T GetView<T>() where T : GameView
		{
			foreach (var view in _viewDictionary.Values)
			{
				var viewOfType = view as T;
				if (viewOfType != null)
				{
					return viewOfType;
				}
			}
			return null;
		}

		public void DeactivateView(GameView view)
		{
			if (view == null)
			{
				return;
			}
			if (view == _activeView)
			{
				_activeView = null;
			}
			view.IsActive = false;
			view.Deactivate();
		}

		public void ActivateView(string viewType)
		{
			GameView gameView = null;

			if (_viewDictionary.TryGetValue(viewType, out gameView) == false)
			{
				Logger.LogError($"Unbale to navigate to {viewType}.");
				return;
			}

			_navigationService.QueueNavigationRequest(viewType,
				gameView,
				OnNavigationRequestCompleted, 
				OnNavigationRequestRejected);
		}

		public void ActivateView(UIEnumTypesBase.eViewTypeBase viewType)
		{
			ActivateView(viewType.ToString());
		}

		public void DestroyView<T>() where T : GameView
		{
			T toDestroy = GetView<T>();
			if (toDestroy == null)
			{
				return;
			}

			var viewName = toDestroy.name;
			_viewDictionary.Remove(viewName);

			if (_activeView == toDestroy)
			{
				Logger.LogError($"Destroyed view {viewName}, which was the active view.");
				_activeView = null;
				OnViewDeactivated();
			}

			Destroy(toDestroy.gameObject);
		}

		public bool IsViewActive(UIEnumTypesBase.eViewTypeBase viewType)
		{
			return IsViewActive(viewType.ToString());
		}

		public bool IsViewActive(string viewType)
		{
			return _activeViewName == viewType;
		}

		public void ParentObjectToRootCanvas(Transform toParent, bool worldPositionStays = true)
		{
			var canvasRect = _uiCanvas.RectTransform;
			toParent.SetParent(canvasRect, worldPositionStays);
		}

		public virtual void OrderPermanentOverlays()
		{
			PermanentOverlays = PermanentOverlays.OrderBy(p => p.Layer?.OrderInLayer).ToList();
			for (var i = 0; i < PermanentOverlays.Count; i++)
			{
				PermanentOverlays[i].transform.SetSiblingIndex(i);
			}
		}

		/// <summary>
		/// Instantiate the BootView during the game instalation sequence.
		/// </summary>
		public GameView LoadBootView()
		{
			string path = $"UI/Views/{UIEnumTypesBase.eViewTypeBase.BootView.ToString()}";
			var bootView = Resources.Load<GameView>(path);
			var bootInstance = Instantiate(bootView);

			bootInstance.name = UIEnumTypesBase.eViewTypeBase.BootView.ToString();
			bootInstance.gameObject.SetActiveSafe(true);
			bootInstance.Initialize();

			_uiCanvas.ParentToLayer(bootInstance.transform, GameViewLayer.eGameViewLayer.Boot);
			AddToDictionary(bootInstance);
			_navigationService.RegisterView(bootInstance);
			return bootInstance;
		}

		public virtual void LoadViews(string[] views = null, bool clearDictionary = true)
		{
			var viewsFromResources = Resources.LoadAll<GameView>("UI/Views");
			if (views != null)
			{
				viewsFromResources = viewsFromResources.Where(resource => views.Any(view => resource.name.Contains(view))).ToArray();
			}
			_uiCanvas.ClearLayer(GameViewLayer.eGameViewLayer.Main);

			if (clearDictionary == true)
			{
				_viewDictionary.Clear();
			}

			foreach (var view in viewsFromResources)
			{
				var instantiatedView = Instantiate(view);

				instantiatedView.name = view.name;
				instantiatedView.SetActiveSafe(false);
				instantiatedView.Initialize();

				_uiCanvas.ParentToLayer(instantiatedView.transform,
					instantiatedView.Layer?.Layer ?? GameViewLayer.eGameViewLayer.Main);

				AddToDictionary(instantiatedView);
				_navigationService.RegisterView(instantiatedView);
			}

			OrderViews();
		}

		public T ShowOverlay<T>(UIEnumTypesBase.eOverlayTypeBase overlayType,
			string title = "",
			Action<GameOverlay> onDismissed = null,
			bool isPermanent = false,
			bool forceCreateNew = false,
			GameViewLayer.eGameViewLayer layer = GameViewLayer.eGameViewLayer.Overlay) where T : GameOverlay
		{
			return ShowOverlay<T>(overlayType.ToString(), title, onDismissed, isPermanent, forceCreateNew, layer);
		}

		public T ShowOverlay<T>(string overlayType,
			string title = "",
			Action<GameOverlay> onDismissed = null,
			bool isPermanent = false,
			bool forceCreateNew = false,
			GameViewLayer.eGameViewLayer layer = GameViewLayer.eGameViewLayer.Overlay) where T : GameOverlay
		{
			return ShowOverlay(overlayType, title, onDismissed, isPermanent, forceCreateNew, layer) as T;
		}

		public GameOverlay ShowOverlay(UIEnumTypesBase.eOverlayTypeBase overlayType,
			string title = "",
			Action<GameOverlay> onDismissed = null,
			bool isPermanent = false,
			bool forceCreateNew = false,
			GameViewLayer.eGameViewLayer layer = GameViewLayer.eGameViewLayer.Overlay)
		{
			return ShowOverlay(overlayType.ToString(), title, onDismissed, isPermanent, forceCreateNew, layer);
		}

		public GameOverlay ShowOverlay(string overlayType,
			string title = "",
			Action<GameOverlay> onDismissed = null,
			bool isPermanent = false,
			bool forceCreateNew = false,
			GameViewLayer.eGameViewLayer layer = GameViewLayer.eGameViewLayer.Overlay)
		{
			var activeOverlay = ActiveOverlays.FirstOrDefault(o => o != null && o.name == overlayType);

			if (forceCreateNew == false && activeOverlay != null)
			{
				Logger.LogMessage($"Attempting to show overlay {overlayType}, but is already displayed.");
				return activeOverlay;
			}

			var searchPath = $"UI/Overlays/{overlayType}";
			var overlayFromResource = Resources.Load<GameOverlay>(searchPath);

			if (overlayFromResource == null)
			{
				throw new Exception($"Attempt to load overlay {overlayType} failed");
			}
			return ShowOverlay(overlayFromResource, title, onDismissed, isPermanent, forceCreateNew, layer);
		}

		public GameOverlay ShowOverlay(GameOverlay overlayPrefab,
			string title = "",
			Action<GameOverlay> onDismissed = null,
			bool isPermanent = false,
			bool forceCreateNew = false,
			GameViewLayer.eGameViewLayer layer = GameViewLayer.eGameViewLayer.Overlay)
		{
			if (overlayPrefab == null)
			{
				return null;
			}

			var activeOverlay = ActiveOverlays.FirstOrDefault(o => o.name == overlayPrefab.name);

			if (forceCreateNew == false && activeOverlay != null)
			{
				Logger.LogMessage($"Attempting to shouw overlay {overlayPrefab.name}, but is already displayed.");
				return activeOverlay;
			}

			var overlay = Instantiate(overlayPrefab);
			overlay.name = overlayPrefab.name;

			_uiCanvas.ParentToLayer(overlay.transform, layer);

			overlay.Initialize();
			overlay.Initialize(title);

			overlay.OnDismiss += OnOverlayDismissed;

			if (onDismissed != null)
			{
				overlay.OnDismiss += onDismissed;
			}

			overlay.Activate();

			if (isPermanent == true)
			{
				PermanentOverlays.Add(overlay);
			}
			else
			{
				ActiveOverlays.Add(overlay);
				var overlayList = ActiveOverlays.OrderBy(o => o.OrderInLayer).ToList();
				for (int i = 0; i < overlayList.Count; i++)
				{
					var o = overlayList[i];
					if (o == null)
					{
						continue;
					}
					o.transform.SetSiblingIndex(i);
				}
			}
			return overlay;
		}

		public T GetOverlay<T>() where T : GameOverlay
		{
			foreach (var overlay in PermanentOverlays)
			{
				var overlayAsType = overlay as T;
				if (overlayAsType != null)
				{
					return overlayAsType;
				}
			}

			foreach (var overlay in ActiveOverlays)
			{
				var overlayAsType = overlay as T;
				if (overlayAsType != null)
				{
					return overlayAsType;
				}
			}
			return null;
		}

		public T GetOverlay<T>(UIEnumTypesBase.eOverlayTypeBase overlayType) where T : GameOverlay
		{
			return GetOverlay(overlayType.ToString()) as T;
		}

		public GameOverlay GetOverlay(UIEnumTypesBase.eOverlayTypeBase overlayType)
		{
			return GetOverlay(overlayType.ToString());
		}

		public GameOverlay GetOverlay(string overlayType)
		{
			if (overlayType.IsNullOrEmpty())
			{
				return null;
			}

			foreach (var overlay in PermanentOverlays)
			{
				if (overlay == null)
				{
					continue;
				}
				if (overlay != null && overlay.name.Contains(overlayType))
				{
					return overlay;
				}
			}

			foreach (var overlay in ActiveOverlays)
			{
				if (overlay == null)
				{
					continue;
				}
				if (overlay != null && overlay.name.Contains(overlayType))
				{
					return overlay;
				}
			}
			return null;
		}

		public GameOverlay DismissOverlay<T>() where T : GameOverlay
		{
			foreach (var overlay in ActiveOverlays)
			{
				if (overlay as T != null)
				{
					DismissOverlay(overlay.name);
					return overlay;
				}
			}
			return null;
		}

		public GameOverlay DismissOverlay(string overlayType)
		{
			var overlay = GetOverlay(overlayType);
			if (overlay != null)
			{
				overlay.DismissSelected();
			}
			return overlay;
		}

		public void DismissActiveOverlays()
		{
			foreach (var overlay in ActiveOverlays)
			{
				overlay.DismissSelected();
			}
			ActiveOverlays.Clear();
		}

		public GameOverlay DismissOverlay(UIEnumTypesBase.eOverlayTypeBase overlayType)
		{
			return DismissOverlay(overlayType.ToString());
		}

		public virtual void LoadPermanentOverlays()
		{
			OrderPermanentOverlays();
		}
		#endregion
	}
}
