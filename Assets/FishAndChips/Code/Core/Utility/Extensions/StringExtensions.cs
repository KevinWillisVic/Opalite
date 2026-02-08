using System.IO;

namespace FishAndChips
{
    public static class StringExtensions
    {
		#region -- Public Methods --
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static string GetFileNameWithoutExtension(this string str)
		{
			return Path.GetFileNameWithoutExtension(str);
		}
		#endregion
	}
}
