using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KNN
{
    public static class Program
    {
        // Mostly functional implementation of K nearest neighbours
        public static void Main()
        {
            Console.WriteLine("Insert K:");
            int k = int.Parse(Console.ReadLine());

            var allItems = File.ReadAllText("iris.data.txt")
                               .Split("\n")
                               .Select(line => new IrisItem(line))
                               .ToArray();

            var testingSample = allItems.GetRandom(20).ToArray();
            var learningSet = allItems.Except(testingSample).ToArray();

            var assumedClasses = testingSample.Select(item => new
            {
                Iris = item,
                AssumedClass = item.GetNearestNeighboursIn(learningSet, count: k).GetMostCommonClass()
            }).ToArray();

            foreach (var testItem in assumedClasses)
            {
                Console.WriteLine(testItem.Iris);
                Console.WriteLine($"Assumed Class: {testItem.AssumedClass}");
                Console.WriteLine(new string('-', 30));
            }

            var accurateMatches = assumedClasses.Count(a => a.AssumedClass == a.Iris.IrisClass);
            Console.WriteLine($"Accuracy: {accurateMatches * 100 / testingSample.Length} %");
        }

        /// <summary> Returns the iris class with most occurances in the given collection </summary>
        public static IrisClass GetMostCommonClass(this IEnumerable<IrisItem> irisItems)              
            => irisItems.GroupBy(i => i.IrisClass)
                        .OrderByDescending(grp => grp.Count())
                        .First().Key;       


        /// <summary> Returns {count} amount of neighbours from the given {collection}, closest to the {item} </summary>
        public static IEnumerable<IrisItem> GetNearestNeighboursIn(this IrisItem item, IEnumerable<IrisItem> collection, int count)
           => collection.OrderBy(i => i.DistanceTo(item)).Take(count);
        
    }
}
