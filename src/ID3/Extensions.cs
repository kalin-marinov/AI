using System;
using System.Collections.Generic;
using System.Linq;

namespace ID3
{
    public static class Extensions
    {
        static Random rng = new Random();

        public static IEnumerable<T> GetRandom<T>(this IList<T> collection, int count)
        {
            var usedIndexes = new HashSet<int>();

            for (int i = 0; i < count && i < collection.Count;)
            {
                var randomIndex = rng.Next(0, collection.Count);

                if (!usedIndexes.Contains(randomIndex))
                {
                    i++;
                    usedIndexes.Add(randomIndex);
                    yield return collection[randomIndex];
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IList<T> data, int count)
        {
            var partitionSize = (int)Math.Ceiling((double)data.Count / count);

            for (int i = 0; i < count; i++)
            {
                var partition = data.GetRandom(partitionSize).ToArray();
                foreach (var item in partition) data.Remove(item);

                yield return partition;
            }
        }


        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> collection)
            => collection.SelectMany(x => x);

    }
}
