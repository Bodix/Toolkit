using System.Collections.Generic;

namespace AI
{
    public static class ListExtensions
    {
        /// <summary>
        /// Reference: LinkToDB.
        /// </summary>
        public static void RemoveDuplicates<T>(this IList<T> list, IEqualityComparer<T> comparer = null)
        {
            if (list.Count <= 1)
                return;
            HashSet<T> objSet = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
            int index = 0;
            while (index < list.Count)
            {
                if (objSet.Add(list[index]))
                    ++index;
                else
                    list.RemoveAt(index);
            }
        }
    }
}