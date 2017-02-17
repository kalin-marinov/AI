using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork
{
    public static class TrainingHelperExtensions
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

        public static string GetMatrixString(this double[,] matrix)
        {
            var sb = new StringBuilder();

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    sb.Append(matrix[row,col] + ",");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static byte ToDigit(this IList<double> data)
            => (byte)data.IndexOf(data.Max());


        public static double[] ToProbabilityArray(this byte label)
        {
            switch (label)
            {
                case 0: return new[] { 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 1: return new[] { 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 2: return new[] { 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 3: return new[] { 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 4: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 5: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d, 0.0d };
                case 6: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d, 0.0d };
                case 7: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d, 0.0d };
                case 8: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d };
                case 9: return new[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d };
                default:
                    throw new ArgumentException("invalid label " + label);
            }
        }



        const double LearnRate = 1;
        const double Momentum = 0.1;

        /// <summary> Back propagation for 3 layer network</summary>
        public static void BackPropagate(this Network net, double[] expectedValues)
        {
            var output = net.LayerValues.Last();
            var outputSignals = new double[output.Length];
            var outputBias = net.Biases.Last();
            var prevOutputBias = net.biasHistory.Last();

            for (int i = 0; i < output.Length; i++)
            {
                var error = expectedValues[i] - output[i];          // Derivative - dErr / dOut 
                var derivative = output[i] * (1 - output[i]);       // Derivative - dOut / dNet   (derivative of sigmoid)
                outputSignals[i] = error * derivative;              // Product: dErr / dOut *  dOut / dNet -  a.k.a delta

                // Update Bias:
                var delta = outputSignals[i] * LearnRate;
                outputBias[i] += delta + prevOutputBias[i] * Momentum;
                prevOutputBias[i] = delta;
            }

            var hiddenValues = net.LayerValues[1];
            var hiddenBiases = net.Biases[0];
            var prevHiddenBias = net.biasHistory[0];
            var hiddenOutputWeights = net.Weights[1]; // weights between hidden and output layer
            var hiddenNeuronSignals = new double[hiddenValues.Length];

            for (int h = 0; h < hiddenValues.Length; h++)
            {
                var derivative = hiddenValues[h] * (1 - hiddenValues[h]);

                var sum = 0.0; // need sums of output signals errors multiplied by the weight of each synapse
                for (int o = 0; o < output.Length; o++)
                    sum += outputSignals[o] * hiddenOutputWeights[h, o];

                hiddenNeuronSignals[h] = derivative * sum;

                // Update bias:
                var delta = hiddenNeuronSignals[h] * LearnRate;
                hiddenBiases[h] += delta + prevHiddenBias[h] * Momentum;
                prevHiddenBias[h] = delta;
            }

            // Update input - hidden weights using the signals calculated
            var input = net.LayerValues.First();
            var inputHiddenWeights = net.Weights.First(); // weights between input and hidden layer
            var prevInputGradients = net.gradientHistory.First();

            for (int h = 0; h < hiddenValues.Length; h++)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    var gradient = hiddenNeuronSignals[h] * input[i];
                    var delta = gradient * LearnRate;

                    inputHiddenWeights[i, h] += delta + prevInputGradients[i, h] * Momentum;
                    prevInputGradients[i, h] = delta; // store the gradient for next layer
                }
            }


            // Update hidden - output weights
            var prevOutputGradients = net.gradientHistory.Last();
            for (int j = 0; j < hiddenValues.Length; j++)
            {
                for (int k = 0; k < output.Length; k++)
                {
                    var gradient = outputSignals[k] * hiddenValues[j];
                    var delta = gradient * LearnRate;

                    hiddenOutputWeights[j, k] += delta + prevOutputGradients[j, k] * Momentum;
                    prevOutputGradients[j, k] = delta; // store the gradient for next layer
                }
            }
        }

    }
}
