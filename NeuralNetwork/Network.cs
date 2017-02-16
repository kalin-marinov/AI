using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public class Network
    {
        public IList<Neuron>[] Layers { get; set; }

        public IList<Neuron> InputLayer => Layers.First();

        public IList<Neuron> OutputLayer => Layers.Last();

        public Network(int inputCount, int hiddenUnits, int outputCount)
        {
            var inputNodes = Enumerable.Repeat(new Neuron(), inputCount).ToList();

            for (int i = 0; i < inputCount; i++)
                inputNodes[i] = new Neuron();

            var hiddenNodes = CreateLayer(hiddenUnits, inputNodes);
            var outputNodes = CreateLayer(outputCount, hiddenNodes);

            Layers = new[] { inputNodes, hiddenNodes, outputNodes };
        }

        public void SetInput(IList<double> inputVaues)
        {
            for (int i = 0; i < InputLayer.Count; i++)
            {
                InputLayer[i].SetValue(inputVaues[i]);
                InputLayer[i].IsInput = true;
            }
        }

        public void SetWeights(int index, double[,] weights)
        {
            if (Layers[index + 1].Count != weights.GetLength(0))
                throw new ArgumentException($"The weights should contain rows equal to the amount of nodes in the {index + 1} layer");

            if (Layers[index].Count != weights.GetLength(1))
                throw new ArgumentException($"The weights should contain rows equal to the amount of nodes in the {index} layer");

            var affectedLayer = Layers[index + 1];

            for (int i = 0; i < affectedLayer.Count; i++)
            {
                var item = affectedLayer[i];
                for (int j = 0; j < item.InputNodes.Count; j++)
                {
                    var node = item.InputNodes[j];
                    node.Weight = weights[i, j];
                }
            }
        }

        /// <summary> Gets the weights between the index and index+1 th layer
        /// <para> ex. 0 means the weights between input and first hidden layer </para>
        /// </summary>
        public double[,] GetWeights(int index)
        {
            var nextLayer = Layers[index + 1];
            var prevLayer = Layers[index];

            var result = new double[nextLayer.Count, prevLayer.Count];

            for (int i = 0; i < nextLayer.Count; i++)
            {
                var item = nextLayer[i];
                for (int j = 0; j < item.InputNodes.Count; j++)
                {
                    var node = item.InputNodes[j];
                    result[i, j] = node.Weight;
                }
            }

            return result;
        }

        public IList<double> GetOutputs()
            => OutputLayer.Select(n => n.GetValue()).ToArray();


        /// <summary> Creates a layer of Neuron, that are connected to each item of the previous layer with random weights </summary>
        private static IList<Neuron> CreateLayer(int unitCount, ICollection<Neuron> previousLayer)
        {
            var layerNodes = new Neuron[unitCount];

            for (int i = 0; i < unitCount; i++)
            {
                layerNodes[i] = new Neuron();
                layerNodes[i].InputNodes = new List<Synapse>(previousLayer.Count);

                foreach (var item in previousLayer)
                {
                    var synapse = Synapse.Randomized(item, layerNodes[i]);
                    layerNodes[i].InputNodes.Add(synapse);
                    item.OutputNodes.Add(synapse);
                }
            }

            return layerNodes;
        }

        public void Reset()
        {
            foreach (var layer in Layers.Skip(1))
                foreach (var neuron in layer)
                    neuron.Reset();
        }

    }
}
