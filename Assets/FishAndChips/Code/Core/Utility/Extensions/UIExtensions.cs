using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
    public static class UIExtensions
    {
        public static void SetSpriteSafe(this Image image, Sprite sprite)
        {
            if (image == null)
            {
                return;
            }
            image.sprite = sprite;
        }
    }
}
