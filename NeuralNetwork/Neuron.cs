using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public class Neuron
    {
        public double Bias { get; set; } = 0;

        public bool IsInput { get; set; } = false;

        private double? cachedNetValue;
        private double? cachedActual;


        public IList<Synapse> InputNodes { get; set; } = new List<Synapse>();

        public IList<Synapse> OutputNodes { get; set; } = new List<Synapse>();

        /// <summary> Useful for the input Neurons - whose values are not calculated, but set directly </summary>
        public void SetValue(double value)
            => this.cachedNetValue = value;


        /// <summary> The neuron value - equals to the activation function applied to the sum of all synapse values and the bias </summary>
        public double GetValue()
           =>  IsInput ? cachedNetValue.Value : Activate(GetValueWithoutActivation());
        

        /// <summary> Returns the net value (without applying activation function - (b1 + w1*v1 + w2*v2 ...))
        /// Useful for back-propagation
        /// </summary>
        public double GetValueWithoutActivation()
        {
            if (!cachedNetValue.HasValue)
            {
                var synapseValues = InputNodes.Select(s => s.Value);
                var sum = Bias + synapseValues.Sum();
                cachedNetValue = sum;
            }

            return cachedNetValue.Value;
        }


        private double Activate(double value)
        {
            if (!cachedActual.HasValue)
                cachedActual = MathHelper.Sigmoid(value);

            return cachedActual.Value;
        }


        public void Reset()
        {
            cachedActual = null;
            cachedNetValue = null;
        }
    }
}
