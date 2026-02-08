using UnityEngine.Events;

namespace FishAndChips
{
    /// <summary>
    /// This class is meant for UnityEngine.Event extensions.
    /// </summary>
    public static class UnityEventExtensions
    {
        /// <summary>
        /// Checks if UnityEvent is null before invoking.
        /// </summary>
        /// <param name="unityEvent">The UnityEvent to invoke.</param>
        public static void FireSafe(this UnityEvent unityEvent)
        {
            if (unityEvent == null)
            {
                return;
            }
            unityEvent.Invoke();
        }
    }
}
