using UnityEngine;

namespace FishAndChips
{
    public static class FloatExtensions
    {
		public static float Remap(this float value, float sourceMin, float sourceMax, float destinationMin, float destinationMax)
		{
			return destinationMin + (value - sourceMin) * (destinationMax - destinationMin) / (sourceMax - sourceMin);
		}

		public static bool Approximately(this float value, float otherValue) => Mathf.Approximately(value, otherValue);

		public static bool CompareEqual(this float f, float value, float epsilon = 0.0001f)
		{
			return Mathf.Abs(f - value) < epsilon;
		}
	}
}
