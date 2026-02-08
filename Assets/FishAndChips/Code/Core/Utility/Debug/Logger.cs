using UnityEngine;
using System;
using System.Text;

namespace FishAndChips
{
	/// <summary>
	/// Helper class to format console log messages.
	/// </summary>
    public static class Logger
    {
		#region -- Supporting --
		public enum eLogColor
		{
			Blue,
			Green,
			Red,
			Yellow,
			White
		}
		#endregion

		#region -- Private Member Vars --
		private static StringBuilder _stringBuilder = new();
		#endregion

		#region -- Public Methods --
		public static void LogException(Exception e)
		{
			Debug.LogException(e);
		}

		public static void LogException(string message)
		{
			Debug.LogException(new Exception(message));
		}

		public static void LogError(string message)
		{
			Debug.LogError(message);
		}

		public static void LogWarning(string message)
		{
			Debug.LogWarning(message);
		}

		public static void LogMessage(string message, eLogColor logColor = eLogColor.White)
		{
			Color color = Color.white;
			switch (logColor)
			{
				case eLogColor.Blue:
					color = Color.blue;
					break;
				case eLogColor.Green:
					color = Color.green;
					break;
				case eLogColor.Red:
					color = Color.red;
					break;
				case eLogColor.Yellow:
					color = Color.yellow;
					break;
				case eLogColor.White:
					color = Color.white;
					break;
			}
			LogMessage(message, color);
		}

		public static void LogMessage(string message, Color color)
		{
			string hex = ColorUtility.ToHtmlStringRGB(color);
			LogMessage(message, hex);
		}

		public static void LogMessage(string message, string hex)
		{
			hex = hex.Replace("#", "");

			_stringBuilder.Clear();
			_stringBuilder.Append("<color=#");
			_stringBuilder.Append(hex);
			_stringBuilder.Append(">");
			_stringBuilder.Append(message);
			_stringBuilder.Append("</color>");

			Debug.Log(_stringBuilder.ToString());
		}
		#endregion
	}
}
