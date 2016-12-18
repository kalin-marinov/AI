using System;
using System.Collections.Generic;
using System.Linq;

namespace NaiveBayesClassifier
{
    public static class Extensions
    {
        static Random rng = new Random();

        public static IEnumerable<T> GetRandom<T>(this IList<T> collection,  int count)
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


        public static bool? ToBool(this string text)
        {
            switch (text.ToLower())
            {
                case "y": return true;
                case "n": return false;
                case "?": return null;
                default: throw new ArgumentException();
            }
        }

        public static Type ToType(this string text)
        {
            switch (text.ToLower())
            {
                case "democrat": return Type.Democrat;
                case "republican": return Type.Republican;
                default: throw new ArgumentException();
            }
        }

        public static string[] Split(this string text, string separator)
            => text.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);


        /// <summary> Increases the value for the given key with 1, If the key does not exists it will be added to the Dictionary </summary>
        public static void Increment<TKey>(this IDictionary<TKey, int> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key]++;

            else
                dictionary.Add(key, 1);
        }

    }
}
