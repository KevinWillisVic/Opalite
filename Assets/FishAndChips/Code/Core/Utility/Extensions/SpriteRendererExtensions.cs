using UnityEngine;

namespace FishAndChips
{
    public static class SpriteRendererExtensions
    {
        public static void SetSpriteSafe(this SpriteRenderer spriteRenderer, Sprite sprite)
        {
            if (spriteRenderer == null)
            {
                return;
            }
            spriteRenderer.sprite = sprite;
        }
    }
}
