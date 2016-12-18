using System;
using System.Collections.Generic;

namespace KNN
{
    public static class Extensions
    {
        static Random rng = new Random();

        public static IEnumerable<T> GetRandom<T>(this IList<T> list, int count)
        {
            var usedIndexes = new HashSet<int>();

            for (int i = 0; i < count && i < list.Count; )
            {
                var randomIndex = rng.Next(0, list.Count);

                if (!usedIndexes.Contains(randomIndex))
                {
                    i++;
                    usedIndexes.Add(randomIndex);
                    yield return list[randomIndex];
                }
            }
        }

        public static string[] Split(this string text, string separator)
            => text.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

        public static IrisClass ToIrisClass(this string classText)
        {
            switch (classText)
            {
                case "Iris-setosa": return IrisClass.Setosa;
                case "Iris-versicolor": return IrisClass.Versicolour;
                case "Iris-virginica": return IrisClass.Virginica;
                default: throw new ArgumentException();
            }
        }
    }
}
