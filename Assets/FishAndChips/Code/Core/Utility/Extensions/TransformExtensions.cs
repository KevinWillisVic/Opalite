using UnityEngine;

namespace FishAndChips
{
    public static class TransformExtensions
    {
		#region -- Public Methods --
		public static void SetWorldPositionX(this Transform transform, float x) => transform.position = transform.position.WithX(x);

		public static void SetLocalPositionX(this Transform transform, float x) => transform.localPosition = transform.localPosition.WithX(x);

		public static void SetWorldPositionY(this Transform transform, float y) => transform.position = transform.position.WithY(y);

		public static void SetLocalPositionY(this Transform transform, float y) => transform.localPosition = transform.localPosition.WithY(y);

		public static void SetWorldSpacePositionZ(this Transform transform, float z) => transform.position = transform.position.WithZ(z);

		public static void SetLocalPositionZ(this Transform transform, float z) => transform.localPosition = transform.localPosition.WithZ(z);

		public static void ResetWorldPositionToOrigin(this Transform transform)
		{
			transform.position = Vector3.zero;
		}

		public static void ResetLocalPositionToOrigin(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
		}

		public static void ResetScale(this Transform transform)
		{
			transform.localScale = Vector3.one;
		}

		public static void FlipTransformScaleOnX(this Transform trans)
		{
			trans.localScale = trans.localScale.WithX(trans.localScale.x * -1);
		}

		public static void FlipTransformScaleOnY(this Transform trans)
		{
			trans.localScale = trans.localScale.WithY(trans.localScale.y * -1);
		}

		public static string GetHeiarchyPath(this Transform current)
		{
			if (current.parent == null)
			{
				return "/" + current.name;
			}
			return current.parent.GetHeiarchyPath() + "/" + current.name;
		}

		public static void DestroyChildren(this Transform trans)
		{
			for (int i = trans.childCount - 1; i >= 0; --i)
			{
				var child = trans.GetChild(i);
				GameObject.DestroyImmediate(child.gameObject);
			}
		}
	}
	#endregion
}
