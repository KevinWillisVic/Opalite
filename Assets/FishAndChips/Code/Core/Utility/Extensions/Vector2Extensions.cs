using UnityEngine;

namespace FishAndChips
{
    public static class Vector2Extensions
    {
		#region -- Private Member Vars --
		private const int X_AXIS = 0;
		private const int Y_AXIS = 1;
		#endregion

		#region -- Public Methods --
		public static Vector2 With(this Vector2 vector, int axis, float value)
		{
			vector[axis] = value;
			return vector;
		}
		public static Vector2 WithX(this Vector2 vector, float x) => With(vector, X_AXIS, x);

		public static Vector2 WITHY(this Vector2 vector, float y) => With(vector, Y_AXIS, y);

		public static float MinComponent(this Vector2 vector) => Mathf.Min(vector.x, vector.y);

		public static float MaxComponent(this Vector2 vector) => Mathf.Max(vector.x, vector.y);

		public static Vector2 Remap(this Vector2 vector, float originRangeMin, float originRangeMax, float destinationRangeMin, float destinationRangeMax)
		{
			var newX = vector.x.Remap(originRangeMin, originRangeMax, destinationRangeMin, destinationRangeMax);
			var newY = vector.y.Remap(originRangeMin, originRangeMax, destinationRangeMin, destinationRangeMax);
			return new Vector2(newX, newY);
		}

		public static Vector2 AssignRandomPointOnCircle(this Vector2 vector, float radius)
		{
			float angle = Random.Range(0f, Mathf.PI * 2);
			vector.x = Mathf.Sin(angle) * radius;
			vector.y = Mathf.Cos(angle) * radius;
			return vector;
		}
		#endregion
	}
}
