using UnityEditor;
using UnityEngine.UIElements;

namespace FishAndChips
{
	/// <summary>
	/// Draw a line in the inspector.
	/// </summary>
	[CustomPropertyDrawer(typeof(LineAttribute))]
	public class LineAttributeDrawer : DecoratorDrawer
    {
		public override VisualElement CreatePropertyGUI()
		{
			LineAttribute attribute = this.attribute as LineAttribute;
			VisualElement container = new VisualElement()
			{
				style =
				{
					height = attribute.Height,
					backgroundColor = attribute.LineColor,
					marginBottom = 10
				}
			};
			return container;
		}
    }
}
