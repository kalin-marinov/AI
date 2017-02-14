using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public class Neuron
    {
        public double Bias { get; set; } = 0;

        private double? value;

        public IList<Synapse> InputNodes { get; set; }


        /// <summary> Useful for the input Neurons - whose values are not calculated </summary>
        public void SetValue(double value)
        {
            this.value = value;
        }

        /// <summary> The neuron value - equals to the activation function applied to the sum of all synapses and the bias </summary>
        public double GetValue()
        {
            if (!value.HasValue)
            {
                var synapseValues = InputNodes.Select(s => s.Value);
                var sum = Bias + synapseValues.Sum();
                value = Activate(sum);
            }

            return value.Value;
        }

        /// <summary> Hyperbolical tangent - a sigmoid function </summary>
        public double Activate(double value)
            => Math.Tanh(value);


    }
}
