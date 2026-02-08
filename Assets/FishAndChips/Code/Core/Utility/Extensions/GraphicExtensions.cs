using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
    public static class GraphicExtensions
    {
		public static Color WithColorR(this Graphic graphic, float r) => graphic.color.WithR(r);
		public static Color WithColorG(this Graphic graphic, float g) => graphic.color.WithG(g);
		public static Color WithColorB(this Graphic graphic, float b) => graphic.color.WithB(b);
		public static Color WithColorA(this Graphic graphic, float a) => graphic.color.WithA(a);
	}
}
