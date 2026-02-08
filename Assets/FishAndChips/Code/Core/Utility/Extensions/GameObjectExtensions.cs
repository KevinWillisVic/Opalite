using UnityEngine;

namespace FishAndChips
{
    public static class GameObjectExtensions
    {
		#region -- Public Methods --
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			if (gameObject.TryGetComponent(out T component))
			{
				return component;
			}
			return gameObject.AddComponent<T>();
		}

		public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive = false) where T : Component
		{
			return component = gameObject.GetComponentInChildren<T>(includeInactive);
		}

		public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component, bool includeInactive = false) where T : Component
		{
			return component = gameObject.GetComponentInParent<T>(includeInactive);
		}

		public static void SetActiveSafe(this GameObject obj, bool value)
		{
			if (obj != null && obj.activeSelf != value)
			{
				obj.SetActive(value);
			}
		}
		#endregion
	}
}
