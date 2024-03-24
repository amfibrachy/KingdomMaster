namespace _Scripts.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        public static void AddRange<T>(this ISet<T> collection, IEnumerable<T> items)
        {
            collection.UnionWith(items);
        }

        public static T GetFirstItem<T>(this ISet<T> collection)
        {
            return collection.FirstOrDefault();
        }
        
        public static List<T> GetAndRemoveElements<T>(this ISet<T> collection, int count)
        {
            var selectedElements = new List<T>();
            var elementsToRemove = new HashSet<T>();
            
            using var iterator = collection.GetEnumerator();

            while (count > 0 && iterator.MoveNext())
            {
                selectedElements.Add(iterator.Current);
                elementsToRemove.Add(iterator.Current);

                count--;
            }

            foreach (var element in elementsToRemove)
            {
                collection.Remove(element);
            }

            return selectedElements;
        }
        
        public static List<TKey> GetAndRemoveElements<TKey, TValue>(this IDictionary<TKey, TValue> collection, int count)
        {
            var selectedKeys = new List<TKey>();

            using var iterator = collection.Keys.GetEnumerator();

            while (count > 0 && iterator.MoveNext())
            {
                selectedKeys.Add(iterator.Current);
                count--;
            }

            foreach (var key in selectedKeys)
            {
                collection.Remove(key);
            }

            return selectedKeys;
        }
    }
}
