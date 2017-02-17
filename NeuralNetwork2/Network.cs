using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuralNetwork
{
    public class Network
    {
        /// <summary>The values of the neurons of each layer </summary>
        public List<Double[]> LayerValues { get; private set; }

        /// <summary>The net values of the neurons of each layer (before applying activation) </summary>
        public List<Double[]> LayerNetValues { get; private set; }

        /// <summary> The weights for synapses between layers. Ex. weights[0] are the weights between layers 0 and 1 </summary>
        public List<Double[,]> Weights { get; private set; }

        /// <summary> The previous weight gradients </summary>
        public List<double[,]> gradientHistory;

        public List<Double[]> Biases { get; private set; }

        /// <summary> The previous bias gradients </summary>
        public List<double[]> biasHistory;


        /// <summary> Creates a Neural network with multiple hidden layers </summary>
        public Network(int inputCount, int[] hiddenLayersUnits, int outputCount)
        {
            LayerValues = new List<double[]>(2 + hiddenLayersUnits.Length);
            LayerNetValues = new List<double[]>(2 + hiddenLayersUnits.Length);

            Weights = new List<double[,]>(1 + hiddenLayersUnits.Length);
            gradientHistory = new List<double[,]>(1 + hiddenLayersUnits.Length);

            Biases = new List<double[]>(1 + hiddenLayersUnits.Length);
            biasHistory = new List<double[]>(1 + hiddenLayersUnits.Length);

            // Prepare arrays for input:
            LayerValues.Add(new double[inputCount]);
            LayerNetValues.Add(new double[inputCount]);

            // Prepare arrays for hidden:
            for (int i = 0; i < hiddenLayersUnits.Length; i++)
            {
                var hiddenCount = hiddenLayersUnits[i];
                var prevLayerCount = LayerNetValues[i].Length;

                Weights.Add(new double[prevLayerCount, hiddenCount]);
                gradientHistory.Add(new double[prevLayerCount, hiddenCount]);

                Biases.Add(new double[hiddenCount]);
                biasHistory.Add(new double[hiddenCount]);

                LayerValues.Add(new double[hiddenCount]);
                LayerNetValues.Add(new double[hiddenCount]);
            }

            // Prepare arrays for output:
            var lastLayerCount = hiddenLayersUnits.Last();

            Weights.Add(new double[lastLayerCount, outputCount]);
            gradientHistory.Add(new double[lastLayerCount, outputCount]);
            LayerValues.Add(new double[outputCount]);
            LayerNetValues.Add(new double[outputCount]);
            Biases.Add(new double[outputCount]);
            biasHistory.Add(new double[outputCount]);

            SetRandomWeights();
        }

        /// <summary> Creates a Neural network with a single hidden layer </summary>
        public Network(int inputCount, int hiddenUnits, int outputCount)
            : this(inputCount, new[] { hiddenUnits }, outputCount) { }


        public void SetRandomWeights()
        {
            var rng = new Random();

            foreach (var weightMatrix in Weights)
            {
                for (int row = 0; row < weightMatrix.GetLength(0); row++)
                    for (int col = 0; col < weightMatrix.GetLength(1); col++)
                        weightMatrix[row, col] = rng.NextDouble() * 0.001;

            }
        }

        public void LoadWeightsFromFile(int index, string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            var weightMatrix = Weights[index];

            for (int i = 0; i < weightMatrix.GetLength(0); i++)
            {
                var items = lines[i].Split(',').Select(double.Parse).ToArray();
                for (int j = 0; j < weightMatrix.GetLength(1); j++)
                {
                    weightMatrix[i, j] = items[j];
                }
            }
        }


        public double[] Calculate(double[] input)
        {
            LayerNetValues[0] = input;
            LayerValues[0] = input;

            for (int i = 1; i <= Weights.Count; i++)
            {
                CalculateLayerNetValues(i); // set net values
                LayerValues[i] = LayerNetValues[i].Select(MathHelper.Sigmoid).ToArray();  // apply sigmoid
            }

            return LayerValues.Last();
        }


        private void CalculateLayerNetValues(int index)
        {
            var inputs = LayerValues[index - 1];      // The outputs from the previous layer
            var layer = LayerNetValues[index];        // The array where the result will be saved
            var inputWeights = Weights[index - 1];    // The weights for the synapses between this and the previous layer

            for (int curr = 0; curr < layer.Length; ++curr)
            {
                layer[curr] = Biases[index - 1][curr]; // reset value

                for (int prev = 0; prev < inputs.Length; ++prev)
                    layer[curr] += inputs[prev] * inputWeights[prev, curr]; // weight * input
            }
        }
    }
}
