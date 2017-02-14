using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class TrainingHelper
    {
        public static double[] CalculateError(IList<double> expected, IList<double> actual)
           => Enumerable.Range(0, expected.Count).Select(i => expected[i] - actual[i]).ToArray();

        public static double[] Multiply(IList<double> first, IList<double> second)
           => Enumerable.Range(0, first.Count).Select(i => first[i] * second[i]).ToArray();
    }
}
