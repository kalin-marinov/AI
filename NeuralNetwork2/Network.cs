using System;
using System.Collections.Generic;
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
        private List<double[,]> gradientHistory;

        public List<Double[]> Biases  { get; private set; }

        /// <summary> The previous bias gradients </summary>
        private List<double[]> biasHistory;




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


        public void Calculate(double[] input)
        {
            LayerNetValues[0] = input;
            LayerValues[0] = input;

            for (int i = 0; i < Weights.Count; i++)
                CalculateLayerValues(i + 1);
        }

        public void SetRandomWeights()
        {
            var rng = new Random();

            foreach (var weightMatrix in Weights)
            {
                for (int row = 0; row < weightMatrix.GetLength(0); row++)
                    for (int col = 0; col < weightMatrix.GetLength(1); col++)
                        weightMatrix[row, col] =  rng.NextDouble() * 0.001;

            }
        }


        public void CalculateLayerNetValues(int index)
        {
            var inputs = LayerValues[index - 1];      // The outputs from the previous layer
            var layer = LayerNetValues[index];        // The array where the result will be saved
            var inputWeights = Weights[index - 1];    // The weights for the synapses between this and the previous layer

            for (int curr = 0; curr < layer.Length; ++curr)
            {
                layer[curr] = 0; // Biases[index-1][curr]; // reset value

                for (int prev = 0; prev < inputs.Length; ++prev)
                    layer[curr] += inputs[prev] * inputWeights[prev, curr]; // weight * input
            }
        }

        public void CalculateLayerValues(int index)
        {
            CalculateLayerNetValues(index); // set net values
            LayerValues[index] = LayerNetValues[index].Select(MathHelper.Sigmoid).ToArray();  // apply sigmoid
        }

        const double LearnRate = 1;
        const double Momentum = 0.1;


        public void BackPropagate(double[] expectedValues)
        {
            var output = LayerValues.Last();
            var outputSignals = new double[output.Length];
            var outputBias = Biases.Last();
            var prevOutputBias = biasHistory.Last();

            for (int i = 0; i < output.Length; i++)
            {
                var error = expectedValues[i] - output[i];          // Derivative - dErr / dOut 
                var derivative = output[i] * (1 - output[i]);       // Derivative - dOut / dNet   (derivative of sigmoid)
                outputSignals[i] = error * derivative;              // Product: dErr / dOut *  dOut / dNet -  a.k.a delta

                // Update Bias:
                //var delta = outputSignals[i] * LearnRate;
                //outputBias[i] += delta + prevOutputBias[i] * Momentum;
                //prevOutputBias[i] = delta;
            }

            var hiddenValues = LayerValues[1];
            var hiddenBiases = Biases[0];
            var prevHiddenBias = biasHistory[0];
            var hiddenOutputWeights = Weights[1]; // weights between hidden and output layer
            var hiddenNeuronSignals = new double[hiddenValues.Length];

            for (int h = 0; h < hiddenValues.Length; h++)
            {
                var derivative = hiddenValues[h] * (1 - hiddenValues[h]);

                var sum = 0.0; // need sums of output signals errors multiplied by the weight of each synapse
                for (int o = 0; o < output.Length; o++)
                    sum += outputSignals[o] * hiddenOutputWeights[h, o];

                hiddenNeuronSignals[h] = derivative * sum;

                // Update bias:
                //var delta = hiddenNeuronSignals[h] * LearnRate;
                //hiddenBiases[h] += delta + prevHiddenBias[h] * Momentum;
                //prevHiddenBias[h] = delta;
            }

            // Update input - hidden weights using the signals calculated
            var input = LayerValues.First();
            var inputHiddenWeights = Weights.First(); // weights between input and hidden layer
            var prevInputGradients = gradientHistory.First();

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
            var prevOutputGradients = gradientHistory.Last();
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
