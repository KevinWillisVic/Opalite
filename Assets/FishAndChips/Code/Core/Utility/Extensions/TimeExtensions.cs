using UnityEngine;
using System;

namespace FishAndChips
{
    public static class TimeExtensions
    {
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private const int SECONDS_TO_MILLISECONDS = 1000;
		private const int MINUTES_TO_SECONDS = 60;
		private const int HOURS_TO_MINUTES = 60;
		private const int DAYS_TO_HOURS = 24;

		public static long ToTimestamp(this DateTime time)
		{
			return (long)(time - Epoch).TotalMilliseconds;
		}

		public static long ToTimestamp(this DateTime? time)
		{
			if (!time.HasValue)
			{
				return 0;
			}
			return (long)(time.Value - Epoch).TotalMilliseconds;
		}

		public static DateTime ToDateTime(this long timestamp)
		{
			try
			{
				return Epoch.AddMilliseconds(timestamp);
			}
			catch (ArgumentOutOfRangeException)
			{
				return DateTime.MaxValue;
			}
		}

		public static double TotalSeconds(this DateTime dateTime)
		{
			return (dateTime - Epoch).TotalSeconds;
		}
	}
}
