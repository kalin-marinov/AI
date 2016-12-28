using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ID3
{
    public static class Program
    {
        /// <summary> Contains a collection of the valid values for each attribute </summary>
        static Dictionary<int, string[]> AttributeValues;

        public static void Main()
        {
            var lines = File.ReadAllLines("breast-cancer.arff").Where(line => line.StartsWith("'"));
            var data = lines.Select(line => new DataSample(line)).ToArray();

            var allAttributes = Enumerable.Range(0, data.First().Attributes.Length); // assuming all samples have the same attributes, so the first one can be used as model

            AttributeValues = allAttributes.ToDictionary(
                keySelector: attr => attr,
                elementSelector: attr => data.Select(sample => sample.Attributes[attr]).Distinct().ToArray());

            var tree = Id3(data, allAttributes);

            var guessedTypes = data.Select(sample => new { Sample = sample, Guess = DetermineType(sample.Attributes, tree) });
            var successRate = (double)guessedTypes.Count(guess => guess.Sample.IsPositive == guess.Guess) / data.Length;
            Console.WriteLine($"Success rate is: {successRate * 100}%");
        }

        static bool DetermineType(string[] attributeValues, TreeNode decisionTree)
        {
            if (decisionTree.Result.HasValue)
                return decisionTree.Result.Value;

            var attribute = decisionTree.Attribute;
            var value = attributeValues[attribute];

            var nextNode = decisionTree.SubNodes[value];

            return DetermineType(attributeValues, nextNode);
        }

        static TreeNode Id3(IEnumerable<DataSample> set, IEnumerable<int> attributes)
        {
            if (set.All(sample => sample.IsPositive))
                return new TreeNode { Result = true };

            if (set.All(sample => !sample.IsPositive))
                return new TreeNode { Result = false };

            var bestAttribute = attributes.OrderByDescending(attr => set.CalculateInformationGain(attr)).First();
            var values = AttributeValues[bestAttribute];

            var result = new TreeNode { Attribute = bestAttribute, SubNodes = new Dictionary<string, TreeNode>() };
            foreach (var value in values)
            {
                var samplesWithTheValue = set.Where(sample => sample.Attributes[bestAttribute] == value).ToArray();
                result.SubNodes[value] = Id3(samplesWithTheValue, attributes.Except(new[] { bestAttribute }));
            }

            return result;
        }


        /// <summary>
        /// This function calculates the entropy of a data set with two classes only - positive / negative
        /// <para> Formula: H(S) = -p(x = positive) * log2(p(x = positive)) - p(x = negative) * log2(p(x = negative)) </para>
        /// </summary>
        static double CalculateEntropy(this IEnumerable<DataSample> set)
        {
            var positiveSamples = set.Count(sample => sample.IsPositive);
            var negativeSamples = set.Count(sample => !sample.IsPositive);
            var total = positiveSamples + negativeSamples;
            var posRatio = (double)positiveSamples / total;
            var negRatio = (double)negativeSamples / total;

            return -posRatio * Math.Log(posRatio, 2d) - negRatio * Math.Log(negRatio, 2d);
        }


        /// <summary>
        /// IG(S, Attr) = H(S) - SUM[p(t)*H(t)]
        /// <para> Where t is a subset created from splitting / grouping set S by Attr |
        /// p(t) - The proportion of the number of elements in t to the number of elements in set S |
        /// H(x) - entropy of the set x </para> 
        /// </summary>
        static double CalculateInformationGain(this IEnumerable<DataSample> set, int attribute)
        {
            var total = set.Count();
            var values = AttributeValues[attribute];

            double sum = 0;
            foreach (var value in values)
            {
                var itemsWithTheValue = set.Where(sample => sample.Attributes[attribute] == value).ToArray();
                var ratio = (double)itemsWithTheValue.Length / total;
                var entropy = itemsWithTheValue.CalculateEntropy();

                sum += ratio * entropy;
            }

            return set.CalculateEntropy() - sum;
        }
    }
}
