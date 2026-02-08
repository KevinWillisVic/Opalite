using UnityEngine;

namespace FishAndChips
{
    public static class ComponentExtensions
    {
        public static void SetActiveSafe(this Component obj, bool value)
        {
            if (obj != null && obj.gameObject != null)
            {
                obj.gameObject.SetActiveSafe(value);
            }
        }

		public static T GetOrAddComponent<T>(this Component component) where T : Component
		{
			return component.gameObject.GetOrAddComponent<T>();
		}

		public static bool TryGetComponentInChildren<T>(this Component sourceComponent, out T component, bool includeInactive = false) where T : Component
		{
			return component = sourceComponent.gameObject.GetComponentInChildren<T>(includeInactive);
		}

		public static bool TryGetComponentInParent<T>(this Component sourceComponent, out T component, bool includeInactive = false) where T : Component
		{
			return component = sourceComponent.gameObject.GetComponentInParent<T>(includeInactive);
		}
	}
}
