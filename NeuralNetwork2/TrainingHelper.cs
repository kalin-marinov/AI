using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public static class TrainingHelper
    {
        public static double[] CalculateDifference(this IList<double> actual, IList<double> expected)
           => Enumerable.Range(0, expected.Count).Select(i => expected[i] - actual[i]).ToArray();


        /// <summary> 1/2 * SUM [(targetN - outputN) ^ 2]    -- for each N in the values arrays </summary>
        public static double CalculateError(this IList<double> actual, IList<double> expected)
           =>  0.5 * actual.CalculateDifference(expected).Select(diff => diff * diff).Sum();

        public static List<T> Shuffle<T>(this IList<T> array)
        {
            var rng = new Random();
            return array.OrderBy(x => rng.Next()).ToList();
        }

    }
}
