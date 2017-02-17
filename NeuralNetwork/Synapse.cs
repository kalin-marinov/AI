using System;

namespace NeuralNetwork
{
    public class Synapse
    {
        private double? cachedValue;


        public Neuron SourceNeuron { get; set; }

        public Neuron TargetNeuron { get; set; }

        public double Value
        {
            get
            {
                if (!cachedValue.HasValue)
                    cachedValue = SourceNeuron.GetValue() * Weight;

                return cachedValue.Value;
            }
        }

        public double Weight { get; set; }

        public double PreviousWeightGradient { get; set; }


        static Random rng = new Random();

        /// <summary> Creates a synapse connecting the source and destination, with a random weight between 0 and 1</summary>
        public static Synapse Randomized(Neuron source, Neuron destination)
            => new Synapse { SourceNeuron = source, TargetNeuron = destination, Weight = rng.NextDouble() * 0.001 };

        public void Reset()
          => cachedValue = null;

    }
}
