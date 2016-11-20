using System.Collections.Generic;

namespace Frogs
{
    public static class Extensions
    {
        public const int INDEX_NOT_FOUND = -1;

        /// <summary> Returns the index of a given item in a list / array 
        /// <para> Returns <see cref="INDEX_NOT_FOUND"/> (-1) if not found </para></summary>
        public static int IndexOf<T>(this IList<T> array, T item)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Equals(item))
                    return i;
            }

            return INDEX_NOT_FOUND;
        }
    }
}
