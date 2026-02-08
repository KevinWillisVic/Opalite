using UnityEngine;

namespace FishAndChips
{
    /// <summary>
    /// This class is meant for extension methods for UnityEngine.Color.
    /// </summary>
    public static class ColorExtensions
    {
        #region -- Private Member Vars --
        private const int RED_CHANNEL = 0;
        private const int GREEN_CHANNEL = 1;
        private const int BLUE_CHANNEL = 2;
        private const int ALPHA_CHANNEL = 3;
        #endregion

        #region -- Public Methods --
        public static Color With(this Color color, int channel, float value)
        {
            color[channel] = value;
            return color;
        }

		public static Color With(this Color color, int channel1, float value1, int channel2, float value2)
		{
			color[channel1] = value1;
            color[channel2] = value2;
			return color;
		}

		public static Color With(this Color color, int channel1, float value1, int channel2, float value2, int channel3, float value3)
		{
			color[channel1] = value1;
			color[channel2] = value2;
            color[channel3] = value3;
			return color;
		}

		public static Color With(this Color color, int channel1, float value1, int channel2, float value2, int channel3, float value3, int channel4, float value4)
		{
			color[channel1] = value1;
			color[channel2] = value2;
			color[channel3] = value3;
			color[channel4] = value4;
			return color;
		}

		public static Color WithR(this Color color, float r) => With(color, RED_CHANNEL, r);
		public static Color WithG(this Color color, float g) => With(color, GREEN_CHANNEL, g);
		public static Color WithB(this Color color, float b) => With(color, BLUE_CHANNEL, b);
		public static Color WithA(this Color color, float a) => With(color, ALPHA_CHANNEL, a);
		public static Color Opaque(this Color color) => With(color, ALPHA_CHANNEL, 1);
		public static Color Transparent(this Color color) => With(color, ALPHA_CHANNEL, 0);
		public static Color OneMinusColor(this Color color)
		{
			var c = new Color(color.r, color.g, color.b);
			c.WithR(1 - color.r);
			c.WithG(1 - color.g);
			c.WithB(1 - color.b);
			return c;
		}

		public static string ConvertToHexRGB(this Color32 input)
		{
			return ConvertToHexRGB((Color)input);
		}

		public static string ConvertToHexRGB(this Color input)
		{
			if (input == null)
			{
				throw new System.ArgumentNullException(nameof(input));
			}
			return ColorUtility.ToHtmlStringRGB(input);
		}

		public static string ConvertToHexRGBA(this Color32 input)
		{
			return ConvertToHexRGBA((Color)input);
		}

		public static string ConvertToHexRGBA(this Color input)
		{
			if (input == null)
			{
				throw new System.ArgumentNullException(nameof(input));
			}
			return ColorUtility.ToHtmlStringRGBA(input);
		}
		#endregion
	}
}
