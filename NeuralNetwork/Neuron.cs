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


        /// <summary> Useful for the input Neurons - whose values are not calculated, but set directly </summary>
        public void SetValue(double value)
            => this.value = value;


        /// <summary> The neuron value - equals to the activation function applied to the sum of all synapse values and the bias </summary>
        public double GetValue()
            => Activate(GetValueWithoutActivation());

        /// <summary> Returns the net value (without applying activation function - (b1 + w1*v1 + w2*v2 ...))
        /// Useful for back-propagation
        /// </summary>
        public double GetValueWithoutActivation()
        {
            if (!value.HasValue)
            {
                var synapseValues = InputNodes.Select(s => s.Value);
                var sum = Bias + synapseValues.Sum();
                value = sum;
            }

            return value.Value;
        }


        /// <summary> Hyperbolical tangent - a sigmoid function </summary>
        private double Activate(double value)
            => Math.Tanh(value);
    }
}
