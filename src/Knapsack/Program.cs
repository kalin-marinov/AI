using System;
using System.Collections.Generic;
using System.Linq;

namespace Knapsack
{
    public class Program
    {
        public static IList<Item> AllItems;

        public static int ChangesCount => 2 * AllItems.Count;

        public static int MutateCount => 2 * AllItems.Count;

        public static int PopulationSize => 4 * AllItems.Count;

        public static long MaxWeight { get; private set; }

        static long HighestScore = -1;

        public static void Main()
        {
            //ReadInput();

            var initial = GenState.GenerateRandomStates(PopulationSize).OrderByDescending(gen => gen.Score);
            var current = GetNextGeneration(initial);

            while (true)
            {
                var next = GetNextGeneration(current);
                if (next != null)
                    current = next;
                else
                    break;
            }

            Console.WriteLine(HighestScore);
        }




        static void ReadInput()
        {
            var initialInput = Console.ReadLine().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            MaxWeight = long.Parse(initialInput.First());
            var count = int.Parse(initialInput.Last());

            AllItems = new List<Item>(count);

            for (int i = 0; i < count; i++)
            {
                var lineEntries = Console.ReadLine().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                AllItems.Add(new Item(long.Parse(lineEntries[1]), long.Parse(lineEntries[0])));
            }
        }

        static IOrderedEnumerable<GenState> GetNextGeneration(IOrderedEnumerable<GenState> currentGeneration)
        {
            var wholeGen = currentGeneration.ToArray();
            var topNotch = wholeGen.Take(PopulationSize - ChangesCount);

            var score = topNotch.First().Score;

            if (score == HighestScore)
                return null;

            HighestScore = score;

            var pairs = wholeGen.GetRandom(ChangesCount).CreatePairs();
            var allChildren = GetCrossoverChildren(pairs);

            var forMutation = wholeGen.GetRandom(MutateCount).ToList();
            forMutation.ForEach(item => item.Mutate());

            var finalResult = topNotch.Union(allChildren).Union(forMutation);
            return finalResult.OrderByDescending(state => state.Score);
        }

        static IEnumerable<GenState> GetCrossoverChildren(IEnumerable<Tuple<GenState, GenState>> pairs)
        {
            foreach (var pair in pairs)
            {
                var crossOver = pair.Item1.CrossOver(pair.Item2);
                yield return crossOver.Item1;
                yield return crossOver.Item2;
            }
        }
    }
}
