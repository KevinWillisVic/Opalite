using UnityEngine;
using TMPro;

namespace FishAndChips
{
    public static class TextMeshProExtensions
    {
        public static void SetTextSafe(this TMP_Text text, string message)
        {
            if (text != null)
            {
                text.SetText(message);
            }
        }

        public static void SetTextColorSafe(this TMP_Text text, Color color)
        {
            if (text != null)
            {
                text.color = color;
            }
        }
    }
}
