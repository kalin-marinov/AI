using System;
using System.Collections.Generic;

namespace Knapsack
{
    public static class Extensions
    {
        static Random rng = new Random();

        public static T GetRandom<T>(this IList<T> list)
        {
            var randomIndex = rng.Next(0, list.Count);
            return list[randomIndex];
        }

        public static IEnumerable<T> GetRandom<T>(this IList<T> list, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randomIndex = rng.Next(0, list.Count);
                yield return list[randomIndex];
            }
        }

        /// <summary> Groups adjacent elements from the collection into pairs 
        /// <para> If the collection has an odd number of items, the last one will not get into any pair </para>
        /// </summary>
        public static IEnumerable<Tuple<T, T>> CreatePairs<T>(this IEnumerable<T> collection) where T : class
        {
            T previous = null;

            foreach (var current in collection)
            {
                if (previous == null)
                {
                    previous = current;
                }
                else
                {
                    yield return Tuple.Create(previous, current);
                    previous = null;
                }
            }
        }
    }
}
