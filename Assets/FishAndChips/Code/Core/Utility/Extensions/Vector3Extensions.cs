using UnityEngine;

namespace FishAndChips
{
    public static class Vector3Extensions
    {
		#region -- Private Member Vars --
		private const int X_AXIS = 0;
		private const int Y_AXIS = 1;
		private const int Z_AXIS = 2;
		#endregion

		#region -- Public Methods --
		public static Vector3 With(this Vector3 vector, int axis, float value)
		{
			vector[axis] = value;
			return vector;
		}

		public static Vector3 WithX(this Vector3 vector, float x) => With(vector, X_AXIS, x);
		public static Vector3 WithY(this Vector3 vector, float y) => With(vector, Y_AXIS, y);
		public static Vector3 WithZ(this Vector3 vector, float z) => With(vector, Z_AXIS, z);
		#endregion
	}
}
