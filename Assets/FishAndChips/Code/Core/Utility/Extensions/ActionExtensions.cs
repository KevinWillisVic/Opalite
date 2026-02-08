using System;

namespace FishAndChips
{
    /// <summary>
    /// This class is meant for System.Action extensions.
    /// </summary>
    public static class ActionExtensions
    {
		public static string UniqueMethodName(this Action action)
		{
			return $"{action.GetHashCode()}.{action.Method.ReflectedType}.{action.Method.Name}";
		}

		public static string UniqueMethodName(this Delegate instance)
		{
			return $"{instance.GetHashCode()}.{instance.Method.ReflectedType}.{instance.Method.Name}";
		}

		public static void FireSafe(this Action action)
        {
            // No Parameter.
            if (action != null)
            {
                action();
            }
        }

        public static void FireSafe<T>(this Action<T> action, T t)
        {
            // 1 Parameter.
            if (action != null)
            {
                action(t);
            }
        }

        public static void FireSafe<T, U>(this Action<T, U> action, T t, U u)
        {
            // 2 Parameter.
            if (action != null)
            {
                action(t, u);
            }
        }

        public static void FireSafe<T, U, V>(this Action<T, U, V> action, T t, U u, V v)
        {
            // 3 Parameter.
            if (action != null)
            {
                action(t, u, v);
            }
        }
    }
}
