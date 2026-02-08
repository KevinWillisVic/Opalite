using UnityEngine;

namespace FishAndChips
{
    /// <summary>
    /// Custom attribute to draw a line in the inspector.
    /// </summary>
    public class LineAttribute : PropertyAttribute
    {
		#region -- Public Member Vars --
		public float Height;
        public Color LineColor;
		#endregion

		#region -- Constructors --
		public LineAttribute(float r = 0,
            float g = 0, 
            float b = 0,
            float height = 3)
        {
            Height = height;
            LineColor = new Color(r/255f, g/255f, b/255f);
        }
		#endregion
	}
}
