using System;
using UnityEngine;

namespace FishAndChips
{
    public static class DoubleExtensions
    {
		public static double Remap(this double value, double sourceMin, double sourceMax, double destinationMin, double destinationMax)
		{
			return destinationMin + (value - sourceMin) * (destinationMax - destinationMin) / (sourceMax - sourceMin);
		}

		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0)
			{
				return min;
			}
			if (val.CompareTo(max) > 0)
			{
				return max;
			}
			return val;
		}

		public static bool CompareEqual(this double d, double value, float epsilon = 0.0001f)
		{
			return Mathf.Abs((float)(d - value)) < epsilon;
		}

		public static bool IsZero(this double d, float epsilon = 0.0001f)
		{
			var num = d;
			if (d < 0)
			{
				num *= -1;
			}
			return num < epsilon;
		}
	}
}
