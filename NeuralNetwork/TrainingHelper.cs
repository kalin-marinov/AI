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


        public static double Gradient(double expected, double actual, double neuronValue)
        {
            // dE/dw = dE/dout * dOut/dNet * dNet/dw
            return -(expected - actual) * actual * (1 - actual) * neuronValue;
        }

        public static void UpdateWeight(Synapse synapse, double expected, double actual)
        {
            var netSource = synapse.SourceNeuron.GetValueWithoutActivation();
            var gradient = Gradient(expected, actual, netSource); // this is dTotalErr / dWeight
            synapse.Weight -= gradient;
        }

    }
}
