using System.Collections.Generic;

namespace FishAndChips
{
    public static class IListExtensions
    {
		public static void Shuffle<T>(this IList<T> collection)
		{
			System.Random rand = new System.Random();
			int size = collection.Count - 1;
			for (int i = size; i > 0; i--)
			{
				int j = rand.Next(0, i);
				T temp = collection[i];
				collection[i] = collection[j];
				collection[j] = temp;
			}
		}

		public static T Pop<T>(this IList<T> collection, int index)
		{
			if (index < 0 || index >= collection.Count)
			{
				return default(T);
			}
			var element = collection[index];
			collection.RemoveAt(index);
			return element;
		}

		public static T PopRandomElement<T>(this IList<T> collection) => Pop(collection, UnityEngine.Random.Range(0, collection.Count));

		public static T FetchRandomElement<T>(this IList<T> collection)
		{
			if (collection == null || collection.Count == 0)
			{
				return default(T);
			}
			int index = UnityEngine.Random.Range(0, collection.Count);
			return collection[index];
		}
	}
}
