using System;
using System.IO;
using System.Linq;

namespace NaiveBayesClassifier
{
    public static class Program
    {
        const int PartitionCount = 10;

        private static ProbabilityCalculator calculator;

        public static void Main()
        {
            var data = File.ReadAllText("house-votes-84.data.txt")
                           .Split("\n")
                           .Select(line => new VotesItem(line))
                           .ToList();


            var partitions = data.Partition(count: PartitionCount).ToList();

            for (int i = 0; i < PartitionCount; i++)
            {
                var testSets = partitions.Skip(i).Take(1);
                var learningSet = partitions.Except(testSets).SelectMany(set => set);
                var testSet = testSets.Single().ToArray();

                calculator = new ProbabilityCalculator(learningSet);

                var accurateMatches = testSet.Count(item => item.CalculateType() == item.Type);
                Console.WriteLine($"{i+1}-th test Accuracy: {accurateMatches * 100 / testSet.Length} %");
            }
        }


        public static Type CalculateType(this VotesItem vote)
        {
            // Formula: P(Democrat) * P(Democrat | p1 == value1) * P(Democrat | p2 == value2) ...
            var democratProbabilities = vote.Parameters.Select(p => calculator.GetProbability(Type.Democrat, p.Key, p.Value));
            var totalDemocrat = calculator.GetProbability(Type.Democrat) * democratProbabilities.Aggregate((p1, p2) => p1 * p2);


            // Formula: P(Republican) * P(Republican | p1 == value1) * P(Republican | p2 == value2) ...
            var republicanProbabilities = vote.Parameters.Select(p => calculator.GetProbability(Type.Republican, p.Key, p.Value));
            var totalRepublican = calculator.GetProbability(Type.Republican) * democratProbabilities.Aggregate((p1, p2) => p1 * p2);

            return totalDemocrat > totalRepublican ? Type.Democrat : Type.Republican;
        }
    }
}
