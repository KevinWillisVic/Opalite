using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace FishAndChips
{
    public class UICanvas : Singleton<UICanvas>
    {
		#region -- Properties --
		public bool IsInputBlockedByUI
		{
			get
			{
				var touches = Input.touches;
				foreach (var touch in touches)
				{
#if UNITY_EDITOR
					if (EventSystem.current.IsPointerOverGameObject() == true)
					{
						return true;
					}
#else
					if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) == true)
					{
						return true;
					}
#endif
				}

				if (Input.mousePresent == true)
				{
					if (Input.GetMouseButtonDown(0) == true)
					{
						if (EventSystem.current.IsPointerOverGameObject() == true)
						{
							return true;
						}
					}
				}

				return false;
			}
		}
		#endregion

		#region -- Inspector --
		public Canvas Canvas;
		public RectTransform RectTransform;
		#endregion

		#region -- Public Member Vars --
		public Dictionary<GameViewLayer.eGameViewLayer, GameViewLayer> Layers = new();
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
			var allLayers = GetComponentsInChildren<GameViewLayer>();
			foreach (var layer in allLayers)
			{
				Layers.Add(layer.Layer, layer);
			}
			RectTransform = GetComponent<RectTransform>();
		}
		#endregion

		#region -- Public Methods --
		public void ClearLayer(GameViewLayer.eGameViewLayer layer)
		{
			if (Layers.ContainsKey(layer) == false)
			{
				return;
			}
			Layers[layer].DestroyChildren();
		}

		public void ParentToLayer(Transform toParent, GameViewLayer.eGameViewLayer layerType)
		{
			if (Layers.TryGetValue(layerType, out var layer) == false)
			{
				return;
			}
			layer.ParentObject(toParent, false);
		}
		#endregion
	}
}
