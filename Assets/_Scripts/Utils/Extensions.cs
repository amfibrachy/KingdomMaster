namespace _Scripts.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        public static void AddRange<T>(this ISet<T> source, IEnumerable<T> items)
        {
            source.UnionWith(items);
        }

        public static T GetFirstItem<T>(this ISet<T> source)
        {
            return source.FirstOrDefault();
        }
    }
}
